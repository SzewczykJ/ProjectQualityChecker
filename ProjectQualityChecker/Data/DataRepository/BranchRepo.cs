using System.Linq;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class BranchRepo : IBranchRepo
    {
        private readonly AppDbContext _context;

        public BranchRepo(AppDbContext context)
        {
            _context = context;
        }

        public Branch GetByName(string name)
        {
            return _context.Branches.SingleOrDefault(b => b.Name == name);
        }

        public int Add(Branch branch)
        {
            _context.Branches.Add(branch);
            return _context.SaveChanges();
        }

        public int Update(Branch branch)
        {
            _context.Branches.Update(branch);
            return _context.SaveChanges();
        }

        public int Delete(Branch branch)
        {
            _context.Branches.Remove(branch);
            return _context.SaveChanges();
        }
    }
}