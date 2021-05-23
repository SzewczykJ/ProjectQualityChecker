using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Controllers
{
    public class ResultController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IResultService _resultService;
        private readonly IBranchService _branchService;

        public ResultController(IResultService resultServices,
            IRepositoryService repositoryService,
            IBranchService branchService)
        {
            _resultService = resultServices;
            _repositoryService = repositoryService;
            _branchService = branchService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var result = await new RepositoryController(_repositoryService, _branchService).Index();
            return View(result);
        }

        [HttpPost()]
        public IActionResult GetResult(ResultsFilter resultsFilter)
        {
            if (!ModelState.IsValid)
                return BadRequest(resultsFilter.BranchId);

            if (resultsFilter.RepositoryId <= 0) return RedirectToAction("Index");


            Repository repository = _repositoryService.GetById(resultsFilter.RepositoryId);
            Branch branch = _branchService.GetById(resultsFilter.BranchId);

            ResultsResponse response = new ResultsResponse();
            response.RespositoryInfo = new RepositoryInfo()
            {
                Name = repository.Name,
                RepositoryId = repository.RepositoryId,
                Url = repository.Url,
                Branch = branch.Name,
                BranchId = branch.BranchId
            };
            response.CommitSummary = _resultService.Summary(resultsFilter);

            return Ok(response);
        }
    }
}