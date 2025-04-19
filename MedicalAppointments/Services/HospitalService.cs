using MedicalAppointments.Interfaces;
using System.Net.Http.Json;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace MedicalAppointments.Services
{
    public class HospitalService : IHospital
    {
        private readonly HttpClient _http;
        public ErrorViewModel? Error { get; set; } = new();
        private const string _endPoint = "api/Hospitals";

        public HospitalService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");

        public async Task<IEnumerable<HospitalViewModel>> GetAllHospitalsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<HospitalViewModel>>($"{_endPoint}") ?? [];
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No hospitals found."
                };
                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching hospitals.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<HospitalViewModel?> GetHospitalByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<HospitalViewModel>($"{_endPoint}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Hospital not found."
                };
                return null;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching the hospital.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<ErrorViewModel> AddHospitalAsync(HospitalViewModel model)
        {
            try
            {
                var hospital = new Hospital
                {
                    Name = model.HospitalName,
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

                var response = await _http.PostAsJsonAsync($"{_endPoint}", hospital);

                if (!response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    ErrorViewModel? error = JsonSerializer.Deserialize<ErrorViewModel>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return error ?? new ErrorViewModel
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred.",
                        Errors = [json]
                    };
                }

                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Hospital added successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the hospital.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> UpdateHospitalAsync(HospitalViewModel model)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{model.Id}", model);

                if (!response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    ErrorViewModel? error = JsonSerializer.Deserialize<ErrorViewModel>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return error ?? new ErrorViewModel
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred.",
                        Errors = [json]
                    };
                }

                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Hospital updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the hospital.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> RemoveHospitalAsync(int hospitalId)
        {
            try
            {
                var response = await _http.DeleteAsync($"{_endPoint}/{hospitalId}");

                if (!response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    ErrorViewModel? error = JsonSerializer.Deserialize<ErrorViewModel>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return error ?? new ErrorViewModel
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred.",
                        Errors = [json]
                    };
                }

                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Hospital removed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while removing the hospital.",
                    Errors = [ex.Message]
                };
            }
        }
    }
}