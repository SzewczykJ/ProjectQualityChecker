using System;
using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Models.Result
{
    public class CommitSummary
    {
        public int CommitId { get; set; }
#nullable enable
        public string? Sha { get; set; }
        public string? Message { get; set; }
#nullable restore
        public DateTime Date { get; set; }

        public Developer Developer { get; set; }

        public Branch Branch { get; set; }

        public AverageMetrics Metrics { get; set; }
    }
}