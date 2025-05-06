using System.ComponentModel.DataAnnotations;
namespace Core.Entities
{
    public class ProfileViewModel
    {
        [Required]
        public string Name { get; set; }
    
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    
        public string? Disease { get; set; }
    
        public List<AppointmentDTO> Appointments { get; set; }
    }
}

