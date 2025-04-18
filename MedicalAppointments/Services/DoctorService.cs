using MedicalAppointments.Interfaces;
using System.Net.Http.Json;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace MedicalAppointments.Services
{
    public class DoctorService : IDoctor
    {
        private readonly HttpClient _http;
        public ErrorViewModel? Error { get; set; } = new();
        private const string _endPoint = "api/Doctors";

        public DoctorService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");

        public async Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<DoctorViewModel>>($"{_endPoint}") ?? [];
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No doctors found.",
                };
                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching doctors.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<Doctor?> GetDoctorByIdAsync(string id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Doctor>($"{_endPoint}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Doctor not found."
                };
                return null;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching the doctor.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<ErrorViewModel> EnrollDoctorAsync(DoctorViewModel model)
        {
            try
            {
                var doctor = new Doctor
                {
                    Title = model.Title,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IDNumber = model.IDNumber,
                    Specialization = model.Specialization,
                    HireDate = model.HireDate,
                    Address = new Address
                    {
                        Street = model.AddressDetails!.Street,
                        Suburb = model.AddressDetails.Suburb,
                        City = model.AddressDetails.City,
                        PostalCode = model.AddressDetails.PostalCode,
                        Country = model.AddressDetails.Country
                    },
                    Contact = new Contact
                    {
                        ContactNumber = model.ContactDetails!.ContactNumber,
                        Fax = model.ContactDetails.Fax,
                        Email = model.ContactDetails.Email
                    }
                };

                var response = await _http.PostAsJsonAsync($"{_endPoint}", doctor);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unknown error occurred.",
                            Errors = [response.ReasonPhrase]
                        };

                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Doctor added successfully."
                };
                return Error;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the doctor.",
                    Errors = [ex.Message]
                };
                return Error;
            }
        }

        public async Task<ErrorViewModel> UpdateDoctorAsync(DoctorViewModel model)
        {
            try
            {
                var doctor = new Doctor
                {
                    Title = model.Title,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IDNumber = model.IDNumber,
                    Specialization = model.Specialization,
                    HireDate = model.HireDate,
                    Address = new Address
                    {
                        Street = model.AddressDetails!.Street,
                        Suburb = model.AddressDetails.Suburb,
                        City = model.AddressDetails.City,
                        PostalCode = model.AddressDetails.PostalCode,
                        Country = model.AddressDetails.Country
                    },
                    Contact = new Contact
                    {
                        ContactNumber = model.ContactDetails!.ContactNumber,
                        Fax = model.ContactDetails.Fax,
                        Email = model.ContactDetails.Email
                    }
                };

                var response = await _http.PutAsJsonAsync($"{_endPoint}/{doctor.Id}", doctor);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unknown error occurred.",
                            Errors = [response.ReasonPhrase]
                        };

                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Doctor updated successfully."
                };
                return Error;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the doctor.",
                    Errors = [ex.Message]
                };
                return Error;
            }
        }

        public async Task<ErrorViewModel> RemoveDoctorAsync(string id)
        {
            try
            {
                var response = await _http.DeleteAsync($"{_endPoint}/{id}");
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "An unknown error occurred.",
                            Errors = [response.ReasonPhrase]
                        };

                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Doctor removed successfully."
                };
                return Error;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while removing the doctor.",
                    Errors = [ex.Message]
                };
                return Error;
            }
        }
    }
}