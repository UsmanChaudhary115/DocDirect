using Infrastructure.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;
using Infrastructure.Data;
using HospitalManagementSystem.Models.Interfaces;

namespace Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HospitalDbContext _context;
        private readonly UserManager<Patient> _userManager;

        // Constructor to inject DbContext and UserManager
        public PatientRepository(HospitalDbContext context, UserManager<Patient> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private void LogDeletedPatient(Patient patient)
        {
            StreamWriter deletedWriter = null;
            try
            {
                deletedWriter = new StreamWriter("DeletedPatients.txt", append: true);
                string deletedPatient = $"{JsonSerializer.Serialize(patient)} Time of Deletion: {DateTime.Now}";
                deletedWriter.WriteLine(deletedPatient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while logging deleted patient: {ex.Message}");
            }
            finally
            {
                if (deletedWriter != null)
                {
                    deletedWriter.Close();
                }
            }
        }

        public async Task<bool> InsertPatient(Patient patient)
        {
            bool status = false;

            try
            {
                var result = await _userManager.CreateAsync(patient, "Password123!"); // Provide a default password or customize as needed
                if (result.Succeeded)
                {
                    status = true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return status;
        }

        public List<Patient> SearchPatientsInDatabase(string name)
        {
            List<Patient> patients = new List<Patient>();

            try
            {
                patients = _context.Users
                    .Where(p => p.Name == name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return patients;
        }

        public async Task<bool> DeletePatientFromDatabase(string patientId)
        {
            bool status = false;

            try
            {
                var patient = await _userManager.FindByIdAsync(patientId);
                if (patient != null)
                {
                    LogDeletedPatient(patient);
                    var result = await _userManager.DeleteAsync(patient);
                    if (result.Succeeded)
                    {
                        status = true;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return status;
        }

        public async Task<bool> UpdatePatientInDatabase(Patient patientToBeUpdated)
        {
            bool status = false;

            try
            {
                var patient = await _userManager.FindByIdAsync(patientToBeUpdated.Id);
                if (patient != null)
                {
                    patient.Name = patientToBeUpdated.Name;
                    patient.Email = patientToBeUpdated.Email;
                    patient.Disease = patientToBeUpdated.Disease;

                    var result = await _userManager.UpdateAsync(patient);
                    if (result.Succeeded)
                    {
                        status = true;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return status;
        }

        public List<Patient> GetAllPatientsFromDatabase()
        {
            List<Patient> patients = new List<Patient>();

            try
            {
                patients = _context.Users.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return patients;
        }

        public async Task<bool> IsPatientIdPresent(string id)
        {
            bool exists = false;

            try
            {
                var patient = await _userManager.FindByIdAsync(id);
                exists = patient != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return exists;
        }
    }
}
