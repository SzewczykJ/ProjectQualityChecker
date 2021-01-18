using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services;

namespace ProjectQualityChecker.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class AnalysisController : Controller
    {
        private readonly RepositoryService _repositoryService;
        private readonly SonarQubeScanner _sonarQubeScanner;

        public AnalysisController(RepositoryService repositoryService,SonarQubeScanner sonarQubeScanner)
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
           if(!ModelState.IsValid) 
               return BadRequest();

           var repo = new Repository{ Name = repositoryForm.Name, Url = repositoryForm.Url };
           if (_repositoryService.Create(repo) > 0)
           {
               await _sonarQubeScanner.ScanRepositoryAsync(repo);
           }
           return Ok();
        }
    }
}