using MedicalAppointments.Interfaces;
using MedicalAppointments.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Services
{
    public class HospitalService : IHospital
    {
        private readonly HttpClient _http;
        private const string _directory = "/hospitals";

        public HospitalService(HttpClient http) => _http = http;

        public async Task<Hospital> AddHospitalAsync(Hospital hospital)
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_directory) || hospital is null)
                throw new ArgumentNullException(nameof(hospital));

            try
            {
                var response = await _http.PostAsJsonAsync($"{_http.BaseAddress}{_directory}", hospital);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<Hospital>();
                return result is null ? throw new InvalidOperationException("Failed to deserialize the response.") : result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the hospital.", ex);
            }
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_directory))
                return [];

            try
            {
                var hospitals = await _http.GetFromJsonAsync<IEnumerable<Hospital>>($"{_http.BaseAddress}{_directory}");
                return hospitals ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id) => 
            await _http.GetFromJsonAsync<Hospital>($"{_http.BaseAddress?.ToString() + _directory}/{id}");

        public async Task RemoveHospitalAsync(Hospital hospital)
        {
            var response = await _http.DeleteAsync($"{_http.BaseAddress?.ToString() + _directory}/{hospital.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHospitalAsync(Hospital hospital)
        {
            var response = await _http.PutAsJsonAsync($"{_http.BaseAddress?.ToString() + _directory}/{hospital.Id}", hospital);
            response.EnsureSuccessStatusCode();
        }
    }
}