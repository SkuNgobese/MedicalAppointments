using MedicalAppointments.Helpers;
using System.Net.Http.Headers;

namespace MedicalAppointments.Handlers
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly AuthHelper _authHelper;

        public AuthTokenHandler(AuthHelper authHelper) => _authHelper = authHelper;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authHelper.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
