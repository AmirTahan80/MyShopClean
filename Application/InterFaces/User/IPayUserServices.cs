using Application.ViewModels;
using Application.ViewModels.User;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface IPayUserServices
    {
        Task<ResultDto> AddRequestPayAsync(string userId);

        Task<VerificationPayViewModel> Verification(string requestPayId, string authority, string status);
    }
}
