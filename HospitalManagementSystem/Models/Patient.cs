using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Patient : IdentityUser
    {
        public string Name { get; set; }
        public string? Disease { get; set; }

        // Navigation property for appointments
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
