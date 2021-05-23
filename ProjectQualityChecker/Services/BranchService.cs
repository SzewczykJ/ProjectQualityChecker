using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepo _branchRepo;

        public BranchService(IBranchRepo branchRepo)
        {
            _branchRepo = branchRepo;
        }

        public int Add(Branch branch)
        {
            return _branchRepo.Add(branch);
        }

        public int Update(Branch branch)
        {
            return _branchRepo.Update(branch);
        }

        public int Delete(Branch branch)
        {
            return _branchRepo.Delete(branch);
        }

        public Branch CreateBranch(string branchName)
        {
            var storedBranch = _branchRepo.GetByName(branchName);

            if (storedBranch == null)
            {
                storedBranch = new Branch {Name = branchName};
                Add(storedBranch);
            }

            return storedBranch;
        }

        public Branch GetByName(string branchName)
        {
            return _branchRepo.GetByName(branchName);
        }

        public Branch GetById(int branchId)
        {
            return _branchRepo.GetById(branchId);
        }

        public Task<List<Branch>> GetAllByRepositoryId(long repositoryId)
        {
            return _branchRepo.GetAllByRepositoryId(repositoryId);
        }
    }
}