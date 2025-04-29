namespace Core.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }  // Navigation property for Appointments

    }
}
