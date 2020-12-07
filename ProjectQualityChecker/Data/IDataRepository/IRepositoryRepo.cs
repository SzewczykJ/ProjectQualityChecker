using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IRepositoryRepo
    {
        Repository GetById(long id);
        int Add(Repository repository);
        int Update(Repository repository);
        int Delete(Repository repository);
        Task<List<Repository>> GetAllAsync();
    }
}