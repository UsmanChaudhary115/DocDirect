using Microsoft.AspNetCore.Mvc;

namespace HMS.web.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
