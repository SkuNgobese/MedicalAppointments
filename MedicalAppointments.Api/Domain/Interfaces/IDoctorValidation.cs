using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Api.Domain.Interfaces
{
    public interface IDoctorValidation
    {
        ErrorViewModel? CanAdd(Doctor doctor, List<Doctor> doctors);
        ErrorViewModel? CanUpdate(Doctor doctor);
        ErrorViewModel? CanRetire(Doctor doctor, DateTime retireDate);
        ErrorViewModel? CanRemove(Doctor doctor, DateTime removeDate = default);
    }
}