using System.Collections.Generic;
using System.Linq;
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

        public CommitSummaryList GetCommitSummaries(int repositoryId)
        {
            var response = new CommitSummaryList();

            response.CommitList = _context.Commits
                .Include(dev => dev.Developer)
                .Include(branch => branch.Branch)
                .Where(r => r.Repository.RepositoryId == repositoryId)
                .Select(c => new CommitSummary
                {
                    Developer = c.Developer,
                    Date = c.Date,
                    Message = c.Message,
                    Sha = c.Sha,
                    CommitId = c.CommitId
                }).ToList();

            return response;
        }
    }
}