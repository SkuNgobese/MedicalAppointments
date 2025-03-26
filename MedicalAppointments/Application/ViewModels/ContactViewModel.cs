using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Application.ViewModels
{
    public partial class ContactViewModel
    {
        [DisplayName("Phone Number")]
        [Phone]
        public required string PhoneNumber { get; set; }

        [DisplayName("Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [DisplayName("Fax")]
        [Phone]
        public string? Fax { get; set; }
    }
}
