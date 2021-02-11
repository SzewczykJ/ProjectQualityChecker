using System.Collections.Generic;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Services
{
    public class ResultServices : IResultServices
    {
        private readonly ICommitRepo _commitRepository;
        private readonly IMetricRepo _metricRepository;

        public ResultServices(ICommitRepo commitRepo, IMetricRepo metricsRepo)
        {
            _commitRepository = commitRepo;
            _metricRepository = metricsRepo;
        }

        public CommitSummaryList Summary(int repositoryId)
        {
            var commitSummaryList = _commitRepository.GetCommitSummaries(repositoryId);
            var groupedMetrics = _metricRepository.GetAverageMetricsGroupedByCommit(repositoryId);

            foreach (var commit in commitSummaryList.CommitList)
            {
                if (!groupedMetrics.ContainsKey(commit.CommitId)) continue;
                commit.Metrics = groupedMetrics.GetValueOrDefault(commit.CommitId);
            }

            return commitSummaryList;
        }
    }
}