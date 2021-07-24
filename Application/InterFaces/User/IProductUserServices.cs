using Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface IProductUserServices
    {
        /// <summary>
        /// گرفتن تمامی محصولات
        /// </summary>
        /// <returns>IEnumerable<GetListOfProductViewModel></returns>
        Task<IEnumerable<GetListOfProductViewModel>> GetProdctsAsync();
    }
}
