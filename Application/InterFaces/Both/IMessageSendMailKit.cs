using System.Threading.Tasks;

namespace Application.InterFaces.Both
{
    public interface IMessageSendMailKit
    {
        Task SendMessageAsync(string toEmail, string userName, string message,string subject);
    }
}
