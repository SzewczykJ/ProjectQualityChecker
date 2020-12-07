using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IMetricRepo
    {
        int Add(Metric metric);

        int Update(Metric metric);

        int Delete(Metric metric);
    }
}