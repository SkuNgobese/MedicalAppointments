namespace MedicalAppointments.Application.ViewModels
{
    public partial class HospitalViewModel
    {
        public required string HospitalName { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        
        public required ContactViewModel ContactDetails { get; set; }
    }
}
