namespace Core.Entities
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Description { get; set; }
        public string DoctorName { get; set; }
        public string Doctor_Specialization { get; set; }
        public bool? isApproved { get; set; }
    }

}
