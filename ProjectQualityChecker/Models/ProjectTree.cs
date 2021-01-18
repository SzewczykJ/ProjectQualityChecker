using System.Collections.Generic;

namespace ProjectQualityChecker.Models
{
    public class ProjectTree
    {
        public Paging Paging { get; set; }
        public BaseComponent BaseComponent { get; set; }
        public List<Component> Components { get; set; }
    }
}