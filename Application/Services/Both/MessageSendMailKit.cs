using Application.InterFaces.Both;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace Application.Services.Both
{
    public class MessageSendMailKit : IMessageSendMailKit
    {
        #region Injection
        private readonly IConfiguration _configuration;
        public MessageSendMailKit(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
        public async Task SendMessageAsync(string toEmail, string userName, string message, string subject)
        {
            MimeMessage ms = new MimeMessage();

            MailboxAddress from = new MailboxAddress(_configuration["MailSettings:DisplayName"], _configuration["MailSettings:Mail"]);
            ms.From.Add(from);

            MailboxAddress to = new MailboxAddress(userName, toEmail);
            ms.To.Add(to);

            ms.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;

            ms.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            await client.ConnectAsync(_configuration["MailSettings:Host"], 587);
            await client.AuthenticateAsync(_configuration["MailSettings:UserName"], _configuration["MailSettings:Password"]);

            await client.SendAsync(ms);
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }
}
