using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("firstname")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string? LastName { get; set; }

        [JsonPropertyName("idnumber")]
        public string? IDNumber { get; set; }

        [JsonPropertyName("createdate")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("removedate")]
        public DateTime? RemoveDate { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }

        [NotMapped]
        [JsonPropertyName("fullname")]
        public string FullName
        {
            get
            {
                var initial = !string.IsNullOrEmpty(FirstName) ? FirstName.Substring(0, 1) : "";
                return $"{Title}. {initial} {LastName ?? ""}".Trim();
            }
        }

        public Contact? Contact { get; set; }
        public Address? Address { get; set; }
    }
}