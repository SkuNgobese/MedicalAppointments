using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Client.Pages
{
    public partial class DoctorComponent
    {
        [Inject]
        public IDoctor Doctor { get; set; }

        protected IEnumerable<Doctor> doctors;

        protected override async Task OnInitializedAsync()
        {
            doctors = await Doctor.GetAllDoctorsAsync();
        }
    }
}
