using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Persistence.Data;
using MedicalAppointments.Infrastructure.Services;
using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Application.Services;
using Serilog;
using MedicalAppointments.Application.Interfaces.Shared;
using MedicalAppointments.Application.Services.Shared;
using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Services.Shared;
using MedicalAppointments.Domain.Interfaces.Shared;
using MedicalAppointments.Domain.Services;
using MedicalAppointments.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,       // Number of retry attempts
            maxRetryDelay: TimeSpan.FromSeconds(10),  // Delay between retries
            errorNumbersToAdd: null  // Use default SQL transient errors
        );
    }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity using custom User model
//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>() // Ensures UserStore is available
//    .AddDefaultTokenProviders();

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Register RoleManager explicitly
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<User>();
builder.Services.AddScoped<IUserStore<User>, UserStore<User, IdentityRole, ApplicationDbContext>>();
builder.Services.AddScoped(sp => (IUserEmailStore<User>)sp.GetRequiredService<IUserStore<User>>());

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRepository<Hospital>, Repository<Hospital>>();
builder.Services.AddScoped<IRepository<Doctor>, Repository<Doctor>>();
builder.Services.AddScoped<IRepository<Patient>, Repository<Patient>>();
builder.Services.AddScoped<IRepository<Appointment>, Repository<Appointment>>();

// Register services
builder.Services.AddScoped<IHospital, HospitalService>();
builder.Services.AddScoped<IDoctor, DoctorService>();
builder.Services.AddScoped<IPatient, PatientService>();
builder.Services.AddScoped<IAppointment, AppointmentService>();
builder.Services.AddScoped<IAddress, AddressService>();
builder.Services.AddScoped<IContact, ContactService>();

// Register validation services
builder.Services.AddScoped<IHospitalValidation, HospitalValidationService>();
builder.Services.AddScoped<IDoctorValidation, DoctorValidationService>();
builder.Services.AddScoped<IPatientValidation, PatientValidationService>();
builder.Services.AddScoped<IAppointmentValidation, AppointmentValidationService>();

// Register user-related services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPatientRegistration, PatientRegistrationService>();

// Add controllers and views
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure logging
ConfigureLogging();

var app = builder.Build();

// Ensure the database is updated and seed roles/admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();

    await DbInitializer.SeedRolesAndSuperAdmin(services, configuration);
}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();

/// <summary>
/// Configures application-wide logging.
/// </summary>
static void ConfigureLogging()
{
    string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/app-.log");

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
        .CreateLogger();
}