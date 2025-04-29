using Infrastructure.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Infrastructure.Data;
using HospitalManagementSystem.Models.Interfaces;

namespace Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HospitalDbContext _context;
        private readonly IPatientRepository PatientRepository;
        private readonly IDoctorRepository DoctorServices;

        public AppointmentRepository(HospitalDbContext context, IPatientRepository patientRepository, IDoctorRepository doctorServices)
        {
            _context = context;
            PatientRepository = patientRepository;
            DoctorServices = doctorServices;
        }

        private void LogDeletedAppointment(Appointment appointment)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("DeletedAppointments.txt", append: true))
                {
                    string deletedAppointment = $"{JsonSerializer.Serialize(appointment)} Time of Deletion: {DateTime.Now}";
                    writer.WriteLine(deletedAppointment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging deleted appointment: {ex.Message}");
            }
        }

        public bool isPatientIdPresent(string PatientId)
        {
            return PatientRepository.GetAllPatientsFromDatabase().Any(p => p.Id == PatientId);
        }

        public bool isDoctorAvailable(int id, DateTime date)
        {
            return !_context.Appointments.Any(a => a.DoctorId == id && a.AppointmentDate.Date == date.Date);
        }

        public bool InsertAppointment(Appointment appointment)
        {
            try
            {
                if (!isDoctorAvailable(appointment.DoctorId, appointment.AppointmentDate))
                {
                    Console.WriteLine($"Doctor with ID {appointment.DoctorId} is not available at {appointment.AppointmentDate:yyyy-MM-dd}");
                    return false;
                }
                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting appointment: {ex.Message}");
                return false;
            }
        }


        public List<Appointment> GetAllAppointmentsFromDatabase()
        {
            try
            {
                return _context.Appointments.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving appointments: {ex.Message}");
                return new List<Appointment>();
            }
        }

        public bool DeleteAppointmentFromDatabase(int appointmentId)
        {
            try
            {
                var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
                if (appointment == null) return false;

                LogDeletedAppointment(appointment);

                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting appointment: {ex.Message}");
                return false;
            }
        }

        public List<Appointment> SearchAppointmentsInDatabase(int doctorId, string patientId)
        {
            try
            {
                return _context.Appointments
                    .Where(a => a.DoctorId == doctorId && a.PatientId == patientId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching appointments: {ex.Message}");
                return new List<Appointment>();
            }
        }

        public bool isAppointmentIdPresent(int id)
        {
            return _context.Appointments.Any(a => a.AppointmentId == id);
        }
    }
}
