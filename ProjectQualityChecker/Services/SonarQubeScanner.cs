using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models;
using Branch = ProjectQualityChecker.Data.Database.Branch;
using Commit = ProjectQualityChecker.Data.Database.Commit;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services
{
    public class SonarQubeScanner
    {
        private readonly IBranchRepo _branchRepo;
        private readonly ICommitRepo _commitRepo;
        private readonly IDeveloperRepo _developerRepo;
        private readonly RepositoryService _repositoryService;
        private readonly SonarQubeClient _sonarQubeClient;
        private readonly SonarQubeService _sonarQubeService;

        private readonly string SONAR_URL = "http://sonarqube:9000";

        public SonarQubeScanner(SonarQubeClient sonarQubeClient,
            RepositoryService repositoryService,
            SonarQubeService sonarQubeService,
            ICommitRepo commitRepo,
            IDeveloperRepo developerRepo,
            IBranchRepo branchRepo)
        {
            _sonarQubeClient = sonarQubeClient;
            _repositoryService = repositoryService;
            _sonarQubeService = sonarQubeService;
            _commitRepo = commitRepo;
            _developerRepo = developerRepo;
            _branchRepo = branchRepo;
        }

        public async Task ScanRepositoryAsync(Repository repository)
        {
            var repositoryURL = repository.Url;
            string branch = null;
            var vs = repositoryURL.Split("/");
            if (vs[vs.Length - 2].Equals("tree"))
            {
                branch = vs[vs.Length - 1];
                repositoryURL = string.Join("/", vs.Take(vs.Length - 2));
            }

            using (var clonedRepository = _repositoryService.CloneRepository(repositoryURL))
            {
                if (branch != null)
                    Commands.Checkout(clonedRepository,
                        clonedRepository.Branches.FirstOrDefault(b => b.FriendlyName.Equals(branch)));

                repository.FullName = _repositoryService.GetNameFromRepositoryUrl(repositoryURL);

                _repositoryService.Update(repository);

                var branchName = clonedRepository.Head.FriendlyName;
                var sonarCommits = await ScanAllCommitsFromRepositoryAsync(clonedRepository.Commits.ToArray(),
                    repository, repositoryURL, clonedRepository,branchName);

                // var currentBranch = _branchRepo.GetByName(branchName);
                //
                // if (currentBranch == null)
                //     currentBranch = new Branch {Name = branchName};
                // else
                //     currentBranch.Commits.AddRange(sonarCommits);

       //         _branchRepo.Update(currentBranch);
            }

            _repositoryService.DeleteRepository(_repositoryService.CreatePathToRepository(repositoryURL));
        }

        private async Task<List<Commit>> ScanAllCommitsFromRepositoryAsync(LibGit2Sharp.Commit[] commits,
            Repository sonarRepository, string repositoryURL, IRepository repository,String branchName, string path = null)
        {
            var userName = _repositoryService.GetUserNameFromRepositoryUrl(repositoryURL);
            var repositoryName = _repositoryService.GetNameFromRepositoryUrl(repositoryURL);

            var project = await _sonarQubeClient.CreateProject($"{userName}-{repositoryName}");
            var token = await _sonarQubeClient.GenerateToken($"{userName}-{repositoryName}");

            if (path == null) path = _repositoryService.CreatePathToRepository(repositoryURL);

            var repositoryType = _repositoryService.GetRepositoryType(path);

           // var currentBranch = _branchRepo.GetByName(branchName);

        //    if (currentBranch == null)
         //       currentBranch = new Branch {Name = branchName};
            
            
            
            
            var commitList = new List<Commit>();

            for (var i = commits.Length - 1; i >= 0; i--)
            {
                var commit = commits[i];

                var developer = _developerRepo.GetByEmail(commit.Committer.Email);
                if (developer == null)
                    developer = new Developer
                    {
                        Username = commit.Committer.Name,
                        Email = commit.Committer.Email
                    };

                Dictionary<string, CommitChanges> commitChanges;

                if (i == commits.Length - 1)
                    commitChanges = GetChangedFilesFromCommit(null, commit.Tree, repository);
                else
                    commitChanges = GetChangedFilesFromCommit(commit.Parents.First().Tree, commit.Tree, repository);

                var sha = commit.Sha;
                CheckoutCommit(sha, path);

                var newCommit = new Commit
                {
                    //Branch = currentBranch,
                    Message = commit.Message,
                    Repository = sonarRepository,
                    Sha = sha,
                    Developer = developer,
                    Date = commit.Author.When.UtcDateTime
                };

                commitList.Add(newCommit);
                _commitRepo.Update(commitList);

                ScanRepository(project, token, path, repositoryType, string.Join(",", commitChanges.Keys));

                await _sonarQubeService.SaveDataFromRepositoryAsync(project.Key, newCommit,
                    commitChanges);
            }

            await _sonarQubeClient.RevokeToken(token.Name);
            await _sonarQubeClient.DeleteProject(project.Key);

            return commitList;
        }

        private Dictionary<string, CommitChanges> GetChangedFilesFromCommit(Tree oldTree, Tree newTree,
            IRepository repository)
        {
            var commitChanges = new Dictionary<string, CommitChanges>();

            foreach (var changes in repository.Diff.Compare<TreeChanges>(oldTree, newTree))
                commitChanges.Add(changes.Path, new CommitChanges {Status = changes.Status, SHA = changes.Oid.Sha});

            return commitChanges;
        }


        private void ScanRepository(Project createProject, ProjectToken createToken, string path,
            RepositoryService.RepositoryType repositoryType, string changedFiles)
        {
            switch (repositoryType)
            {
                case RepositoryService.RepositoryType.MAVEN:
                {
                    ScanMavenProject(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
                case RepositoryService.RepositoryType.MS:
                {
                    this.ScanDotnetProject(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
                case RepositoryService.RepositoryType.OTHER:
                {
                    this.ScanOtherType(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
            }
        }

        private void ScanMavenProject(string projectKey, string loginToken, string path, string changedFiles)
        {
            this.BuildMaven(path);
            this.ScanMaven(projectKey, loginToken, path, changedFiles);
        }


        private void ScanDotnetProject(string projectkey, string loginToken, string path, string changedFiles)
        {
            this.StartDotnetScanner(projectkey, loginToken, path, changedFiles);
            this.DotnetRestore(path);
            this.RebuildDotnetProject(path);
            this.EndDotnetScanner(loginToken, path);
        }


        public int CheckoutCommit(string commitSHA, string workingDirectory)
        {
            return Execute("git", $"checkout -f {commitSHA}", workingDirectory);
        }

        public int BuildMaven(string workingDirectory)
        {
            return Execute("mvn", @"clean install -DskipTests", workingDirectory);
        }

        private int ScanMaven(string projectKey, string loginToken, string workingDirectory, string changedFiles)
        {
            if (changedFiles == null)
            {
                return Execute("mvn",
                    $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken}",
                    workingDirectory);
            }

            return Execute("mvn",
                $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken} -Dsonar.inclusions={changedFiles}",
                workingDirectory);
        }

        private int StartDotnetScanner(string projectKey, string loginToken, string workingDirectory,
            string changedFiles)
        {
            if (changedFiles == null)
                return Execute("/bin/bash",
                    $@"-c ""dotnet sonarscanner begin /k:""{projectKey}"" /d:sonar.host.url={SONAR_URL} /d:sonar.login=""{loginToken}"" ",
                    workingDirectory);

            return Execute("/bin/bash",
                @"-c ""dotnet sonarscanner begin /k:""{projectKey}"" /d:sonar.host.url={SONAR_URL} /d:sonar.login=""{loginToken}"" /d:sonar.inclusions=""{changedFiles}""""",
                workingDirectory);
        }

        private int DotnetRestore(string workingDirectory)
        {
            return Execute("/bin/bash", @"-c ""dotnet restore""", workingDirectory);
        }

        private int RebuildDotnetProject(string workingDirectory)
        {
            return Execute("/bin/bash", @"-c ""dotnet build""", workingDirectory);
        }

        private int EndDotnetScanner(string loginToken, string workingDirectory)
        {
            return Execute("/bin/bash", $@"-c ""dotnet sonarscanner end /d:sonar.login=""{loginToken}""""",
                workingDirectory);
        }

        private int ScanOtherType(string projectKey, string loginToken, string workingDirectory, string changedFiles)
        {
            if (changedFiles == null)
                return Execute("/bin/bash",
                    $@"-c ""sonar-scanner -Dsonar.projectKey={projectKey} -Dsonar.sources=. -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken}""",
                    workingDirectory);


            return Execute("/bin/bash",
                $@"-c ""sonar-scanner -Dsonar.projectKey={projectKey} -Dsonar.sources=. -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken} -Dsonar.inclusions={changedFiles}""",
                workingDirectory);
        }

        private int Execute(string command, string argument, string workingDirectory)
        {
            var process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = argument;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            var err = process.StandardError.ReadToEnd();
            Console.WriteLine(err);

            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Close();

            return exitCode;
        }
    }
}