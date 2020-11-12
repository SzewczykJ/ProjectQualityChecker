using System.Collections.Generic;

namespace ProjectQualityChecker.Data.Database
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string Name { get; set; }

        public List<Commit> Commits { get; set; }
    }
}