using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterFaces.Both
{
    public interface IMessageSenderServices
    {
        Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
    }
}
