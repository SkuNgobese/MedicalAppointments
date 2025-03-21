using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Application.Models
{
    public class DiagnosticFile
    {
        public DiagnosticFile()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? AttachmentFileName { get; set; }

        public Patient? Patient { get; set; }
    }
}
