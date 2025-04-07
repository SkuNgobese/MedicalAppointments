using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class DoctorService : IDoctor
    {
        private readonly IRepository<Doctor> _repository;

        public DoctorService(IRepository<Doctor> repository) => _repository = repository;

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync() =>
            await _repository.GetAllAsync();

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital) =>
            await _repository.GetAllAsync(d => d.Hospital == hospital);

        public async Task<Doctor> EnrollDoctorAsync(Doctor doctor) =>
            await _repository.AddAsync(doctor);

        public async Task<Doctor?> GetDoctorByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task<Doctor?> GetDoctorByIdAsync(string id, Hospital hospital)
        {
            return await _repository.GetByConditionAsync(
                d => d.Id == id && d.Hospital == hospital
            );
        }

        public async Task UpdateDoctorAsync(Doctor doctor) =>
            await _repository.UpdateAsync(doctor);

        public async Task RemoveDoctorAsync(Doctor doctor) =>
            await _repository.DeleteAsync(doctor);
    }
}
