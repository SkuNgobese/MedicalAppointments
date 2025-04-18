﻿using MedicalAppointments.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace MedicalAppointments.Providers
{
    public class ApiAuthenticationStateProvider(IHttpClientFactory httpClientFactory,
                                                AuthHelper authHelper) : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthorizedAPI");
        private readonly AuthHelper _authHelper = authHelper;
        private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _authHelper.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
            
            return new AuthenticationState(authenticatedUser);
        }

        public void MarkUserAsAuthenticated(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "apiauth");
            var user = new ClaimsPrincipal(identity);
            var authState = Task.FromResult(new AuthenticationState(user));

            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }

        private static List<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles != null)
            {
                if (!roles.ToString()!.Trim().StartsWith('['))
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                else
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

                    foreach (var parsedRole in parsedRoles!)
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}