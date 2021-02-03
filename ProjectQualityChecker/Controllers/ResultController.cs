using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services;

namespace ProjectQualityChecker.Controllers
{
    public class ResultController : Controller
    {
        private readonly ResultServices _resultServices;
        private readonly RepositoryService _repositoryService;
        
        public ResultController(ResultServices resultServices, RepositoryService repositoryService)
        {
            _resultServices = resultServices;
            _repositoryService = repositoryService;
        }
        
        // GET
        public async Task<IActionResult> Index()
        {
            ListRepositories response = new ListRepositories();
            response.Repositories = await _repositoryService.GetAllAsync();
            return View(response);
        }

        public IActionResult GetResult([FromQuery] int repositoryId)
        {
            if (repositoryId <= 0) return RedirectToAction("Index");
            
            var commits = _resultServices.Summary(repositoryId);
            return Json(commits);
        }
        
    }
}