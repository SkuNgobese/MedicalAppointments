using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class UserViewModel
    {
        public string? Id { get; set; }

        [Required, DisplayName("Title")]
        public required string Title { get; set; }

        [Required, DisplayName("First Name")]
        public required string FirstName { get; set; }

        [Required, DisplayName("Last Name")]
        public required string LastName { get; set; }

        [Required, DisplayName("ID Number")]
        public string? IDNumber { get; set; }

        public string FullName => $"{Title}. {FirstName?[..1]} {LastName}";

        public AddressViewModel? AddressDetails { get; set; }
        public ContactViewModel? ContactDetails { get; set; }
    }
}