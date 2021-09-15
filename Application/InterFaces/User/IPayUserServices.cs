using Application.ViewModels;
using Application.ViewModels.User;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface IPayUserServices
    {
        Task<ResultDto> AddRequestPayZarinPallAsync(string userId);
        Task<ResultDto> AddRquestPayIdPayAsync(string userId);

        Task<VerificationPayViewModel> VerificationZarinPall(string requestPayId, string authority, string status);
        Task<VerificationPayViewModel> VerificationIdPay(GetResponseIdPayValueViewModel response);
    }
}
