using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class Patient : ApplicationUser
    {
        [JsonPropertyName("idnumber")]
        public string? IDNumber { get; set; }

        [JsonPropertyName("createdate")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("removedate")]
        public DateTime? RemoveDate { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }

        public Doctor? PrimaryDoctor { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<DiagnosticFile> DiagnosticFiles { get; set; } = [];
    }
}