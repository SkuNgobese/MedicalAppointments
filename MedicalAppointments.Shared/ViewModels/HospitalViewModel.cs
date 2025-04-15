using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class HospitalViewModel
    {
        public int? Id { get; set; }

        [Required, DisplayName("Hospital Name")]
        public required string HospitalName { get; set; }

        public AddressViewModel? AddressDetails { get; set; }
        
        public ContactViewModel? ContactDetails { get; set; }
    }
}