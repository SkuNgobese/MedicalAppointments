using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Client.Services
{
    public class DoctorService : IDoctor
    {
        private readonly HttpClient _http;
        private const string _directory = "/doctors";

        public DoctorService(HttpClient http) => _http = http;

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_directory))
                return [];

            try
            {
                var doctors = await _http.GetFromJsonAsync<IEnumerable<Doctor>>($"{_http.BaseAddress}{_directory}");
                return doctors ?? [];
            }
            catch
            {
                return [];
            }
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital)
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_directory) || hospital?.Id == null)
                return [];

            try
            {
                var doctors = await _http.GetFromJsonAsync<IEnumerable<Doctor>>(
                    $"{_http.BaseAddress}{_directory}?hospitalId={hospital.Id}");
                return doctors ?? [];
            }
            catch
            {
                return [];
            }
        }

        public async Task<Doctor> EnrollDoctorAsync(Doctor doctor)
        {
            if (_http.BaseAddress is null || string.IsNullOrWhiteSpace(_directory) || doctor is null)
                return null!;

            try
            {
                var response = await _http.PostAsJsonAsync($"{_http.BaseAddress}{_directory}", doctor);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Doctor>() ?? null!;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<Doctor?> GetDoctorByIdAsync(string id) => 
            await _http.GetFromJsonAsync<Doctor>($"{_http.BaseAddress?.ToString() + _directory}/{id}");

        public async Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital) => 
            await _http.GetFromJsonAsync<Doctor>($"{_http.BaseAddress?.ToString() + _directory}/{id}?hospitalId={hospital.Id}");

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            var response = await _http.PutAsJsonAsync($"{_http.BaseAddress?.ToString() + _directory}/{doctor.Id}", doctor);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveDoctorAsync(Doctor doctor)
        {
            var response = await _http.DeleteAsync($"{_http.BaseAddress?.ToString() + _directory}/{doctor.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}