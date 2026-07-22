using Application.InterFaces.Both;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Application.Services.Both
{
    public class MessageSenderServices : IMessageSenderServices
    {
        private readonly IConfiguration _configuration;

        public MessageSenderServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
        {
            var senderEmail = GetRequiredSetting("MailSettings:Mail");
            var host = GetRequiredSetting("MailSettings:Host");
            var userName = GetRequiredSetting("MailSettings:UserName");
            var password = GetRequiredSetting("MailSettings:Password");
            var port = _configuration.GetValue("MailSettings:Port", 587);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_configuration["MailSettings:DisplayName"] ?? "My Shop", senderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(isMessageHtml ? "html" : "plain") { Text = message };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(userName, password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }

        private string GetRequiredSetting(string key)
        {
            var value = _configuration[key];
            return !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new InvalidOperationException($"Configuration value '{key}' is required to send email.");
        }
    }
}
