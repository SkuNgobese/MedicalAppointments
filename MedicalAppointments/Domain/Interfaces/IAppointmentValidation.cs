using MedicalAppointments.Application.Models;

namespace MedicalAppointments.Application.Interfaces
{
    public interface IAppointmentValidation
    {
        bool CanSchedule(DateTime date, Doctor doctor, Patient patient);
        bool CanReschedule(DateTime newDate, Appointment appointment);
        bool CanReassign(Doctor doctor, Appointment appointment);
        bool CanCancel(Appointment appointment);
    }
}
