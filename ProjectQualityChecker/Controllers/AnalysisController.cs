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

        public AnalysisController(IRepositoryService repositoryService, ISonarQubeScanner sonarQubeScanner)
        {
            _repositoryService = repositoryService;
            _sonarQubeScanner = sonarQubeScanner;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("Analysis")]
        public async Task<IActionResult> Analysis(RepositoryForm repositoryForm)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Repository repository = _repositoryService.Create(repositoryForm);

            try
            {
                await _sonarQubeScanner.ScanRepositoryAsync(repository);
            }
            catch (ApplicationException applicationException)
            {
                return BadRequest(applicationException.Message);
            }

            return Ok();
        }
    }
}