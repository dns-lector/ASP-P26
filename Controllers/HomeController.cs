using System.Diagnostics;
using ASP_P26.Models;
using ASP_P26.Models.Home;
using ASP_P26.Services.Random;
using ASP_P26.Services.Time;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITimeService _timeService;
        private readonly IRandomService _randomService;

        public HomeController(ILogger<HomeController> logger, ITimeService timeService, IRandomService randomService)
        {
            _logger = logger;
            _timeService = timeService;
            _randomService = randomService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Ioc()
        {
            ViewData["timestamp"] = _timeService.Timestamp() + " -- " + _randomService.Otp();
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
