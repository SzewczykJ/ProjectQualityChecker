using System.Threading.Tasks;
using LibGit2Sharp;
using Commit = LibGit2Sharp.Commit;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace ProjectQualityChecker.Services.IServices
{
    public interface ISonarQubeScanner
    {
        Task ScanRepositoryAsync(Repository repository, string branch = null);
        Task ScanRepositorySinceLastScannedCommit(Repository repository, Data.Database.Branch branch);

        Task ScanAllCommitsFromRepositoryAsync(Commit[] commits,
            Repository sonarRepository,
            string repositoryURL,
            IRepository repository,
            string branchName);

        int CheckoutCommit(string commitSHA, string workingDirectory);
    }
}