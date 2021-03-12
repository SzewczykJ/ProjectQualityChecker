using System.Collections.Generic;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Services
{
    public class CommitService : ICommitService
    {
        private readonly ICommitRepo _commitRepo;

        public CommitService(ICommitRepo commitRepo)
        {
            _commitRepo = commitRepo;
        }

        public Commit GenerateCommitFromGitCommitInfo(LibGit2Sharp.Commit commit,
            Repository repository,
            Developer developer)
        {
            return new Commit
            {
                //Branch = currentBranch,
                Message = commit.MessageShort,
                Repository = repository,
                Sha = commit.Sha,
                Developer = developer,
                Date = commit.Author.When.UtcDateTime
            };
        }

        public int Add(Commit commit)
        {
            return _commitRepo.Add(commit);
        }

        public int Update(Commit commit)
        {
            return _commitRepo.Update(commit);
        }

        public int Update(List<Commit> commits)
        {
            return _commitRepo.Update(commits);
        }

        public int Delete(Commit commit)
        {
            return _commitRepo.Delete(commit);
        }

        public CommitSummaryList GetCommitSummaries(int repositoryId)
        {
            return _commitRepo.GetCommitSummaries(repositoryId);
        }
    }
}