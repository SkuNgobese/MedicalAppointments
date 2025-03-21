using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Application.Models;
using MedicalAppointments.Domain.Interfaces;

namespace MedicalAppointments.Domain.Services
{
    public class HospitalValidationService : IHospitalValidation
    {
        public bool CanAddHospital(Hospital hospital, List<Hospital> hospitals)
        {
            if (hospitals.Any(h => h.Name == hospital.Name))
                throw new ArgumentException($"{hospital.Name} already exists.");

            return true;
        }

        public bool CanUpdateHospital(Hospital hospital, List<Hospital> hospitals)
        {
            if (hospitals.Any(h => h.Name == hospital.Name))
                throw new ArgumentException($"{hospital.Name} already exists.");

            return true;
        }

        public bool CanDeleteHospital(Hospital hospital)
        {
            if (hospital.Doctors != null && hospital.Doctors.Any(p => p.IsActive == true))
                throw new ArgumentException($"{hospital.Name} has active doctors.");

            if (hospital.Patients != null && hospital.Patients.Any(p => p.IsActive == true))
                throw new ArgumentException($"{hospital.Name} has active patients.");

            return true;
        }
    }
}
