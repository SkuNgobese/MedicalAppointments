using MedicalAppointments.Interfaces;
using MedicalAppointments.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Components
{
    public partial class DoctorComponent
    {
        [Inject]
        public IDoctor? Doctor { get; set; }

        protected IEnumerable<Doctor>? doctors;

        protected override async Task OnInitializedAsync()
        {
            if (Doctor != null)
                doctors = await Doctor.GetAllDoctorsAsync();
            else
                doctors = [];
        }
    }
}
