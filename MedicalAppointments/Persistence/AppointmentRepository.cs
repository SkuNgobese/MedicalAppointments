using MedicalAppointments.Data;
using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Persistence
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(int id) =>
            await _context.Appointments.FindAsync(id);

        public async Task<IEnumerable<Appointment>> GetAllAsync() =>
            await _context.Appointments.ToListAsync();

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
