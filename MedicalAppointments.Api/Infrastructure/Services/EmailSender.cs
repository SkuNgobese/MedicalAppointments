using MedicalAppointments.Api.Infrastructure.POCOs;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MedicalAppointments.Api.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtp;
        public string SenderName => _smtp.SenderName;

        public EmailSender(IOptions<SmtpSettings> options) => 
            _smtp = options.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.User, _smtp.Password),
                EnableSsl = _smtp.EnableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_smtp.SenderEmail, SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(email);

            await client.SendMailAsync(mail);
        }
    }
}