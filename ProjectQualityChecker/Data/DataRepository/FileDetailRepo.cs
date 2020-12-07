using System.Linq;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class FileDetailRepo : IFileDetailRepo
    {
        private readonly AppDbContext _context;

        public FileDetailRepo(AppDbContext context)
        {
            _context = context;
        }

        public FileDetail FindByPath(string pathName)
        {
            return _context.FileDetails.SingleOrDefault(p => p.FullPath == pathName);
        }

        public int Add(FileDetail fileDetail)
        {
            _context.FileDetails.Add(fileDetail);
            return _context.SaveChanges();
        }

        public int Update(FileDetail fileDetail)
        {
            _context.FileDetails.Update(fileDetail);
            return _context.SaveChanges();
        }

        public int Delete(FileDetail fileDetail)
        {
            _context.FileDetails.Remove(fileDetail);
            return _context.SaveChanges();
        }
    }
}