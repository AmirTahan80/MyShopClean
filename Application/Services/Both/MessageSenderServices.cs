using Application.InterFaces.Both;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Both
{
    public class MessageSenderServices : IMessageSenderServices
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
        {
            using (var client = new SmtpClient())
            {

                var credentials = new NetworkCredential()
                {
                    UserName = "amirhosin6405",
                    Password = "33682964"
                };

                client.Credentials = credentials;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                using var emailMessage = new MailMessage()
                {
                    To = { new MailAddress(toEmail) },
                    From = new MailAddress("amirhosin6405@gmail.com"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = isMessageHtml
                };
                client.Send(emailMessage);
            }

            return Task.CompletedTask;
        }
    }
}
