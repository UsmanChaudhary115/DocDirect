using System.Diagnostics;
using Core.Entities;
using HMS.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMS.web.Controllers
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
            ViewBag.NumberOfDoc = 5;
            var doctors = new List<Doctor>
            {

                new Doctor { DoctorId = 4, Name = "Dr. Amanda Jepson", Specialization = "Neurosurgeon" },
                new Doctor { DoctorId = 4, Name = "Dr. Amanda Jepson", Specialization = "Neurosurgeon" },
                new Doctor { DoctorId = 4, Name = "Dr. Amanda Jepson", Specialization = "Neurosurgeon" }
            };
            var testimonials = new List<Testimonials>
            {
                new Testimonials { Id = 1, Name = "Ayesha Khan", Testimonial = "The hospital system is extremely user-friendly and has made managing appointments a breeze." },
                new Testimonials { Id = 2, Name = "Ali Raza", Testimonial = "Quick, efficient, and reliable. I can’t imagine going back to manual records again." },
                new Testimonials { Id = 3, Name = "Fatima Sheikh", Testimonial = "An excellent solution for handling patient data and daily operations seamlessly." },
                new Testimonials { Id = 4, Name = "Zainab Malik", Testimonial = "I love how the system sends reminders for upcoming appointments. Very helpful!" },
                new Testimonials { Id = 5, Name = "Hamza Tariq", Testimonial = "We’ve saved so much time and paperwork since switching to this system." },
                new Testimonials { Id = 6, Name = "Usman Javed", Testimonial = "The reporting tools are powerful and provide valuable insights into our operations." },
                new Testimonials { Id = 7, Name = "Mehwish Nadeem", Testimonial = "Impressed by the speed and accuracy of patient record handling." },
                new Testimonials { Id = 8, Name = "Imran Qureshi", Testimonial = "It’s helped us maintain better communication with both staff and patients." },
                new Testimonials { Id = 9, Name = "Rabia Yousaf", Testimonial = "The interface is intuitive and makes training new staff much easier." },
                new Testimonials { Id = 10, Name = "Bilal Ahmed", Testimonial = "Overall a great system for hospital management – we’re very satisfied." }
            };
            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.Doctors = doctors;
            indexViewModel.Testimonials = testimonials; 
            return View(indexViewModel);

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
