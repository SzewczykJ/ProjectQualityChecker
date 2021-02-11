using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Controllers
{
    public class ResultController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IResultServices _resultServices;

        public ResultController(IResultServices resultServices, IRepositoryService repositoryService)
        {
            _resultServices = resultServices;
            _repositoryService = repositoryService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var response = new ListRepositories();
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