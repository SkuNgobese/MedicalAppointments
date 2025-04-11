using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Client.Pages
{
    public partial class HospitalComponent
    {
        [Inject]
        public IHospital? Hospital { get; set; }

        protected IEnumerable<Hospital>? hospitals;

        protected override async Task OnInitializedAsync()
        {
            if (Hospital != null)
                hospitals = await Hospital.GetAllHospitalsAsync();
            else
                hospitals = [];
        }
    }
}