using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IRepositoryService _repositoryService;

        public RepositoryController(IRepositoryService repositoryService, IBranchService branchService)
        {
            _repositoryService = repositoryService;
            _branchService = branchService;
        }

        public async Task<ListRepositories> Index()
        {
            var response = new ListRepositories();
            response.Repositories = await _repositoryService.GetAllAsync();

            return response;
        }

        [HttpGet()]
        public async Task<IActionResult> GetRepositoryBranches([FromQuery] long? repositoryId = null)
        {
            if (!repositoryId.HasValue)
                return BadRequest("No data");

            List<Branch> Branches = await _branchService.GetAllByRepositoryId((int) repositoryId);
            return Ok(Branches);
        }
    }
}