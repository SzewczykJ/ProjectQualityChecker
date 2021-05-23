using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            return _context.Branches.Where(b => b.Name == name).SingleOrDefault();
        }

        public Branch GetById(int branchId)
        {
            return _context.Branches.Where(b => b.BranchId == branchId).SingleOrDefault();
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

        public async Task<List<Branch>> GetAllByRepositoryId(long repositoryId)
        {
            return await _context.Commits.Where(c => c.Repository.RepositoryId == repositoryId)
                .Select(c => c.Branch).Distinct().ToListAsync();
        }
    }
}