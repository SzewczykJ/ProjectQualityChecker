using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Services.IServices;
using Commit = LibGit2Sharp.Commit;

namespace ProjectQualityChecker.Services
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IDeveloperRepo _developerRepo;

        public DeveloperService(IDeveloperRepo developerRepo)
        {
            _developerRepo = developerRepo;
        }

        public Developer CreateDeveloperFromGitCommit(Commit commit)
        {
            var developer = _developerRepo.GetByEmail(commit.Committer.Email);
            if (developer == null)
                return new Developer
                {
                    Username = commit.Committer.Name,
                    Email = commit.Committer.Email
                };

            return developer;
        }

        public int Add(Developer developer)
        {
            return _developerRepo.Add(developer);
        }

        public int Update(Developer developer)
        {
            return _developerRepo.Update(developer);
        }

        public int Delete(Developer developer)
        {
            return _developerRepo.Delete(developer);
        }

        public Developer GetByEmail(string email)
        {
            return _developerRepo.GetByEmail(email);
        }
    }
}