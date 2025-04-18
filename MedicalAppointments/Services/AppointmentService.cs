﻿using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MedicalAppointments.Services
{
    public class AppointmentService : IAppointment
    {
        private readonly HttpClient _http;
        public ErrorViewModel? Error { get; set; } = new();
        private const string _endPoint = "api/Appointments";

        public AppointmentService(IHttpClientFactory httpClientFactory) => 
            _http = httpClientFactory.CreateClient("AuthorizedAPI");


        public async Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<IEnumerable<AppointmentViewModel>>($"{_endPoint}") ?? [];
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No appointments found.",
                    Errors = [ex.Message]
                };
                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching appointments.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<AppointmentViewModel?> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<AppointmentViewModel>($"{_endPoint}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Appointment not found.",
                    Errors = [ex.Message]
                };
                return null;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while fetching the appointment.",
                    Errors = [ex.Message]
                };
                return null;
            }
        }

        public async Task<ErrorViewModel> BookAppointmentAsync(AppointmentViewModel model)
        {
            try
            {
                Doctor doctor = new()
                {
                    Id = model.DoctorId!
                };

                Patient patient = new()
                {
                    Id = model.PatientId!
                };

                var appointment = new Appointment
                {
                    Date = model.Date,
                    Description = model.Description,
                    Doctor = doctor,
                    Patient = patient,
                    Hospital = null!
                };

                var response = await _http.PostAsJsonAsync($"{_endPoint}", appointment);
                
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
                    Message = "Success: Appointment booked successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while booking the appointment.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> RescheduleAppointmentAsync(int appointmentId, DateTime newDate)
        {
            try
            {
                var response = await _http.PutAsync($"{_endPoint}/{appointmentId}/reschedule/{newDate:O}", null);

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
                    Message = "Success: Appointment rescheduled successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while rescheduling the appointment.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> ReAssignAppointmentAsync(AppointmentViewModel model, Doctor doctor)
        {
            try
            {
                var response = await _http.PutAsync($"{_endPoint}/{model.Id}/reassign/{doctor.Id}", null);
                
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

                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Appointment reassigned successfully."
                };
                return Error;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while reassigning the appointment.",
                    Errors = [ex.Message]
                };
                return Error;
            }
        }

        public async Task<ErrorViewModel> CancelAppointmentAsync(AppointmentViewModel model)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{model.Id}/cancel", model);
                
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

                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success: Appointment cancelled successfully."
                };
                return Error;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while cancelling the appointment.",
                    Errors = [ex.Message]
                };
                return Error;
            }
        }
    }
}