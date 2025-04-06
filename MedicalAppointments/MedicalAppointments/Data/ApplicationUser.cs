using MedicalAppointments.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointments.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
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
