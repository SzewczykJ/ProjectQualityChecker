using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
    }
}