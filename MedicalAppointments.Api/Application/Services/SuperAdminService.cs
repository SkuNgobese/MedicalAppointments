using MedicalAppointments.Shared.Application.Interfaces;
using MedicalAppointments.Shared.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Shared.Application.Services
{
    public class SuperAdminService : ISuperAdmin
    {
        private readonly IRepository<SuperAdmin> _repository;

        public SuperAdminService(IRepository<SuperAdmin> repository) => _repository = repository;

        public async Task<SuperAdmin> AddAdminAsync(SuperAdmin superAdmin) =>
            await _repository.AddAsync(superAdmin);

        public async Task<SuperAdmin?> GetAdminAsync(string email) => 
            await _repository.GetByConditionAsync(a => a.Email == email);

        public async Task<bool> ExistsAsync(string email) => 
            await _repository.Exists(sa => sa.Email == email);
    }
}
