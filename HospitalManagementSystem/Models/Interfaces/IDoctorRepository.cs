using Core.Entities;
namespace Infrastructure.Interfaces
{
    public interface IDoctorRepository
    {
        bool DeleteDoctorFromDatabase(int doctorId);
        List<Doctor> GetAllDoctorsFromDatabase();
        bool InsertDoctor(Doctor doctor);
        bool isDoctorIdPresent(int id);
        List<Doctor> SearchDoctorsInDatabase(string specialization);
        bool UpdateDoctorInDatabase(Doctor doctorToBeUpdated);
    }
}