using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Application.Services;
using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Domain.Services.Shared;
using MedicalAppointments.Api.Domain.Services;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Infrastructure.Persistence.Data;
using MedicalAppointments.Api.Infrastructure.Services;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Application.Services.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MedicalAppointments.Api.Domain.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();  // Add default token providers for UserManager/RoleManager

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedicalAppointments API",
        Version = "v1"
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MedicalAppointments API v1");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCors("Open");

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();