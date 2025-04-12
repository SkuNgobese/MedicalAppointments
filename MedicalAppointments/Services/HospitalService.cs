using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Services
{
    public class HospitalService : IHospital
    {
        private readonly HttpClient _http;
        private const string _endPoint = "api/Hospitals";

        public HospitalService(IHttpClientFactory httpClientFactory) => _http = httpClientFactory.CreateClient("AuthorizedAPI");

        public async Task<Hospital> AddHospitalAsync(Hospital hospital)
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_endPoint) || hospital is null)
                throw new ArgumentNullException(nameof(hospital));

            try
            {
                var response = await _http.PostAsJsonAsync($"{_endPoint}", hospital);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Hospital>() ?? throw new InvalidOperationException("Failed to deserialize the response.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the hospital.", ex);
            }
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_endPoint))
                return [];

            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<Hospital>>($"{_endPoint}") ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id) => 
            await _http.GetFromJsonAsync<Hospital>($"{_endPoint}/{id}");

        public async Task RemoveHospitalAsync(Hospital hospital)
        {
            var response = await _http.DeleteAsync($"{_endPoint}/{hospital.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHospitalAsync(Hospital hospital)
        {
            var response = await _http.PutAsJsonAsync($"{_endPoint}/{hospital.Id}", hospital);
            response.EnsureSuccessStatusCode();
        }
    }
}