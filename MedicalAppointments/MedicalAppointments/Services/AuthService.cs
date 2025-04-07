using Blazored.LocalStorage;
using Blazored.SessionStorage;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.DTOs.Auth;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MedicalAppointments.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private const string TokenKey = "authToken";

        public AuthService(HttpClient http, ILocalStorageService localStorage, ISessionStorageService sessionStorage)
        {
            _http = http;
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
        }

        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var response = await _http.PostAsJsonAsync("auth/login", loginDto);
            if (!response.IsSuccessStatusCode)
                return false;

            var loginResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
                return false;

            if (loginDto.RememberMe)
                await _localStorage.SetItemAsync(TokenKey, loginResponse.Token);
            else
                await _sessionStorage.SetItemAsync(TokenKey, loginResponse.Token);
           
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);
            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            await _sessionStorage.RemoveItemAsync(TokenKey);
            _http.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<string?> GetTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TokenKey);
            return token ?? await _sessionStorage.GetItemAsync<string>(TokenKey);
        }
    }
}