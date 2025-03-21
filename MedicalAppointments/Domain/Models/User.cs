using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointments.Application.Models
{
    public class User : IdentityUser
    {
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [NotMapped]
        public string FullName => $"{Title}. {FirstName?[..1]} {LastName}";
    }
}
