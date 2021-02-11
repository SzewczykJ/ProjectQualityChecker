using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services.IServices
{
    public interface IRepositoryService
    {
        int Update(Repository repository);
        int Delete(Repository repository);
        int Create(Repository newRepository);
        Repository GetById(int repositoryId);
        Task<List<Repository>> GetAllAsync();
        IRepository CloneRepository(string repositoryUrl);
        void DeleteRepositoryDirectory(string path);
        RepositoryService.RepositoryType GetRepositoryType(string path);
        string GetUserNameFromRepositoryUrl(string repositoryUrl);
        string GetRepositoryNameFromRepositoryUrl(string repositoryUrl);
        string CreatePathToRepository(string repositoryURL);
    }
}