using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.ViewModels
{
    public partial class HospitalViewModel
    {
        [Required, DisplayName("Hospital Name")]
        public required string HospitalName { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        
        public required ContactViewModel ContactDetails { get; set; }
    }
}