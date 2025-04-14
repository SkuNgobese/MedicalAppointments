using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class AddressViewModel
    {
        public int? Id { get; set; }

        [Required, DisplayName("Street")]
        [StringLength(100, ErrorMessage = "Street name cannot exceed 100 characters")]
        public required string Street { get; set; }

        [Required, DisplayName("City")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City name can only contain letters")]
        public required string City { get; set; }

        [Required, DisplayName("Suburb")]
        [StringLength(50, ErrorMessage = "Suburb name cannot exceed 50 characters")]
        public required string Suburb { get; set; }

        [Required, DisplayName("Postal Code")]
        [RegularExpression(@"^\d{4,10}$", ErrorMessage = "Postal Code must be between 4 to 10 digits")]
        public required string PostalCode { get; set; }

        [Required, DisplayName("Country")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Country name can only contain letters")]
        public required string Country { get; set; }
    }
}