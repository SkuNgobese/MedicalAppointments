using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedicalAppointments.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("firstname")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string? LastName { get; set; }

        public Contact? Contact { get; set; }
        public Address? Address { get; set; }
        
        [NotMapped]
        [JsonPropertyName("fullname")]
        public string FullName => $"{Title}. {FirstName?[..1]} {LastName}";
    }
}