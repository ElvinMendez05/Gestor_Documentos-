using Document_Manager.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Document_Manager.Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress("no-reply@documentmanager.com");

            var smtp = new SmtpClient(
                config["EmailSettings:Host"],
                int.Parse(config["EmailSettings:Port"])
            )
            {
                Credentials = new NetworkCredential(
                    config["EmailSettings:Username"],
                    config["EmailSettings:Password"]
                ),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
