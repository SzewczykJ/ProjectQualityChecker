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


            // TODO: validate and clear text from special chart // remove '/tree/' from link 
            var repo = new Repository {Name = repositoryForm.Name, Url = repositoryForm.Url};
            if (_repositoryService.Create(repo) > 0)
                try
                {
                    await _sonarQubeScanner.ScanRepositoryAsync(repo);
                }
                catch (ApplicationException applicationException)
                {
                    return BadRequest(applicationException.Message);
                }

            return Ok();
        }
    }
}