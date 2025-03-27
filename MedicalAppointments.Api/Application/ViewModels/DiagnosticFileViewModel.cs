using System.ComponentModel;

namespace MedicalAppointments.Api.Application.ViewModels
{
    public partial class DiagnosticFileViewModel
    {
        [DisplayName("Diagnosis")]
        public required string Diagnosis { get; set; }

        [DisplayName("Treatment")]
        public string? Treatment { get; set; }

        [DisplayName("Attachment File")]
        public IFormFile? AttachmentFileName { get; set; }
    }
}