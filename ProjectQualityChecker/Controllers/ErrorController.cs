using Microsoft.AspNetCore.Mvc;

namespace ProjectQualityChecker.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{StatusCode}")]
        public IActionResult StatusCodeHandle(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("404");
                default:
                    return View("Error");
            }
        }
    }
}