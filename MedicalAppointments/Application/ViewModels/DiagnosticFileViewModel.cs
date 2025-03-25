namespace MedicalAppointments.Application.ViewModels
{
    public partial class DiagnosticFileViewModel
    {
        public required string Diagnosis { get; set; }
        public required string Treatment { get; set; }
        public required string AttachmentFileName { get; set; }
    }
}
