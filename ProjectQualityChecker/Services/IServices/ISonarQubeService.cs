using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectQualityChecker.Models;
using Commit = ProjectQualityChecker.Data.Database.Commit;

namespace ProjectQualityChecker.Services.IServices
{
    public interface ISonarQubeService
    {
        Task SaveDataFromRepositoryAsync(string projectName, Commit sonarCommit,
            Dictionary<string, CommitChanges> commitChanges);
    }
}