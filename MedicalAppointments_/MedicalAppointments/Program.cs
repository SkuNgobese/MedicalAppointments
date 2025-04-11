using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MedicalAppointments.Client.Pages;
using MedicalAppointments.Components;
using MedicalAppointments.Components.Account;
using MedicalAppointments.Handlers;
using MedicalAppointments.Helpers;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Providers;
using MedicalAppointments.Services;
using MedicalAppointments.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

////builder.Services.AddScoped(sp =>
////    new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!) });

builder.Services.AddHttpClient("AuthorizedAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
})
.AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddScoped<AuthTokenHandler>();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

builder.Services.AddScoped<IdentityRedirectManager>();
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHelper>();

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