using Microsoft.AspNetCore.Identity.UI.Services;

namespace MedicalAppointments.Api.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO: Implement email sending (e.g., SMTP, SendGrid, etc.)
            Console.WriteLine($"Sending email to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}