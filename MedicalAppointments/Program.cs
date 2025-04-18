using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MedicalAppointments;
using MedicalAppointments.Handlers;
using MedicalAppointments.Helpers;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Providers;
using MedicalAppointments.Services;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!) });

builder.Services.AddHttpClient("AuthorizedAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
})
.AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddScoped<AuthTokenHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHelper>();

builder.Services.AddScoped<IHospital, HospitalService>();
builder.Services.AddScoped<IDoctor, DoctorService>();
builder.Services.AddScoped<IAppointment, AppointmentService>();
builder.Services.AddScoped<IPatient, PatientService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();