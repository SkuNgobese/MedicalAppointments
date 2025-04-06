using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Models;
using System.IO;
using System.Net.Http.Json;

namespace MedicalAppointments.Client.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly HttpClient _http;
        private const string _directory = "/appointments";

        public AppointmentService(HttpClient http) => _http = http;

        public async Task<List<Appointment>> GetAppointmentsAsync() =>
            await _http.GetFromJsonAsync<List<Appointment>>(_http.BaseAddress?.ToString() + _directory);

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(Hospital hospital) =>
            await _http.GetFromJsonAsync<List<Appointment>>($"{_http.BaseAddress?.ToString() + _directory}/{hospital.Id}");

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
        await _http.GetFromJsonAsync<Appointment>($"{_http.BaseAddress?.ToString() + _directory}/{id}");

        public async Task BookAppointmentAsync(Appointment appointment)
        {
            var response = await _http.PostAsJsonAsync(_http.BaseAddress?.ToString() + _directory, appointment);
            response.EnsureSuccessStatusCode();
        }

        public async Task ReAssignAppointmentAsync(Appointment appointment)
        {
            var response = await _http.PutAsJsonAsync($"{_http.BaseAddress?.ToString() + _directory}/{appointment.Id}/reassign", appointment);
            response.EnsureSuccessStatusCode();
        }

        public async Task CancelAppointmentAsync(Appointment appointment)
        {
            var response = await _http.PutAsJsonAsync($"{_http.BaseAddress?.ToString() + _directory}/{appointment.Id}/cancel", appointment);
            response.EnsureSuccessStatusCode();
        }
    }
}
