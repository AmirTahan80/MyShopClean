﻿using Application.InterFaces.Both;
using System.Net;
using System.Net.Mail;
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
                    UserName = "amirhosin6402",
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
                    From = new MailAddress("amirhosin6402@gmail.com"),
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
