using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface IBranchRepo
    {
        Branch GetByName(string name);
        int Add(Branch branch);
        int Update(Branch branch);
        int Delete(Branch branch);
    }
}