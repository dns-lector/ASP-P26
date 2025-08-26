using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

    }
}
