using System.Collections.Generic;
using System.Linq;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class LanguageRepo : ILanguageRepo
    {
        private readonly AppDbContext _context;

        public LanguageRepo(AppDbContext context)
        {
            _context = context;
        }

        public Language FindByName(string name)
        {
            return _context.Languages.FirstOrDefault(l => l.Name.Equals(name));
        }

        public List<Language> GetAll()
        {
            return _context.Languages.ToList();
        }
    }
}