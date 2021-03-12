using System.ComponentModel.DataAnnotations;

namespace ProjectQualityChecker.Models
{
    public class RepositoryForm
    {
        [Required]
        [StringLength(250, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string Name { get; set; }


        [Required]
        [Url]
        public string Url { get; set; }
    }
}