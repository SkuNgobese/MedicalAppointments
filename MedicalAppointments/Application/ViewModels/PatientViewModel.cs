using System.ComponentModel;

namespace MedicalAppointments.Application.ViewModels
{
    public partial class PatientViewModel : UserViewModel
    {
        public required DiagnosticFileViewModel DiagnosticDetails { get; set; }
    }
}
