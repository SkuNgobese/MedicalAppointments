using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MedicalAppointments.Client.Pages;
using MedicalAppointments.Components;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Providers;
using MedicalAppointments.Services;
using MedicalAppointments.Shared.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!) });

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHospital, HospitalService>();
builder.Services.AddScoped<IDoctor, DoctorService>();
builder.Services.AddScoped<IAppointment, AppointmentService>();
builder.Services.AddScoped<IPatient, PatientService>();

builder.Services.AddAuthorization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MedicalAppointments.Client._Imports).Assembly);

app.Run();