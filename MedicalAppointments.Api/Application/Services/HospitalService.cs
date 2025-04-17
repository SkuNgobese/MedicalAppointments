using MedicalAppointments.Shared.Models;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Application.Interfaces;

namespace MedicalAppointments.Api.Application.Services
{
    public class HospitalService : IHospital
    {
        private readonly IRepository<Hospital> _hospitalRepository;
        private readonly IDoctor _doctor;
        private readonly IPatient _patient;
        private readonly IAppointment _appointment;
        private readonly IAdmin _sysAdmin;

        private readonly ICurrentUserHelper _helper;

        public HospitalService(IRepository<Hospital> hospitalRepository,
                               IDoctor doctor,
                               IPatient patient,
                               IAppointment appointment,
                               IAdmin sysAdmin,
                               ICurrentUserHelper helper)
        {
            _hospitalRepository = hospitalRepository;
            _doctor = doctor;
            _patient = patient;
            _appointment = appointment;
            _sysAdmin = sysAdmin;
            _helper = helper;
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync()
        {
            var hospitals = await _hospitalRepository.GetAllAsync(
                                            h => h.Address!, 
                                            h => h.Contact!,
                                            h => h.Doctors,
                                            h => h.Patients,
                                            h => h.Appointments);

            if (!hospitals.Any())
                return [];

            return hospitals;
        }

        public async Task<Hospital?> GetHospitalByIdAsync(int id)
        {
            var hospital = await _hospitalRepository.GetByIdAsync(
                                            id: id,
                                            h => h.Address!,
                                            h => h.Contact!);

            return hospital;
        }

        public async Task<Hospital> AddHospitalAsync(Hospital hospital) =>
            await _hospitalRepository.AddAsync(hospital);

        public async Task RemoveHospitalAsync(Hospital hospital)
        {
            await _appointment.DeleteAppointmentsAsync(hospital);
            await _patient.DeletePatientsAsync(hospital);
            await _doctor.DeleteDoctorsAsync(hospital);
            await _sysAdmin.DeleteAdminAsync(hospital);

            await _hospitalRepository.DeleteAsync(hospital, h => h.Contact!, h => h.Address!);
        }

        public async Task UpdateHospitalAsync(Hospital hospital) =>
            await _hospitalRepository.UpdateAsync(hospital);

        public async Task<Hospital?> GetCurrentUserHospitalAsync()
        {
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync() ?? throw new InvalidOperationException("Unauthorized.");

            Hospital hospital;
            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    hospital = doctor!.Hospital!;
                    break;
                case "Admin":
                    var admin = user as Admin;
                    hospital = admin!.Hospital!;
                    break;
                default:
                    return null;
            }

            return hospital;
        }
    }
}