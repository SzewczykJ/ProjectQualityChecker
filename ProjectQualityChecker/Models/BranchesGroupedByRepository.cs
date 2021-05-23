using System.Collections.Generic;
using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Models
{
    public class BranchesGroupedByRepository
    {
        public long RepositoryId { get; set; }
        public List<Branch> Branches { get; set; }
    }
}