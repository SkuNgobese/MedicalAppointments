using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;

namespace MedicalAppointments.Application.Services
{
    public class DoctorService : IDoctor
    {
        public Task AddAsync(Doctor appointment)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Doctor>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Doctor?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Doctor appointment)
        {
            throw new NotImplementedException();
        }
    }
}
