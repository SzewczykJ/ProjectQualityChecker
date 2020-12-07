using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class MetricsRepo : IMetricRepo
    {
        private readonly AppDbContext _context;

        public MetricsRepo(AppDbContext context)
        {
            _context = context;
        }

        public int Add(Metric metric)
        {
            _context.Metrics.Add(metric);
            return _context.SaveChanges();
        }

        public int Update(Metric metric)
        {
            _context.Metrics.Update(metric);
            return _context.SaveChanges();
        }

        public int Delete(Metric metric)
        {
            _context.Metrics.Remove(metric);
            return _context.SaveChanges();
        }
    }
}