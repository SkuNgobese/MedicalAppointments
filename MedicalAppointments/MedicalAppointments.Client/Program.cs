using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://your-api-url/api") });
builder.Services.AddScoped<IHospital, HospitalService>();
builder.Services.AddScoped<IDoctor, DoctorService>();
builder.Services.AddScoped<IAppointment, AppointmentService>();
builder.Services.AddScoped<IPatient, PatientService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

await builder.Build().RunAsync();
