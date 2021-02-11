using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class FileRepo : IFileRepo
    {
        private readonly AppDbContext _context;

        public FileRepo(AppDbContext context)
        {
            _context = context;
        }

        public File FindById(int id)
        {
            return _context.Files
                .Select(f => f).FirstOrDefault(f => f.FileId.Equals(id));
        }

        public int Add(File file)
        {
            _context.Files.Add(file);
            return _context.SaveChanges();
        }

        public int Update(File file)
        {
            _context.Files.Update(file);
            return _context.SaveChanges();
        }

        public int Delete(File file)
        {
            _context.Files.Remove(file);
            return _context.SaveChanges();
        }

        public async Task<List<File>> GetListAsync(int? repositoryId = null)
        {
            var result = _context.Files
                .Include(c => c.Commit)
                .ThenInclude(r => r.Repository)
                .OrderBy(r => r.Commit.Repository.Name)
                .Include(b => b.Commit.Branch)
                .OrderBy(c => c.Commit.Branch.Name)
                .AsNoTracking()
                .AsQueryable();
            if (repositoryId.HasValue) result = result.Where(r => r.Commit.Repository.RepositoryId == repositoryId);

            return await result.ToListAsync();
        }
    }
}