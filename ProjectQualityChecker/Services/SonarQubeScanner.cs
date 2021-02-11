using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services.IServices;
using Commit = LibGit2Sharp.Commit;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services
{
    public class SonarQubeScanner : ISonarQubeScanner
    {
        private readonly IBranchRepo _branchRepo;
        private readonly ICommitService _commitService;
        private readonly IDeveloperService _developerService;
        private readonly IRepositoryService _repositoryService;
        private readonly ISonarQubeClient _sonarQubeClient;
        private readonly ISonarQubeService _sonarQubeService;

        private readonly string SONAR_URL = "http://sonarqube:9000";

        public SonarQubeScanner(ISonarQubeClient sonarQubeClient,
            IRepositoryService repositoryService,
            ISonarQubeService sonarQubeService,
            ICommitService commitService,
            IDeveloperService developerService,
            IBranchRepo branchRepo)
        {
            _sonarQubeClient = sonarQubeClient;
            _repositoryService = repositoryService;
            _sonarQubeService = sonarQubeService;
            _commitService = commitService;
            _developerService = developerService;
            _branchRepo = branchRepo;
        }

        public async Task ScanRepositoryAsync(Repository repository, string branch)
        {
            var repositoryURL = repository.Url;
            repository.FullName = _repositoryService.GetRepositoryNameFromRepositoryUrl(repositoryURL);
            _repositoryService.Update(repository);

            using (var clonedRepository = _repositoryService.CloneRepository(repositoryURL))
            {
                if (branch != null)
                    Commands.Checkout(clonedRepository,
                        clonedRepository.Branches.FirstOrDefault(b => b.FriendlyName.Equals(branch)));
                var branchName = clonedRepository.Head.FriendlyName;

                await ScanAllCommitsFromRepositoryAsync(clonedRepository.Commits.ToArray(),
                    repository, repositoryURL, clonedRepository, branchName);
            }

            _repositoryService.DeleteRepositoryDirectory(_repositoryService.CreatePathToRepository(repositoryURL));
        }

        public async Task ScanAllCommitsFromRepositoryAsync(Commit[] commits,
            Repository sonarRepository, string repositoryURL, IRepository repository, string branchName,
            string path = null)
        {
            var userName = _repositoryService.GetUserNameFromRepositoryUrl(repositoryURL);
            var repositoryName = _repositoryService.GetRepositoryNameFromRepositoryUrl(repositoryURL);

            var project = await _sonarQubeClient.CreateProject($"{userName}-{repositoryName}");
            var token = await _sonarQubeClient.GenerateToken($"{userName}-{repositoryName}");

            if (path == null) path = _repositoryService.CreatePathToRepository(repositoryURL);

            var repositoryType = _repositoryService.GetRepositoryType(path);

            // var currentBranch = _branchRepo.GetByName(branchName);

            //    if (currentBranch == null)
            //       currentBranch = new Branch {Name = branchName};


            for (var i = commits.Length - 1; i >= 0; i--)
            {
                var actualCommit = commits[i];

                var developerOfCommit = _developerService.CreateDeveloperFromGitCommit(actualCommit);
                var commitToSave =
                    _commitService.GenerateCommitFromGitCommitInfo(actualCommit, sonarRepository, developerOfCommit);
                _commitService.Update(commitToSave);

                Dictionary<string, CommitChanges> commitChanges;
                if (i == commits.Length - 1)
                    commitChanges = GetChangedFilesFromCommit(null, actualCommit.Tree, repository);
                else
                    commitChanges = GetChangedFilesFromCommit(actualCommit.Parents.First().Tree, actualCommit.Tree,
                        repository);

                CheckoutCommit(actualCommit.Sha, path);

                ScanRepository(project, token, path, repositoryType, string.Join(",", commitChanges.Keys));

                await _sonarQubeService.SaveDataFromRepositoryAsync(project.Key, commitToSave,
                    commitChanges);
            }

            await _sonarQubeClient.RevokeToken(token.Name);
            await _sonarQubeClient.DeleteProject(project.Key);
        }


        public int CheckoutCommit(string commitSHA, string workingDirectory)
        {
            return Execute("git", $"checkout -f {commitSHA}", workingDirectory);
        }

        private Dictionary<string, CommitChanges> GetChangedFilesFromCommit(Tree oldTree, Tree newTree,
            IRepository repository)
        {
            var commitChanges = new Dictionary<string, CommitChanges>();
            if (oldTree == null && newTree == null)
                return commitChanges;

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
                    ScanDotnetProject(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
                case RepositoryService.RepositoryType.OTHER:
                {
                    ScanOtherType(createProject.Key, createToken.Token, path, changedFiles);
                    break;
                }
            }
        }

        private void ScanMavenProject(string projectKey, string loginToken, string path, string changedFiles)
        {
            BuildMaven(path);
            ScanMaven(projectKey, loginToken, path, changedFiles);
        }


        private void ScanDotnetProject(string projectkey, string loginToken, string path, string changedFiles)
        {
            StartDotnetScanner(projectkey, loginToken, path, changedFiles);
            DotnetRestore(path);
            RebuildDotnetProject(path);
            EndDotnetScanner(loginToken, path);
        }

        private int BuildMaven(string workingDirectory)
        {
            return Execute("mvn", @"clean install -DskipTests", workingDirectory);
        }

        private int ScanMaven(string projectKey, string loginToken, string workingDirectory, string changedFiles)
        {
            if (changedFiles == null)
                return Execute("mvn",
                    $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken}",
                    workingDirectory);

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