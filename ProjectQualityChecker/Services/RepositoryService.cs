using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;
using ProjectQualityChecker.Data.IDataRepository;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services
{
    public class RepositoryService
    {
        public enum RepositoryType
        {
            MAVEN,
            MS,
            OTHER
        }

        private readonly IRepositoryRepo _repositoryRepo;

        public RepositoryService(IRepositoryRepo repositoryRepo)
        {
            _repositoryRepo = repositoryRepo;
        }

        public int Update(Repository repository)
        {
            return _repositoryRepo.Update(repository);
        }

        public int Delete(Repository repository)
        {
            return _repositoryRepo.Delete(repository);
        }

        public int Create(Repository newRepository)
        {
            var result = _repositoryRepo.Add(newRepository);
            return result;
        }

        public Repository GetById(int repositoryId)
        {
            return _repositoryRepo.GetById(repositoryId);
        }

        public async Task<List<Repository>> GetAllAsync()
        {
            return await _repositoryRepo.GetAllAsync();
        }

        public IRepository CloneRepository(string repositoryUrl)
        {
            try
            {
                return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                    CreatePathToRepository(repositoryUrl)));
            }
            catch (NameConflictException)
            {
                DeleteRepository(CreatePathToRepository(repositoryUrl));
                return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                    CreatePathToRepository(repositoryUrl)));
            }
        }

        public void DeleteRepository(string path)
        {
            foreach (var subDir in Directory.EnumerateDirectories(path)) DeleteRepository(subDir);
            foreach (var fileName in Directory.EnumerateFiles(path))
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }

            Directory.Delete(path);
        }

        public RepositoryType GetRepositoryType(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
                if (file.ToLower().Contains("pom.xml"))
                    return RepositoryType.MAVEN;
                else if (file.ToLower().Contains(".sln")) return RepositoryType.MS;

            return RepositoryType.OTHER;
        }

        public string GetUserNameFromRepositoryUrl(string repositoryUrl)
        {
            var split = repositoryUrl.Split("/");
            return split[split.Length - 2];
        }

        public string GetNameFromRepositoryUrl(string repositoryUrl)
        {
            var split = repositoryUrl.Split("/");
            return split[split.Length - 1];
        }

        public string CreatePathToRepository(string repositoryURL)
        {
            var userName = GetUserNameFromRepositoryUrl(repositoryURL);
            var repositoryName = GetRepositoryNameFromRepositoryUrl(repositoryURL);

            return $"downloadedRepositories/{userName}/{repositoryName}";
        }

        public string GetRepositoryNameFromRepositoryUrl(string repositoryUrl)
        {
            var split = repositoryUrl.Split("/");
            return split[split.Length - 1];
        }
    }
}