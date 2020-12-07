using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IFileDetailRepo
    {
        FileDetail FindByPath(string pathName);
        int Add(FileDetail fileDetail);
        int Update(FileDetail fileDetail);
        int Delete(FileDetail fileDetail);
    }
}