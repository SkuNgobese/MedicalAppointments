using MedicalAppointments.Api.Domain.Models;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IAppointmentValidation
    {
        bool CanSchedule(DateTime date, Doctor doctor, Patient patient);
        bool CanReschedule(DateTime newDate, Appointment appointment);
        bool CanReassign(Doctor doctor, Appointment appointment);
        bool CanCancel(Appointment appointment);
    }
}
