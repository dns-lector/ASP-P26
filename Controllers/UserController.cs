using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    public class UserController : Controller
    {
        public IActionResult SignUp()
        { 
            if(Request.Method == "POST")
            {
                return RedirectToAction(nameof(SignUp));
            }
            return View();
        }
    }
}
