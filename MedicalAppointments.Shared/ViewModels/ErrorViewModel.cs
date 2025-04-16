using Microsoft.AspNetCore.Http;

namespace MedicalAppointments.Shared.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool? ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int? StatusCode { get; set; } = StatusCodes.Status500InternalServerError;
        public List<string>? Errors { get; set; } = [];
        public string? Message { get; set; }
    }
}