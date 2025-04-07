using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class ContactViewModel
    {
        [DisplayName("Phone Number")]
        [Phone]
        public required string ContactNumber { get; set; }

        [DisplayName("Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [DisplayName("Fax")]
        [Phone]
        public string? Fax { get; set; }
    }
}