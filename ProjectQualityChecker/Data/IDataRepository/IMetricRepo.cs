using System.Collections.Generic;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IMetricRepo
    {
        int Add(Metric metric);

        int Update(Metric metric);

        int Delete(Metric metric);

        Dictionary<int, AverageMetrics> GetAverageMetricsGroupedByCommit(int repositoryId);
    }
}