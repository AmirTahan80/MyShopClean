using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface IPayRepository : ISaveInterFaces
    {
        Task<IEnumerable<RequestPay>> GetRequestPaiesAsync();
        Task<IEnumerable<Factor>> GetFactors();

        Task AddRequestPay(RequestPay t);
        Task AddFactor(Factor t);
        Task AddFactorDetails(IEnumerable<FactorDetail> t);
    }
}
