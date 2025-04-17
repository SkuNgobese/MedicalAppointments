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
                    Message = "No doctors found."
                };
                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    Message = "An error occurred while fetching doctors.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<ErrorViewModel> EnrollDoctorAsync(Doctor doctor)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{_endPoint}", doctor);
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
                    Message = "Success: Doctor added successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while adding the doctor.",
                    Errors = [ex.Message]
                };
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
                    Message = "An error occurred while fetching the doctor.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital)
        {
            try
            {
                return await _http.GetFromJsonAsync<Doctor>($"{_endPoint}/{id}?Id={hospital.Id}");
            }
            catch(HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
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
                    Message = "An error occurred while fetching the doctor.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<ErrorViewModel> UpdateDoctorAsync(Doctor doctor)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{doctor.Id}", doctor);
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
                    Message = "Success: Doctor updated successfully."
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

        public async Task<ErrorViewModel> RemoveDoctorAsync(Doctor doctor)
        {
            try
            {
                var response = await _http.DeleteAsync($"{_endPoint}/{doctor.Id}");
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
                    Message = "Success: Doctor removed successfully."
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
    }
}