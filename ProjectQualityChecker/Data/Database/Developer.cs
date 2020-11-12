using System.ComponentModel.DataAnnotations;

namespace ProjectQualityChecker.Data.Database
{
    public class Developer
    {
        [Key]
        public long DeveloperId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}