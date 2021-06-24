using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class AnalysisController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ISonarQubeScanner _sonarQubeScanner;
        private readonly IBranchService _branchService;

        public AnalysisController(IRepositoryService repositoryService,
            ISonarQubeScanner sonarQubeScanner,
            IBranchService branchService)
        {
            _repositoryService = repositoryService;
            _sonarQubeScanner = sonarQubeScanner;
            _branchService = branchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Analysis")]
        public async Task<IActionResult> Analysis(RepositoryForm repositoryForm)
        {
            if (!ModelState.IsValid) return BadRequest();

            Repository repository = _repositoryService.Create(repositoryForm);

            try
            {
                await _sonarQubeScanner.ScanRepositoryAsync(repository, repositoryForm.Branch);
            }
            catch (ApplicationException applicationException)
            {
                return BadRequest(applicationException.Message);
            }

            return Ok();
        }

        [HttpPost("AnalysisUpdate")]
        public async Task<IActionResult> AnalysisUpdate(StoredRepository storedRepository)
        {
            if (!ModelState.IsValid) return BadRequest();

            Repository repository = _repositoryService.GetById(storedRepository.RepositoryId);
            Branch branch = null;
            if (storedRepository.BranchId != null) branch = _branchService.GetById((int)storedRepository.BranchId);
            else if (storedRepository.Branch != String.Empty)
            {
                branch = _branchService.GetByName(storedRepository.Branch);
            }

            if (branch == null)
            {
                branch = _branchService.CreateBranch(storedRepository.Branch);
            }


            try
            {
                await _sonarQubeScanner.ScanRepositorySinceLastScannedCommit(repository, branch);
            }
            catch (ApplicationException applicationException)
            {
                return BadRequest(applicationException.Message);
            }

            return Ok();
        }
    }
}