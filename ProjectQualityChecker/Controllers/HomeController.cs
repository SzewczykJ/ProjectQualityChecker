using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services;

namespace ProjectQualityChecker.Controllers
{
    public class HomeController : Controller
    {
      
        private readonly SonarQubeClient _sonarQubeClient;
        private SonarQubeScanner _sonarQubeScanner;

        public HomeController(ILogger<HomeController> logger, SonarQubeClient sonarQubeClient, SonarQubeScanner qubeScanner)
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Test([FromForm] string name,[FromForm] string url)
        {
            var repo = new Repository()
            {Name = name,
                Url = url
                
            };
            await _sonarQubeScanner.ScanRepositoryAsync(repo);
        }
    }
}