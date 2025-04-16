namespace MedicalAppointments.Shared.ViewModels
{
    public partial class PatientViewModel : UserViewModel
    {
        public string? PrimaryDoctorId { get; set; }

        public DiagnosticFileViewModel? DiagnosticDetails { get; set; }
    }
}