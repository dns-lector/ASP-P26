using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    public class StorageController : Controller
    {
        [HttpGet]
        public IActionResult Index(String id)
        {
            String path = "C:/storage/ASP26/" + id;
            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "image/jpeg");
            }
            return NotFound();
        }
    }
}
