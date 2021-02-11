using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IFileRepo
    {
        File FindById(int id);
        int Add(File file);
        int Update(File file);
        int Delete(File file);
        Task<List<File>> GetListAsync(int? repositoryId = null);
    }
}