using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Client.Pages
{
    public partial class HospitalComponent
    {
        [Inject]
        public IHospital Hospital { get; set; }

        protected IEnumerable<Hospital> hospitals;

        protected override async Task OnInitializedAsync()
        {
            hospitals = await Hospital.GetAllHospitalsAsync();
        }
    }
}
