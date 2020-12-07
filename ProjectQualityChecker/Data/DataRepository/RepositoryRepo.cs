using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class RepositoryRepo : IRepositoryRepo
    {
        private readonly AppDbContext _context;

        public RepositoryRepo(AppDbContext context)
        {
            _context = context;
        }

        public Repository GetById(long id)
        {
            return _context.Repositories.AsNoTracking().FirstOrDefault(r => r.RepositoryId == id);
        }

        public int Add(Repository repository)
        {
            _context.Repositories.Add(repository);
            return _context.SaveChanges();
        }

        public int Update(Repository repository)
        {
            _context.Repositories.Update(repository);
            return _context.SaveChanges();
        }

        public int Delete(Repository repository)
        {
            _context.Repositories.Remove(repository);
            return _context.SaveChanges();
        }

        public async Task<List<Repository>> GetAllAsync()
        {
            var result = _context.Repositories.AsQueryable();
            return await result.AsNoTracking().ToListAsync();
        }
    }
}