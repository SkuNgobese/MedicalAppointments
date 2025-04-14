using MedicalAppointments.Shared.Domain.Interfaces;
using MedicalAppointments.Shared.Enums;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Domain.Services
{
    public class DoctorValidationService : IDoctorValidation
    {
        public bool CanAdd(Doctor doctor, List<Doctor> doctors)
        {
            if (doctors.Any(d => d.FullName == doctor.FullName && d.IDNumber == doctor.IDNumber))
                throw new Exception($"{doctor.FullName} already exists");

            return true;
        }

        public bool CanRemove(Doctor doctor, DateTime removeDate = default)
        {
            removeDate = removeDate == default ? DateTime.Now : removeDate;

            if (doctor.Appointments != null && doctor.Appointments.Any(a => a.Date > removeDate && a.Status != AppointmentStatus.Cancelled))
                throw new Exception($"{doctor.FullName} has coming appointments");

            return true;
        }

        public bool CanRetire(Doctor doctor, DateTime retireDate)
        {
            return CanRemove(doctor, retireDate);
        }
    }
}
