using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace MedicalAppointments.Client.ViewModels
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