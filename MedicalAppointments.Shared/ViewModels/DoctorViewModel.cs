using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class DoctorViewModel : UserViewModel
    {
        [Required, DisplayName("Specialization")]
        public required string Specialization { get; set; }

        [Required, DisplayName("Hire Date")]
        public DateTime? HireDate { get; set; }

        //public IEnumerable<PatientViewModel>? Patients { get; set; } = [];
    }
}