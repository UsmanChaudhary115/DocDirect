using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Infrastructure.Interfaces;
using Core.Entities; // For Identity user (Patient)
using Microsoft.AspNetCore.Identity;
using HospitalManagementSystem.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Add Identity services
builder.Services.AddIdentity<Patient, IdentityRole>()
    .AddEntityFrameworkStores<HospitalDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Patient/Signin"; // Change if your Signin view is elsewhere
});

// Optional Identity configuration (e.g., password rules)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
});

// Register repositories
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
