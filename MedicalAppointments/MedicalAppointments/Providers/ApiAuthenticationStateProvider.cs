using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace MedicalAppointments.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;

        public ApiAuthenticationStateProvider(HttpClient httpClient,
                                              ILocalStorageService localStorage,
                                              ISessionStorageService sessionStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;

        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await GetTokenAsync();

                if (string.IsNullOrWhiteSpace(savedToken))
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop", StringComparison.InvariantCultureIgnoreCase))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void MarkUserAsAuthenticated(string email)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, email)], "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles != null)
            {
                if (roles.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

                    foreach (var parsedRole in parsedRoles!)
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
                else
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private async Task<string?> GetTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return token ?? await _sessionStorage.GetItemAsync<string>("authToken");
        }
    }
}
