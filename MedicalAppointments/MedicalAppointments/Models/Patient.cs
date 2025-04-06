using MedicalAppointments.Data;

namespace MedicalAppointments.Models
{
    public class Patient : ApplicationUser
    {
        public string? IDNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? RemoveDate { get; set; }
        public bool IsActive { get; set; }

        public Doctor? PrimaryDoctor { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<DiagnosticFile> DiagnosticFiles { get; set; } = [];
    }
}
