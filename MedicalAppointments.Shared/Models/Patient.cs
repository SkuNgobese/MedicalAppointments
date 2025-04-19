using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class Patient : ApplicationUser
    {
        [JsonPropertyName("medicalaidnumber")]
        public string? MedicalAidNumber { get; set; }

        public Doctor? PrimaryDoctor { get; set; }
        public Hospital? Hospital { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<DiagnosticFile> DiagnosticFiles { get; set; } = [];
    }
}