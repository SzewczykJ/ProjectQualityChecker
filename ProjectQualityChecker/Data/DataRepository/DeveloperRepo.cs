using System.Linq;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class DeveloperRepo : IDeveloperRepo
    {
        private readonly AppDbContext _context;

        public DeveloperRepo(AppDbContext context)
        {
            _context = context;
        }

        public int Add(Developer developer)
        {
            _context.Developers.Add(developer);
            return _context.SaveChanges();
        }

        public int Update(Developer developer)
        {
            _context.Developers.Update(developer);
            return _context.SaveChanges();
        }

        public int Delete(Developer developer)
        {
            _context.Developers.Remove(developer);
            return _context.SaveChanges();
        }

        public Developer GetByEmail(string email)
        {
            return _context.Developers.SingleOrDefault(d => d.Email == email);
        }
    }
}