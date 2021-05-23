using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;
using ProjectQualityChecker.Models;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services.IServices
{
    public interface IRepositoryService
    {
        int Update(Repository repository);
        int Delete(Repository repository);
        int Add(Repository repository);
        Repository Create(RepositoryForm repositoryForm);
        Repository GetById(long repositoryId);
        Task<List<Repository>> GetAllAsync();
        IRepository CloneRepository(string repositoryUrl, string branch = null);
        void DeleteRepositoryDirectory(string path);
        RepositoryService.RepositoryType GetRepositoryType(string path);
        string GetUserNameFromRepositoryUrl(string repositoryUrl);
        string GetRepositoryNameFromRepositoryUrl(string repositoryUrl);
        string CreatePathToRepository(string repositoryURL);
    }
}