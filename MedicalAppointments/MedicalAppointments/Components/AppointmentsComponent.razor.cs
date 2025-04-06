using MedicalAppointments.Interfaces;
using MedicalAppointments.Models;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Components
{
    public partial class AppointmentsComponent
    {
        [Inject]
        public IAppointment? Appointment { get; set; }

        protected List<Appointment>? appointments;

        protected override async Task OnInitializedAsync()
        {
            if (Appointment != null)
                appointments = await Appointment.GetAppointmentsAsync();
            else
                appointments = [];
        }
    }
}
