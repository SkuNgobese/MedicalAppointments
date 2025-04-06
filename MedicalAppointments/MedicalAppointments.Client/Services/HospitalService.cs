using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Client.Services
{
    public class HospitalService : IHospital
    {
        private readonly HttpClient _http;
        private const string _directory = "/hospitals";

        public HospitalService(HttpClient http) => _http = http;

        public async Task<Hospital> AddHospitalAsync(Hospital hospital)
        {
            var response = await _http.PostAsJsonAsync(_http.BaseAddress?.ToString() + _directory, hospital);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Hospital>();
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync() => 
            await _http.GetFromJsonAsync<IEnumerable<Hospital>>(_http.BaseAddress?.ToString() + _directory);

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