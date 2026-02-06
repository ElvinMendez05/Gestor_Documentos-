using Document_Manager.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Document_Manager.Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = _config["EmailSettings:Host"],
                Port = int.Parse(_config["EmailSettings:Port"]!),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]
                ),
                EnableSsl = true 
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:From"]!, "Document Manager"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await smtp.SendMailAsync(mail);
        }
    }
}
