namespace ProjectQualityChecker.Models.Result
{
    public class RepositoryInfo
    {
        public string Name { get; set; }
        public long RepositoryId;
        public string Url { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
    }
}