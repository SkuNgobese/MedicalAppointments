using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.ViewModels
{
    public partial class DoctorViewModel : UserViewModel
    {
        [Required, DisplayName("Specialization")]
        public required string Specialization { get; set; }

        [Required, DisplayName("Hire Date")]
        public required DateTime HireDate { get; set; }
    }
}