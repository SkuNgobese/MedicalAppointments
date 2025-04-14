namespace MedicalAppointments.Shared.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Successful { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public IEnumerable<string>? Roles { get; set; }
    }
}