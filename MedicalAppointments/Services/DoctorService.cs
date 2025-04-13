using MedicalAppointments.Interfaces;
using MedicalAppointments.Api.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Services
{
    public class DoctorService : IDoctor
    {
        private readonly HttpClient _http;
        private const string _endPoint = "api/Doctors";

        public DoctorService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_endPoint))
                return [];

            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<Doctor>>($"{_endPoint}") ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital)
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_endPoint) || hospital?.Id == null)
                return [];

            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<Doctor>>(
                    $"{_endPoint}?hospitalId={hospital.Id}") ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Doctor> EnrollDoctorAsync(Doctor doctor)
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_endPoint) || doctor is null)
                return null!;

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
            await _http.GetFromJsonAsync<Doctor>($"{_http.BaseAddress?.ToString() + _endPoint}/{id}");

        public async Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital) => 
            await _http.GetFromJsonAsync<Doctor>($"{_endPoint}/{id}?hospitalId={hospital.Id}");

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