using Microsoft.AspNetCore.Mvc;

namespace HMS.web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
