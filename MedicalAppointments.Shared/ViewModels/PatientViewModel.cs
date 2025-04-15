namespace MedicalAppointments.Shared.ViewModels
{
    public partial class PatientViewModel : UserViewModel
    {
        public HospitalViewModel? HospitalDetails { get; set; }
        public DoctorViewModel? DoctorDetails { get; set; }
        public IEnumerable<AppointmentViewModel>? AppointmentDetails { get; set; }
        public DiagnosticFileViewModel? DiagnosticDetails { get; set; }
    }
}