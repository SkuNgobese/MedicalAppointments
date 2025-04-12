using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Api.Application.Services
{
    public class SysAdminService : ISysAdmin
    {
        private readonly IRepository<SysAdmin> _repository;

        public SysAdminService(IRepository<SysAdmin> repository) => _repository = repository;

        public async Task<IEnumerable<SysAdmin>> GetAllSysAdminsAsync() =>
            await _repository.GetAllAsync(sa => sa.Hospital!);

        public async Task<SysAdmin?> GetAdminAsync(string email) => await _repository.GetByConditionAsync(sa => sa.Email == email, p => p.Hospital!) ?? null;
    }
}