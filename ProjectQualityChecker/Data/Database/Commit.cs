using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectQualityChecker.Data.Database
{
    public class Commit
    {
        [Key]
        public long CommitId { get; set; }

        public string? Sha { get; set; }
        public string? Message { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public Developer Developer { get; set; }

        [Required]
        public Repository Repository { get; set; }

        public Branch Branch { get; set; }

        public List<File> Files { get; set; }
    }
}