using MedicalAppointments.Application.Models;

namespace MedicalAppointments.Domain.Interfaces
{
    public interface IDoctorValidation
    {
        bool CanAdd(Doctor doctor, List<Doctor> doctors);
        bool CanRetire(Doctor doctor, DateTime retireDate);
        bool CanRemove(Doctor doctor, DateTime removeDate = default);
    }
}
