using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Api.Infrastructure.Persistence.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private object d;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) => d = new object();

    public DbSet<SuperAdmin> SuperAdmins { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
}
