using System.Text.Json.Serialization;

namespace MedicalAppointments.Shared.Models
{
    public class Patient : ApplicationUser
    {
        public string? MedicalAidNumber { get; set; }

        public string? PrimaryDoctorId { get; set; }
        public Doctor? PrimaryDoctor { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<DiagnosticFile> DiagnosticFiles { get; set; } = [];
    }
}