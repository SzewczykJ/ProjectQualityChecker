using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectQualityChecker.Data.Database
{
    public class FileDetail
    {
        [Key]
        public int FileDetailId { get; set; }

        public string Name { get; set; }
        public string Extension { get; set; }
        public string? FullPath { get; set; }

        public Language Language { get; set; }
        public List<File> Files { get; set; }
    }
}