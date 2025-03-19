using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Domain.Models
{
    public class User : IdentityUser
    {
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
