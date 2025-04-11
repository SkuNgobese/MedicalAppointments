using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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