using MedicalAppointments.Interfaces;
using System.Net.Http.Json;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using System.Net;

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
                    Message = "An error occurred while fetching hospitals.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Hospital>($"{_endPoint}/{id}");
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
                    Message = "An error occurred while fetching the hospital.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<ErrorViewModel> AddHospitalAsync(Hospital hospital)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_endPoint}", hospital);
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
                    Message = "Success: Hospital added successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while adding the hospital.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> UpdateHospitalAsync(Hospital hospital)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{hospital.Id}", hospital);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Errors = [response.ReasonPhrase],
                            Message = "An unknown error occurred."
                        };

                return new ErrorViewModel
                {
                    Message = "Success: Hospital updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
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
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ErrorViewModel>() ??
                        new ErrorViewModel
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Errors = [response.ReasonPhrase],
                            Message = "An unknown error occurred."
                        };

                return new ErrorViewModel
                {
                    Message = "Success: Hospital removed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while removing the hospital.",
                    Errors = [ex.Message]
                };
            }
        }
    }
}