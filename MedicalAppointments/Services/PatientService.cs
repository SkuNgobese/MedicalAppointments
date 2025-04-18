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
        public ErrorViewModel? Error { get; set; } = new();
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
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No patients found."
                };

                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching patients.",
                    Errors = [ex.Message]
                };
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
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Patient not found."
                };
                return null;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while searching for the patient.",
                    Errors = [ex.Message]
                };
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
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Patient not found."
                };
                return null;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    Message = "An error occurred while fetching the patient.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<PatientViewModel> AddPatientAsync(PatientViewModel model)
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

                if (!response.IsSuccessStatusCode)
                    Error = await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unknown error occurred.",
                            Errors = [response.ReasonPhrase]
                        };

                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Patient added successfully."
                };

                return await response.Content.ReadFromJsonAsync<PatientViewModel>() ?? null!;
            }
            catch(HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Patient not found."
                };
                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    Message = "An error occurred while adding the patient.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<ErrorViewModel?> UpdatePatientAsync(Patient patient)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{patient.Id}", patient);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unknown error occurred.",
                            Errors = [response.ReasonPhrase]
                        };

                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Patient updated successfully."
                };
            }
            catch(HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Patient not found."
                };
            }
            catch(Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the patient.",
                    Errors = [ex.Message]
                };
            }
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
                            Message = "An unknown error occurred.",
                            Errors = [response.ReasonPhrase]
                        };

                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Patient removed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while removing the patient.",
                    Errors = [ex.Message]
                };
            }
        }
    }
}