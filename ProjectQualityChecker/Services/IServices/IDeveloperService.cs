using ProjectQualityChecker.Data.Database;
using Commit = LibGit2Sharp.Commit;

namespace ProjectQualityChecker.Services.IServices
{
    public interface IDeveloperService
    {
        Developer CreateDeveloperFromGitCommit(Commit commit);
        int Add(Developer developer);
        int Update(Developer developer);
        int Delete(Developer developer);
        Developer GetByEmail(string email);
    }
}