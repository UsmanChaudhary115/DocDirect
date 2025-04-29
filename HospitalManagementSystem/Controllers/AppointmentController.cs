using Microsoft.AspNetCore.Mvc;

namespace HMS.web.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
