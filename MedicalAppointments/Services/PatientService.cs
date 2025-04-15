using MedicalAppointments.Shared.Models;
using MedicalAppointments.Interfaces;
using System.Net.Http.Json;
using MedicalAppointments.Shared.ViewModels;
using System.Net;

namespace MedicalAppointments.Services
{
    public class PatientService : IPatient
    {
        private readonly HttpClient _http;
        private const string _endPoint = "api/Patients";

        public PatientService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");

        public async Task<IEnumerable<PatientViewModel>> GetAllPatientsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<PatientViewModel>>(_endPoint) ?? [];
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null!;
            }
        }

        public async Task<Patient?> GetPatientByIdNumberOrContactAsync(string term)
        {
            try
            {
                return await _http.GetFromJsonAsync<Patient>($"{_endPoint}/patientsearch?term={term}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Patient?> GetPatientByIdAsync(string id) => 
            await _http.GetFromJsonAsync<Patient>($"{_endPoint}/{id}");

        public async Task<Patient> AddPatientAsync(Patient patient)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_endPoint}", patient);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Patient>() ?? null!;
            }
            catch(HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null!;
            }
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            var response = await _http.PutAsJsonAsync($"{_endPoint}/{patient.Id}", patient);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemovePatientAsync(Patient patient)
        {
            var response = await _http.DeleteAsync($"{_endPoint}/{patient.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}