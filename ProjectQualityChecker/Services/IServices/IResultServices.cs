using ProjectQualityChecker.Models.Result;

namespace ProjectQualityChecker.Services.IServices
{
    public interface IResultServices
    {
        CommitSummaryList Summary(int repositoryId);
    }
}