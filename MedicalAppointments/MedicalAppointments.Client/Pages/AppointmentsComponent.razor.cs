using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Client.Pages
{
    public partial class AppointmentsComponent
    {
        [Inject]
        public IAppointment? Appointment { get; set; }

        protected List<Appointment>? appointments;

        protected override async Task OnInitializedAsync()
        {
            appointments = await Appointment.GetAppointmentsAsync();
        }
    }
}
