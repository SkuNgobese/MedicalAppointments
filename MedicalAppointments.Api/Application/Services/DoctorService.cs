using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class DoctorService : IDoctor
    {
        private readonly IRepository<Doctor> _repository;

        public DoctorService(IRepository<Doctor> repository) => _repository = repository;

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            var doctors =  await _repository.GetAllAsync();

            return doctors.Select(d => new Doctor
            {
                Id = d.Id,
                Title = d.Title,
                FirstName = d.FirstName,
                LastName = d.LastName,
                IDNumber = d.IDNumber,
                Specialization = d.Specialization,

                Address = new Address
                {
                    Id = d.Address?.Id ?? 0,
                    Street = d.Address?.Street ?? string.Empty,
                    Suburb = d.Address?.Suburb ?? string.Empty,
                    City = d.Address?.City ?? string.Empty,
                    PostalCode = d.Address?.PostalCode ?? string.Empty,
                    Country = d.Address?.Country ?? string.Empty
                },
                Contact = new Contact
                {
                    Id = d.Contact?.Id ?? 0,
                    Email = d.Contact?.Email ?? string.Empty,
                    ContactNumber = d.Contact?.ContactNumber ?? string.Empty,
                    Fax = d.Contact?.Fax ?? string.Empty
                }
            });
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(Hospital hospital)
        {
            var doctors = await _repository.GetAllAsync(d => d.Hospital!.Id == hospital.Id);

            if (!doctors.Any())
                return [];

            return doctors.Select(d => new Doctor
            {
                Id = d.Id,
                Title = d.Title,
                FirstName = d.FirstName,
                LastName = d.LastName,
                IDNumber = d.IDNumber,
                Specialization = d.Specialization,
                Hospital = hospital,

                Address = new Address
                {
                    Id = d.Address?.Id ?? 0,
                    Street = d.Address?.Street ?? string.Empty,
                    Suburb = d.Address?.Suburb ?? string.Empty,
                    City = d.Address?.City ?? string.Empty,
                    PostalCode = d.Address?.PostalCode ?? string.Empty,
                    Country = d.Address?.Country ?? string.Empty
                },
                Contact = new Contact
                {
                    Id = d.Contact?.Id ?? 0,
                    Email = d.Contact?.Email ?? string.Empty,
                    ContactNumber = d.Contact?.ContactNumber ?? string.Empty,
                    Fax = d.Contact?.Fax ?? string.Empty
                }
            });
        }

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
