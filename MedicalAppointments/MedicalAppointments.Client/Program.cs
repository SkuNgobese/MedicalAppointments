using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MedicalAppointments.Client.Handlers;
using MedicalAppointments.Client.Helpers;
using MedicalAppointments.Client.Interfaces;
using MedicalAppointments.Client.Providers;
using MedicalAppointments.Client.Services;
using MedicalAppointments.Shared.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Step 1: Load appsettings.json from wwwroot
using var response = await new HttpClient().GetAsync("appsettings.json");
var jsonStream = await response.Content.ReadAsStreamAsync();
builder.Configuration.AddJsonStream(jsonStream);

builder.Services.AddScoped<AuthTokenHandler>();
builder.Services.AddHttpClient("AuthorizedAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
}).AddHttpMessageHandler<AuthTokenHandler>();

// Provide the HttpClient instance using factory
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("AuthorizedAPI");
});

// Storage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

// Auth state
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

builder.Services.AddAuthorizationCore();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHelper>();

builder.Services.AddScoped<IHospital, HospitalService>();
builder.Services.AddScoped<IDoctor, DoctorService>();
builder.Services.AddScoped<IAppointment, AppointmentService>();
builder.Services.AddScoped<IPatient, PatientService>();

await builder.Build().RunAsync();
