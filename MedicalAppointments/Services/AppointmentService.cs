using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;

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
                    Message = "No appointments found."
                };
                return null!;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
                    Message = "An error occurred while fetching appointments.",
                    Errors = [ex.Message]
                };
                return null!;
            }
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Appointment>($"{_endPoint}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Error = new ErrorViewModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Appointment not found."
                };
                return null;
            }
            catch (Exception ex)
            {
                Error = new ErrorViewModel
                {
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
                    Message = "Success: Appointment booked successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while booking an appointment.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> RescheduleAppointmentAsync(Appointment appointment)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{appointment.Id}/reschedule/", appointment);
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
                    Message = "Success: Appointment rescheduled successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while rescheduling the appointment.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> ReAssignAppointmentAsync(Appointment appointment, Doctor doctor)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{doctor.Id}/reassign", appointment);
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
                    Message = "Success: Appointment reassigned successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while reassigning the appointment.",
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ErrorViewModel> CancelAppointmentAsync(Appointment appointment)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{_endPoint}/{appointment.Id}/cancel", appointment);
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
                    Message = "Success: Appointment cancelled successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorViewModel
                {
                    Message = "An error occurred while cancelling the appointment.",
                    Errors = [ex.Message]
                };
            }
        }
    }
}