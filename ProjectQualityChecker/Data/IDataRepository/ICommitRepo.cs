using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface ICommitRepo
    {
        int Add(Commit commit);
        int Update(Commit commit);
        int Update(List<Commit> commit);
        int Delete(Commit commit);
        CommitSummaryList GetCommitSummaries(int repositoryId);
    }
}