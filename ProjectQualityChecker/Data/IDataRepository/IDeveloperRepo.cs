using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IDeveloperRepo
    {
        int Add(Developer developer);
        int Update(Developer developer);
        int Delete(Developer developer);
        Developer GetByEmail(string email);
    }
}