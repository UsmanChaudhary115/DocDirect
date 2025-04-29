using Core.Entities;

namespace HospitalManagementSystem.Models.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> DeletePatientFromDatabase(string patientId);
        List<Patient> GetAllPatientsFromDatabase();
        Task<bool> InsertPatient(Patient patient);
        Task<bool> IsPatientIdPresent(string id);
        List<Patient> SearchPatientsInDatabase(string name);
        Task<bool> UpdatePatientInDatabase(Patient patientToBeUpdated);
    }
}