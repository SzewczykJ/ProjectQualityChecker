using System.Collections.Generic;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;

namespace ProjectQualityChecker.Services.IServices
{
    public interface ICommitService
    {
        Commit GenerateCommitFromGitCommitInfo(LibGit2Sharp.Commit commit, Repository repository, Developer developer);
        int Add(Commit commit);
        int Update(Commit commit);
        int Update(List<Commit> commits);
        int Delete(Commit commit);
        CommitSummaryList GetCommitSummaries(int repositoryId);
    }
}