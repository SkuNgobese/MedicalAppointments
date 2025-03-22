using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Application.ViewModels
{
    public class ContactViewModel
    {
        [Phone]
        public required string PhoneNumber { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public string? Fax { get; set; }
    }
}
