using MedicalAppointments.Shared.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MedicalAppointments.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MedicalAppointments.Api.Application.Interfaces;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Infrastructure.Persistence.Data;
using MedicalAppointments.Api.Infrastructure.Services;
using MedicalAppointments.Api.Domain.Interfaces.Shared;
using MedicalAppointments.Api.Domain.Interfaces;
using MedicalAppointments.Api.Domain.Services.Shared;
using MedicalAppointments.Api.Application.Services;
using MedicalAppointments.Api.Domain.Services;
using MedicalAppointments.Api.Application.Helpers;
using MedicalAppointments.Api.Application.Interfaces.Shared;
using MedicalAppointments.Api.Application.Services.Shared;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        );
    }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = builder.Configuration["Jwt:Key"];
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
});

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRepository<Hospital>, Repository<Hospital>>();
builder.Services.AddScoped<IRepository<Doctor>, Repository<Doctor>>();
builder.Services.AddScoped<IRepository<Patient>, Repository<Patient>>();
builder.Services.AddScoped<IRepository<Appointment>, Repository<Appointment>>();

// Register services
builder.Services.AddScoped<ISuperAdmin, SuperAdminService>();
builder.Services.AddScoped<IAdmin, AdminService>();
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

builder.Services.AddScoped<ICurrentUserHelper, CurrentUserHelper>();

// Register user-related services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped(typeof(IRegistrationService<>), typeof(RegistrationService<>));

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins(allowedOrigins!)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddAuthorization();

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

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .CacheOutput();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowBlazorClient");


// Seed roles and super admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    await DbInitializer.SeedRolesAndSuperAdmin(services, config);
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();