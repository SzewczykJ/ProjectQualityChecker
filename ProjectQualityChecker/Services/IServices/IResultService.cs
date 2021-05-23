using ProjectQualityChecker.Models.Result;

namespace ProjectQualityChecker.Services.IServices
{
    public interface IResultService
    {
        CommitSummaryList Summary(ResultsFilter resultsFilter);
    }
}