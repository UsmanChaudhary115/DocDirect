﻿using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace HMS.web.Controllers
{
    public class PatientController : Controller
    {
        private readonly UserManager<Patient> _userManager;
        private readonly SignInManager<Patient> _signInManager;
        private readonly HospitalDbContext _context;

        public PatientController(UserManager<Patient> userManager, SignInManager<Patient> signInManager, HospitalDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {

            TempData["WelcomeMessage"] = $"Welcome, {HttpContext.Request.Cookies["PatientName"]}!";

            var doctors = _context.Doctors.ToList();

            ViewBag.PatientID = HttpContext.Request.Cookies["PatientID"];
            // Assuming 'user' is the logged-in user or a specific user object.
            

            return View(doctors);
        }
        public IActionResult SearchDoctors(string query)
        {
            var doctors = new List<Doctor>
    {
        new Doctor { DoctorId = 1, Name = "Dr. Amanda Jepson", Specialization = "Neurosurgeon" },
        new Doctor{ DoctorId = 2, Name = "Dr. Taylor", Specialization = "Cardiologist" },
        new Doctor{ DoctorId = 3, Name = "Dr. Archer", Specialization = "Ophthalmologist" },
        new Doctor { DoctorId = 4, Name = "Dr. Penny", Specialization = "Psychiatrist" },
        new Doctor { DoctorId = 5, Name = "Dr. Riley", Specialization = "Gynecologist" }
    };

            if (!string.IsNullOrWhiteSpace(query))
            {
                doctors = doctors
                    .Where(d => d.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                             || d.Specialization.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return PartialView("_DoctorCardList", doctors);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> MakeAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data submitted.";
                return RedirectToAction("Index");
            }

            // Fetch patient by ID (assuming PatientId is the Identity User ID)
            var patient = await _context.Users.OfType<Patient>().FirstOrDefaultAsync(p => p.Id == appointment.PatientId);

            // Fetch doctor by ID
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == appointment.DoctorId);

            if (patient == null || doctor == null)
            {
                TempData["ErrorMessage"] = "Patient or Doctor not found.";
                return RedirectToAction("Index");
            }

            // Create new appointment and associate patient and doctor
            var newAppointment = new Appointment
            {
                AppointmentDate = appointment.AppointmentDate,
                Description = appointment.Description,
                isApproved = false,
                Patient = patient,
                Doctor = doctor
            };

            // Add to context
            _context.Appointments.Add(newAppointment);

            // Optional: update navigation collections (helps in memory but not required for EF)
            patient.Appointments ??= new List<Appointment>();
            doctor.Appointments ??= new List<Appointment>();
            patient.Appointments.Add(newAppointment);
            doctor.Appointments.Add(newAppointment);

            await _context.SaveChangesAsync();

            TempData["Message"] = "Appointment successfully created!";
            return RedirectToAction("Index");
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Signin");

            //var appointments = _context.Appointments
            //    .Where(a => a.PatientId == user.Id)
            //    .Include(a => a.Doctor)
            //    .ToList();
            var json = Request.Cookies["PatientAppointments"];
            var appointments = JsonSerializer.Deserialize<List<AppointmentDTO>>(json);


            var model = new ProfileViewModel
            {
                Disease = user.Disease,
                Name = user.Name,
                Email = user.Email,
                Appointments = appointments
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (model.NewPassword != null && !ModelState.IsValid || (model.NewPassword != model.ConfirmPassword))
                return View("Profile", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Signin");

            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Disease = model.Disease;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View("Profile", model);
            }

            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var passResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!passResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update password.");
                    return View("Profile", model);
                }
            }
            TempData["Message"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(patient, model.Password);
                if (result.Succeeded)
                { 
                    return RedirectToAction("Signin", "Patient");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public IActionResult Signin()
        {
            TempData["Message"] = "Please enter your credentials to access your account.";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        HttpContext.Response.Cookies.Append("PatientName", user.Name);
                        HttpContext.Response.Cookies.Append("PatientID", user.Id);
                        HttpContext.Response.Cookies.Append("PatientEmail", user.Email);

                        var user1 = _context.Users.OfType<Patient>()
                        .Include(p => p.Appointments)  // This ensures the Appointments are loaded
                        .FirstOrDefault(u => u.Id == user.Id);

                        var appointments = user1.Appointments ?? new List<Appointment>();

                        var appointmentsList = appointments.Select(a => new AppointmentDTO
                        {
                            AppointmentId = a.AppointmentId,
                            AppointmentDate = a.AppointmentDate,
                            Description = a.Description,
                            isApproved = a.isApproved,
                            DoctorName = a.Doctor?.Name ?? "Unknown",
                            Doctor_Specialization = a.Doctor?.Specialization ?? "Unknown"
                        }).ToList();



                        HttpContext.Response.Cookies.Append("PatientAppointments", JsonSerializer.Serialize(appointmentsList));


                    }
                    else
                    {
                        Console.WriteLine("User not found.");
                    }
                    return RedirectToAction("Index", "Patient");
                }
                TempData["Message"] = "Email or password is incorrect.";
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
        [Authorize]
        public IActionResult SignOutConfirmation()
        {
            return View();
        }
        public async Task<IActionResult> SignOut()
        {
            // Sign out the user
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
