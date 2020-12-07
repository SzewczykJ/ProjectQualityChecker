using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class CommitRepo : ICommitRepo
    {
        private readonly AppDbContext _context;

        public CommitRepo(AppDbContext context)
        {
            _context = context;
        }

        public int Add(Commit commit)
        {
            _context.Commits.Add(commit);
            return _context.SaveChanges();
        }

        public int Update(Commit commit)
        {
            _context.Commits.Update(commit);
            return _context.SaveChanges();
        }

        public int Update(List<Commit> commit)
        {
            _context.Commits.UpdateRange(commit);
            return _context.SaveChanges();
        }

        public int Delete(Commit commit)
        {
            _context.Commits.Remove(commit);
            return _context.SaveChanges();
        }
    }
}