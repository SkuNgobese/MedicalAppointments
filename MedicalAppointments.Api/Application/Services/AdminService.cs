using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class AdminService : IAdmin
    {
        private readonly IRepository<Admin> _repository;

        public AdminService(IRepository<Admin> repository) => _repository = repository;

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync() =>
            await _repository.GetAllAsync(sa => sa.Hospital!);

        public async Task<Admin?> GetAdminAsync(string email) => 
            await _repository.GetByConditionAsync(sa => sa.Email == email, p => p.Hospital!) ?? null;

        public async Task<Admin> AddAdminAsync(Admin admin) =>
            await _repository.AddAsync(admin);

        public async Task DeleteAdminAsync(Hospital hospital)
        {
            var admins = await _repository.GetAllAsync(sa => sa.Hospital == hospital);
            
            if (admins == null)
                return;

            foreach (var admin in admins)
                await _repository.DeleteAsync(admin);
        }

        public async Task<bool> ExistsAsync(string email) =>
            await _repository.Exists(sa => sa.Email == email);
    }
}