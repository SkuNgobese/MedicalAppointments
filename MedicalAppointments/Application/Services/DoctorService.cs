using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Application.Services
{
    public class DoctorService : IDoctor
    {
        private readonly IRepository<Doctor> _repository;

        public DoctorService(IRepository<Doctor> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync() =>
            await _repository.GetAllAsync();

        public async Task EnrollDoctorAsync(Doctor doctor) =>
            await _repository.AddAsync(doctor);

        public async Task<Doctor?> GetDoctorByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task UpdateDoctorAsync(Doctor doctor) =>
            await _repository.UpdateAsync(doctor);

        public async Task RemoveDoctorAsync(string id) =>
            await _repository.DeleteAsync(id);
    }
}
