using MedicalAppointments.Interfaces;
using MedicalAppointments.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Services
{
    public class DoctorService : IDoctor
    {
        private readonly HttpClient _http;
        private const string _directory = "/doctors";

        public DoctorService(HttpClient http) => _http = http;

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync() => 
            await _http.GetFromJsonAsync<IEnumerable<Doctor>>(_http.BaseAddress?.ToString() + _directory);

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital) => 
            await _http.GetFromJsonAsync<IEnumerable<Doctor>>($"{_http.BaseAddress?.ToString() + _directory}?hospitalId={hospital.Id}");

        public async Task<Doctor> EnrollDoctorAsync(Doctor doctor)
        {
            var response = await _http.PostAsJsonAsync(_http.BaseAddress?.ToString() + _directory, doctor);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Doctor>();
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