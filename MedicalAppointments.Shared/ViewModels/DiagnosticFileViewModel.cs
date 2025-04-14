using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class DiagnosticFileViewModel
    {
        public Guid? Id { get; set; }

        [DisplayName("Diagnosis")]
        public required string Diagnosis { get; set; }

        [DisplayName("Treatment")]
        public string? Treatment { get; set; }

        [DisplayName("Attachment File")]
        public IFormFile? AttachmentFileName { get; set; }
    }
}