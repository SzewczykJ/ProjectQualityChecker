using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models.Result;

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

        public Dictionary<int,AverageMetrics> GetAverageMetricsGroupedByCommit(int repositoryId)
        {
            var selectedMetrics = _context.Files
                .Include(metric => metric.Metric)
                .Where(r => r.Commit.Repository.RepositoryId == repositoryId)
                .Select(metric =>new {Key = metric.Commit.CommitId, Value = metric.Metric}).AsEnumerable();
            
            var groupedMetrics = selectedMetrics.GroupBy(c => c.Key, value => value.Value);

            Dictionary<int,AverageMetrics> response = new Dictionary<int, AverageMetrics>();
            foreach (var group in groupedMetrics)
            {
                response.Add(group.Key, this.CalculateAverageMetrics(group.ToList()));
            }

            return response;
        }

        private AverageMetrics CalculateAverageMetrics(List<Metric> metrics)
        {
            return new AverageMetrics
            {
                BranchCoverage = metrics.Where(t => t.BranchCoverage.HasValue).Average(t => t.BranchCoverage),
                CodeSmells = metrics.Where(t => t.CodeSmells.HasValue).Average(t => t.CodeSmells),
                CognitiveComplexity = metrics.Where(t => t.CognitiveComplexity.HasValue)
                    .Average(t => t.CognitiveComplexity),
                Complexity = metrics.Where(t => t.Complexity.HasValue).Average(t => t.Complexity),
                DuplicatedLines = metrics.Where(t => t.DuplicatedLines.HasValue).Average(t => t.DuplicatedLines),
                NewCodeSmells = metrics.Where(t => t.NewCodeSmells.HasValue).Average(t => t.NewCodeSmells),
                CommentLines = metrics.Where(t => t.CommentLines.HasValue).Average(t => t.CommentLines),
                CommentLinesDensity = metrics.Where(t => t.CommentLinesDensity.HasValue)
                    .Average(t => t.CommentLinesDensity),
                Ncloc = metrics.Where(t => t.Ncloc.HasValue).Average(t => t.Ncloc),
                Statements = metrics.Where(t => t.Statements.HasValue).Average(t => t.Statements),
                LineCoverage = metrics.Where(t => t.LineCoverage.HasValue).Average(t => t.LineCoverage)
            };
        }
    }
}