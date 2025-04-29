    using Core.Entities;
    namespace Infrastructure.Interfaces
    {
        public interface IAppointmentRepository
        {
            bool DeleteAppointmentFromDatabase(int appointmentId);
            List<Appointment> GetAllAppointmentsFromDatabase();
            bool InsertAppointment(Appointment appointment);
            bool isAppointmentIdPresent(int id);
            bool isDoctorAvailable(int id, DateTime date);
            bool isPatientIdPresent(string id);
            List<Appointment> SearchAppointmentsInDatabase(int doctorId, string patientId);
        }
    }