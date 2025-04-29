namespace Core.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Description { get; set; }
        public virtual Patient Patient { get; set; }  // Navigation property for Patient
        public virtual Doctor Doctor { get; set; }    // Navigation property for Doctor
    }
}
