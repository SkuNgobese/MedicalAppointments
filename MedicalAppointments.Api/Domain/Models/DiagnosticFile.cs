using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Api.Domain.Models
{
    public class DiagnosticFile
    {
        public DiagnosticFile()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public required string Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? AttachmentFileName { get; set; }

        public required Patient Patient { get; set; }
    }
}
