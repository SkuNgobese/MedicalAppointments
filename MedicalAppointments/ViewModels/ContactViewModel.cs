using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.ViewModels
{
    public partial class ContactViewModel
    {
        [Required, DisplayName("Phone Number")]
        [Phone]
        public required string ContactNumber { get; set; }

        [Required, DisplayName("Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [DisplayName("Fax")]
        [Phone]
        public string? Fax { get; set; }
    }
}