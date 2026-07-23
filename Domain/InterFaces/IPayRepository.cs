using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface IPayRepository : ISaveInterFaces
    {
        Task<IEnumerable<RequestPay>> GetRequestPaiesAsync();
        Task<IEnumerable<Factor>> GetFactors();
        Task<RequestPay> GetRequestPayAsync(string requestPayId);
        Task<Factor> GetFactorAsync(int factorId, string userId);
        Task<Factor> GetFactorByCartAsync(int cartId, string userId);
        Task<Factor> FinalizePaymentAsync(string requestPayId, int referenceId);

        Task AddRequestPay(RequestPay t);
        Task AddFactor(Factor t);
        Task AddFactorDetails(IEnumerable<FactorDetail> t);
    }
}
