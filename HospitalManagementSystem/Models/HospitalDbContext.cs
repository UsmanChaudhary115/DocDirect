using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class HospitalDbContext : IdentityDbContext<Patient>
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

        // DbSets for your custom entities
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        // SaveChanges override (optional but usually not required unless custom logic is needed)
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        // Entity configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // IMPORTANT: Always call this first for Identity

            // Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional: configure Doctor entity if needed
        }
    }
}
