namespace MedicalAppointments.ViewModels
{
    public partial class PatientViewModel : UserViewModel
    {
        public required DiagnosticFileViewModel DiagnosticDetails { get; set; }
    }
}