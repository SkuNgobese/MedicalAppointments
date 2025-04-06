namespace MedicalAppointments.Client.ViewModels
{
    public partial class PatientViewModel : UserViewModel
    {
        public required DiagnosticFileViewModel DiagnosticDetails { get; set; }
    }
}