using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace MedicalAppointments.Client.Helpers
{
    public class AuthHelper
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;

        public AuthHelper(ILocalStorageService localStorage, ISessionStorageService sessionStorage)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
        }

        public async Task<string?> GetTokenAsync()
        {
            if (OperatingSystem.IsBrowser())
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                return token ?? await _sessionStorage.GetItemAsync<string>("authToken");
            }

            return null;
        }
    }
}
