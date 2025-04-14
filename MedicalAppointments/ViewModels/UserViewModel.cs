using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.ViewModels
{
    public partial class UserViewModel
    {
        [Required, DisplayName("Title")]
        public required string Title { get; set; }

        [Required, DisplayName("First Name")]
        public required string FirstName { get; set; }

        [Required, DisplayName("Last Name")]
        public required string LastName { get; set; }

        [Required, DisplayName("ID Number")]
        public required string IDNumber { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        public required ContactViewModel ContactDetails { get; set; }
    }
}