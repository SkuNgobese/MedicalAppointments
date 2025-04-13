using MedicalAppointments.Api.Application.Helpers;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Models;
using System.Collections;

namespace MedicalAppointments.Api.Application.Services
{
    public class DoctorService : IDoctor
    {
        private readonly IRepository<Doctor> _repository;

        private readonly CurrentUserHelper _helper;

        public DoctorService(IRepository<Doctor> repository, CurrentUserHelper helper)
        {
            _repository = repository;
            _helper = helper;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            var doctors = await _repository.GetAllAsync(
                                d => d.Address!,
                                d => d.Contact!,
                                d => d.Hospital!
                            );

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
            var doctors = await _repository.GetAllAsync(
                                d => d.Address!,
                                d => d.Contact!,
                                d => d.Hospital!
                            );

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

        public async Task RemoveDoctorsAsync(Hospital hospital)
        {
            var doctors = await _repository.GetAllAsync(d => d.Hospital == hospital);

            if (doctors == null)
                return;

            foreach (var doctor in doctors)
                await _repository.DeleteAsync(doctor);
        }

        public async Task<Doctor?> GetDoctorAsync(string email) => 
            await _repository.GetByConditionAsync(d => d.Email == email);

        public async Task<bool> ExistsAsync(string email) => 
            await _repository.Exists(d => d.Email == email);

        public async Task<IEnumerable<Doctor?>> GetCurrentUserHospitalDoctorsAsync()
        {
            var user = await _helper.GetCurrentUserAsync() ?? throw new InvalidOperationException("Unauthorized.");
            var role = await _helper.GetUserRoleAsync() ?? throw new InvalidOperationException("Unauthorized.");

            IEnumerable<Doctor> doctors = null!;

            switch (role)
            {
                case "Doctor":
                    var doctor = user as Doctor;
                    doctors = doctors!.Append(doctor!);
                    break;
                case "SysAdmin":
                    var admin = user as SysAdmin;
                    doctors = await GetAllDoctorsAsync(admin!.Hospital!);
                    break;
                case "SuperAdmin":
                    doctors = await GetAllDoctorsAsync();
                    break;
                default:
                    return doctors;
            }

            return doctors;
        }
    }
}
