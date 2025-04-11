using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Providers;
using MedicalAppointments.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace MedicalAppointments.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private const string TokenKey = "authToken";

        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationStateProvider AuthenticationStateProvider => _authenticationStateProvider;

        public AuthService(
            IHttpClientFactory httpClientFactory, 
            ILocalStorageService localStorage, 
            ISessionStorageService sessionStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("AuthorizedAPI");
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/Auth", 
                new StringContent(loginAsJson, 
                                  Encoding.UTF8, 
                                  "application/json"));
            var loginResult = JsonSerializer.Deserialize<AuthResponseDto>(
                    await response.Content.ReadAsStringAsync(), 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

            if (!response.IsSuccessStatusCode)
                return loginResult!;

            ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                                            .MarkUserAsAuthenticated(loginResult!.Token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult!.Token);

            return loginResult!;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            await _sessionStorage.RemoveItemAsync(TokenKey);

            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();

            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}