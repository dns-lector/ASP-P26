using System.Diagnostics;
using ASP_P26.Models;
using ASP_P26.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Razor()
        {
            HomeRazorPageModel model = new()
            {
                Arr = ["Item 1", "Item 2", "Item 3", "Item 4", "Item 5"]
            };
            return View(model);
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
    }
}
