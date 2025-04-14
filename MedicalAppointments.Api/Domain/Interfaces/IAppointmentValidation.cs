using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Domain.Interfaces
{
    public interface IAppointmentValidation
    {
        bool CanSchedule(DateTime date, Doctor doctor, Patient patient);
        bool CanReschedule(DateTime newDate, Appointment appointment);
        bool CanReassign(Doctor doctor, Appointment appointment);
        bool CanCancel(Appointment appointment);
    }
}