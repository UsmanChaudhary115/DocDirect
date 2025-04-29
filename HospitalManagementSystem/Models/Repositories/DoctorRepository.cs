using Core.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.Data;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalDbContext _context;

        public DoctorRepository(HospitalDbContext context)
        {
            _context = context;
        }

        private void LogDeletedDoctor(Doctor doctor)
        {
            StreamWriter deletedWriter = null;
            try
            {
                deletedWriter = new StreamWriter("DeletedDoctors.txt", append: true);
                string deletedDoctor = $"{JsonSerializer.Serialize(doctor)} Time of Deletion: {DateTime.Now}";
                deletedWriter.WriteLine(deletedDoctor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while logging deleted doctor: {ex.Message}");
            }
            finally
            {
                deletedWriter?.Close();
            }
        }

        public bool InsertDoctor(Doctor doctor)
        {
            bool status = false;

            try
            {
                _context.Doctors.Add(doctor);
                int count = _context.SaveChanges();
                status = count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return status;
        }

        public List<Doctor> GetAllDoctorsFromDatabase()
        {
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                doctors = _context.Doctors.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return doctors;
        }

        public bool UpdateDoctorInDatabase(Doctor doctorToBeUpdated)
        {
            bool status = false;

            try
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == doctorToBeUpdated.DoctorId);

                if (doctor != null)
                {
                    doctor.Name = doctorToBeUpdated.Name;
                    doctor.Specialization = doctorToBeUpdated.Specialization;

                    _context.Doctors.Update(doctor);
                    int count = _context.SaveChanges();

                    status = count > 0;
                }
                else
                {
                    Console.WriteLine("Doctor not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return status;
        }

        public bool DeleteDoctorFromDatabase(int doctorId)
        {
            bool status = false;

            try
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);

                if (doctor != null)
                {
                    LogDeletedDoctor(doctor);
                    _context.Doctors.Remove(doctor);
                    int count = _context.SaveChanges();
                    status = count > 0;
                }
                else
                {
                    Console.WriteLine("Doctor not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return status;
        }

        public List<Doctor> SearchDoctorsInDatabase(string specialization)
        {
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                doctors = _context.Doctors
                    .Where(d => d.Specialization.Contains(specialization))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return doctors;
        }

        public bool isDoctorIdPresent(int id)
        {
            bool exists = false;

            try
            {
                exists = _context.Doctors.Any(d => d.DoctorId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return exists;
        }
    }
}
