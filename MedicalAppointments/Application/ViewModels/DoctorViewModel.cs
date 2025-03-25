namespace MedicalAppointments.Application.ViewModels
{
    public partial class DoctorViewModel
    {
        public required string IDNumber { get; set; }
        public required string Specialization { get; set; }
        public required DateTime HireDate { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        public required ContactViewModel ContactDetails { get; set; }
    }
}
