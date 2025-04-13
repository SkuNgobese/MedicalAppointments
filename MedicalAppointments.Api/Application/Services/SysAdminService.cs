using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class SysAdminService : ISysAdmin
    {
        private readonly IRepository<SysAdmin> _repository;

        public SysAdminService(IRepository<SysAdmin> repository) => _repository = repository;

        public async Task<IEnumerable<SysAdmin>> GetAllSysAdminsAsync() =>
            await _repository.GetAllAsync(sa => sa.Hospital!);

        public async Task<SysAdmin?> GetAdminAsync(string email) => 
            await _repository.GetByConditionAsync(sa => sa.Email == email, p => p.Hospital!) ?? null;

        public async Task RemoveAdminAsync(Hospital hospital)
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