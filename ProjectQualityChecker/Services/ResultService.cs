using System.Collections.Generic;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Services
{
    public class ResultService : IResultService
    {
        private readonly ICommitRepo _commitRepository;
        private readonly IMetricRepo _metricRepository;

        public ResultService(ICommitRepo commitRepo, IMetricRepo metricsRepo)
        {
            _commitRepository = commitRepo;
            _metricRepository = metricsRepo;
        }

        public CommitSummaryList Summary(ResultsFilter resultsFilter)
        {
            var commitSummaryList =
                _commitRepository.GetCommitSummaries(resultsFilter.RepositoryId, resultsFilter.BranchId);
            var groupedMetrics =
                _metricRepository.GetAverageMetricsGroupedByCommit(resultsFilter.RepositoryId, resultsFilter.BranchId);

            foreach (var commit in commitSummaryList.CommitList)
            {
                if (!groupedMetrics.ContainsKey(commit.CommitId)) continue;
                commit.Metrics = groupedMetrics.GetValueOrDefault(commit.CommitId);
            }

            return commitSummaryList;
        }
    }
}