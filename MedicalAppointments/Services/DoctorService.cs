using MedicalAppointments.Interfaces;
using System.Net.Http.Json;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Services
{
    public class DoctorService : IDoctor
    {
        private readonly HttpClient _http;
        private const string _endPoint = "api/Doctors";

        public DoctorService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");

        public async Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<DoctorViewModel>>($"{_endPoint}") ?? [];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting doctors: {ex.Message}");
                throw;
            }
        }

        public async Task<Doctor> EnrollDoctorAsync(Doctor doctor)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_endPoint}", doctor);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Doctor>() ?? null!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Doctor?> GetDoctorByIdAsync(string id) => 
            await _http.GetFromJsonAsync<Doctor>($"{_endPoint}/{id}");

        public async Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital) => 
            await _http.GetFromJsonAsync<Doctor>($"{_endPoint}/{id}?Id={hospital.Id}");

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            var response = await _http.PutAsJsonAsync($"{_endPoint}/{doctor.Id}", doctor);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveDoctorAsync(Doctor doctor)
        {
            var response = await _http.DeleteAsync($"{_endPoint}/{doctor.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}