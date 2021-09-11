using Application.ViewModels.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface IAdminIndexManagerServices
    {
        Task<int> GetTodayUserRegisterAsync();
        Task<int> GetTodayProductCreateAsync();
        Task<int> GetUserCountAsync();
        Task<int> GetProductCountAsync();
        Task<int> GetTodayCommentsAsync();
        Task<int> GetTodayQuestionsAsync();
        Task<int> GetTodayFactorsAsync();
        Task<ICollection<FactorViewModel>> GetFactorsAsync();
    }
}
