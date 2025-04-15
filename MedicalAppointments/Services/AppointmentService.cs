using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using System.Net;
using System.Net.Http.Json;

namespace MedicalAppointments.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly HttpClient _http;
        private const string _endPoint = "api/Appointments";

        public AppointmentService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");


        public async Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<AppointmentViewModel>>($"{_endPoint}") ?? [];
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Error getting doctors: {ex.Message}");
                throw;
            }
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) =>
        await _http.GetFromJsonAsync<Appointment>($"{_endPoint}/{id}");

        public async Task<Appointment> BookAppointmentAsync(Appointment appointment)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(_endPoint, appointment);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Appointment>() ?? null!;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null!;
            }
        }

        public async Task RescheduleAppointmentAsync(Appointment appointment)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{appointment.Id}/reschedule/", appointment);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw;
            }
        }

        public async Task ReAssignAppointmentAsync(Appointment appointment, Doctor doctor)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{doctor.Id}/reassign", appointment);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw;
            }
        }

        public async Task CancelAppointmentAsync(Appointment appointment)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{appointment.Id}/cancel", appointment);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw;
            }
        }
    }
}