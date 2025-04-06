using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointments.Client.Models
{
    public class User : IdentityUser
    {
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Contact? Contact { get; set; }
        public Address? Address { get; set; }
        public Hospital? Hospital { get; set; }

        [NotMapped]
        public string FullName => $"{Title}. {FirstName?[..1]} {LastName}";
    }
}
