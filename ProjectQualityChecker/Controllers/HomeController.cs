using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISonarQubeClient _sonarQubeClient;
        private readonly ISonarQubeScanner _sonarQubeScanner;

        public HomeController(ISonarQubeClient sonarQubeClient, ISonarQubeScanner qubeScanner)
        {
            _sonarQubeClient = sonarQubeClient;
            _sonarQubeScanner = qubeScanner;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task Test([FromForm] string name, [FromForm] string url)
        {
            var repo = new Repository
            {
                Name = name,
                Url = url
            };
            await _sonarQubeScanner.ScanRepositoryAsync(repo);
        }
    }
}