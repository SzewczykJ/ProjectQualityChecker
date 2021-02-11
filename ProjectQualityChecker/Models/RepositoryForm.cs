using System.ComponentModel;

namespace ProjectQualityChecker.Models
{
    public class RepositoryForm
    {
        [DisplayName("Project name")] public string Name { get; set; }

        [DisplayName("Link to repository")] public string Url { get; set; }
    }
}