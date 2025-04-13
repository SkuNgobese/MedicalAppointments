using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Interfaces;
using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class SuperAdminService : ISuperAdmin
    {
        private readonly IRepository<SuperAdmin> _repository;

        public SuperAdminService(IRepository<SuperAdmin> repository) => _repository = repository;

        public async Task<SuperAdmin?> GetAdminAsync(string email) => 
            await _repository.GetByConditionAsync(a => a.Email == email);

        public async Task<bool> ExistsAsync(string email) => 
            await _repository.Exists(sa => sa.Email == email);
    }
}
