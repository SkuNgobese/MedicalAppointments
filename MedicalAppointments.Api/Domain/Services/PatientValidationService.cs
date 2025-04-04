﻿using MedicalAppointments.Api.Domain.Enums;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Domain.Models;

namespace MedicalAppointments.Api.Domain.Services
{
    public class PatientValidationService : IPatientValidation
    {
        public bool CanAddPatient(Patient patient, List<Patient> patients)
        {
            if (patients.Any(p => p.FullName == patient.FullName && p.IDNumber == patient.IDNumber))
                throw new Exception($"{patient.FullName} already exists as patient");

            return true;
        }

        public bool CanRemovePatient(Patient patient)
        {
            if (patient.Appointments != null && patient.Appointments.Any(a => a.Date > DateTime.Now && a.Status != AppointmentStatus.Cancelled))
                throw new Exception($"{patient.FullName} has future appointments");

            return true;
        }

        public bool CanUpdatePatient(Patient patient)
        {
            throw new NotImplementedException();
        }
    }
}
