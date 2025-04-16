using MedicalAppointments.Shared.Models;
using MedicalAppointments.Interfaces;
using System.Net.Http.Json;
using MedicalAppointments.Shared.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Http;

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

        public async Task<PatientViewModel?> GetPatientByIdNumberOrContactAsync(string term)
        {
            try
            {
                return await _http.GetFromJsonAsync<PatientViewModel>($"{_endPoint}/patientsearch?term={term}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Patient?> GetPatientByIdAsync(string id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Patient>($"{_endPoint}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Patient> AddPatientAsync(PatientViewModel model)
        {
            try
            {
                var patient = new Patient
                {
                    Title = model.Title,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IDNumber = model.IDNumber,
                    PrimaryDoctorId = model.Id,
                    Contact = new Contact
                    {
                        Email = model.ContactDetails!.Email,
                        ContactNumber = model.ContactDetails.ContactNumber,
                        Fax = model.ContactDetails.Fax
                    },
                    Address = new Address
                    {
                        Street = model.AddressDetails!.Street,
                        Suburb = model.AddressDetails.Suburb,
                        City = model.AddressDetails!.City,
                        PostalCode = model.AddressDetails!.PostalCode,
                        Country = model.AddressDetails.Country
                    }
                };

                var response = await _http.PostAsJsonAsync($"{_endPoint}", patient);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Patient>() ?? null!;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null!;
            }
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            var response = await _http.PutAsJsonAsync($"{_endPoint}/{patient.Id}", patient);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ErrorViewModel> RemovePatientAsync(Patient patient)
        {
            try
            { 
                var response = await _http.DeleteAsync($"{_endPoint}/{patient.Id}");
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unknown error occurred."
                        };

                return new ErrorViewModel
                {
                    Message = "Success: Patient removed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while adding the patient.",
                    Errors = [ex.Message]
                };
            }
        }
    }
}