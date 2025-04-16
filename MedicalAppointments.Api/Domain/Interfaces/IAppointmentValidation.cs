using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IAppointmentValidation
    {
        ErrorViewModel? CanSchedule(DateTime date, Doctor doctor, Patient patient);
        ErrorViewModel? CanReschedule(DateTime newDate, Appointment appointment);
        ErrorViewModel? CanReassign(Doctor doctor, Appointment appointment);
        ErrorViewModel? CanCancel(Appointment appointment);
    }
}