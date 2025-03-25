using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Application.ViewModels
{
    public partial class ContactViewModel
    {
        [Phone]
        public required string PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Fax { get; set; }
    }
}
