using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models.Result;

namespace ProjectQualityChecker.Data.DataRepository
{
    public class CommitRepo : ICommitRepo
    {
        private readonly AppDbContext _context;

        public CommitRepo(AppDbContext context)
        {
            _context = context;
        }

        public int Add(Commit commit)
        {
            _context.Commits.Add(commit);
            return _context.SaveChanges();
        }

        public int Update(Commit commit)
        {
            _context.Commits.Update(commit);
            return _context.SaveChanges();
        }

        public int Update(List<Commit> commit)
        {
            _context.Commits.UpdateRange(commit);
            return _context.SaveChanges();
        }

        public int Delete(Commit commit)
        {
            _context.Commits.Remove(commit);
            return _context.SaveChanges();
        }

        public CommitSummaryList GetCommitSummaries(long repositoryId, int? branchId = null)
        {
            var response = new CommitSummaryList();

            var query = _context.Commits
                .Include(dev => dev.Developer)
                .Include(branch => branch.Branch)
                .Where(r => r.Repository.RepositoryId == repositoryId).AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(r => r.Branch.BranchId == (int) branchId);
            }

            response.CommitList = query.Select(c => new CommitSummary
            {
                Developer = c.Developer,
                Date = c.Date,
                Message = c.Message,
                Sha = c.Sha,
                CommitId = c.CommitId
            }).ToList();

            return response;
        }

        public Task<Commit> FindLast(long repositoryId, int branchId)
        {
            var response = _context.Commits
                .Where(r => r.Repository.RepositoryId == repositoryId)
                .Where(r => r.Branch.BranchId == branchId)
                .LastOrDefaultAsync();
            return response;
        }
    }
}