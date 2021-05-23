using System.ComponentModel.DataAnnotations;

namespace ProjectQualityChecker.Models.Result
{
    public class ResultsFilter
    {
        [Required]
        public long RepositoryId { get; set; }

        [Required]
        public int BranchId { get; set; }
    }
}