namespace MedicalAppointments.Application.ViewModels
{
    public class PatientViewModel
    {
        public required string Title { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string IDNumber { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        public required ContactViewModel ContactDetails { get; set; }
        public required DiagnosticFileViewModel DiagnosticDetails { get; set; }
    }
}
