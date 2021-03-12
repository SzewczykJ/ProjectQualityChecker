using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services.IServices;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services
{
    public class RepositoryService : IRepositoryService
    {
        public enum RepositoryType
        {
            MAVEN,
            MS,
            GRADLE,
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

        public Repository Create(RepositoryForm repositoryForm)
        {
            if (repositoryForm == null) return null;
            var repository = new Repository
            {
                Name = repositoryForm.Name,
                Url = repositoryForm.Url,
                FullName = GetRepositoryNameFromRepositoryUrl(repositoryForm.Url)
            };

            Add(repository);

            return repository;
        }

        public int Add(Repository repository)
        {
            return _repositoryRepo.Add(repository);
        }

        public Repository GetById(int repositoryId)
        {
            return _repositoryRepo.GetById(repositoryId);
        }

        public async Task<List<Repository>> GetAllAsync()
        {
            return await _repositoryRepo.GetAllAsync();
        }

        public IRepository CloneRepository(string repositoryUrl, string branch = null)
        {
            try
            {
                if (branch != null && branch != String.Empty)
                {
                    return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                        CreatePathToRepository(repositoryUrl),
                        new CloneOptions()
                        {
                            BranchName = branch
                        })
                    );
                }
                else
                {
                    return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                        CreatePathToRepository(repositoryUrl)));
                }
            }
            catch (Exception ex)
            {
                if (ex is NameConflictException)
                {
                    DeleteRepositoryDirectory(CreatePathToRepository(repositoryUrl));
                    return new LibGit2Sharp.Repository(LibGit2Sharp.Repository.Clone(repositoryUrl,
                        CreatePathToRepository(repositoryUrl)));
                }

                throw new ApplicationException(
                    "Can not clone the repository. Check is it set to the public repository and try again. ", ex);
            }
        }


        public void DeleteRepositoryDirectory(string path)
        {
            foreach (var subDir in Directory.EnumerateDirectories(path)) DeleteRepositoryDirectory(subDir);
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
                else if (file.ToLower().Contains(".gradle"))
                    return RepositoryType.GRADLE;
                else if (file.ToLower().Contains(".sln"))
                    return RepositoryType.MS;

            return RepositoryType.OTHER;
        }

        public string GetUserNameFromRepositoryUrl(string repositoryUrl)
        {
            //example url => https://github.com/github/training-kit/ 
            var split = repositoryUrl.Split("/");
            return split[3];
        }

        public string GetRepositoryNameFromRepositoryUrl(string repositoryUrl)
        {
            //example url => https://github.com/github/training-kit/ 
            var split = repositoryUrl.Split("/");
            return split[4];
        }

        public string CreatePathToRepository(string repositoryURL)
        {
            var userName = GetUserNameFromRepositoryUrl(repositoryURL);
            var repositoryName = GetRepositoryNameFromRepositoryUrl(repositoryURL);

            return $"downloadedRepositories/{userName}/{repositoryName}";
        }
    }
}