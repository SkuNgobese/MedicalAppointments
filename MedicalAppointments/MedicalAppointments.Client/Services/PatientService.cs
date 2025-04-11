using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using System.Net.Http.Json;

namespace MedicalAppointments.Client.Services
{
    public class PatientService : IPatient
    {
        private readonly HttpClient _http;
        private const string _directory = "/patients";

        public PatientService(HttpClient http) => _http = http;

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(Hospital hospital)
        {
            if (_http.BaseAddress is null)
                return [];

            try
            {
                var patients = await _http.GetFromJsonAsync<IEnumerable<Patient>>($"{_http.BaseAddress}{_directory}?hospitalId={hospital.Id}");
                return patients ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task AddPatientAsync(Patient patient)
        {
            var response = await _http.PostAsJsonAsync(_http.BaseAddress?.ToString() + _directory, patient);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Patient?> GetPatientByIdAsync(string id) => 
            await _http.GetFromJsonAsync<Patient>($"{_http.BaseAddress?.ToString() + _directory}/{id}");

        public async Task UpdatePatientAsync(Patient patient)
        {
            var response = await _http.PutAsJsonAsync($"{_http.BaseAddress?.ToString() + _directory}/{patient.Id}", patient);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemovePatientAsync(Patient patient)
        {
            var response = await _http.DeleteAsync($"{_http.BaseAddress?.ToString() + _directory}/{patient.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}