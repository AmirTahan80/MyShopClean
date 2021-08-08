using Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface IProductUserServices
    {
        /// <summary>
        /// گرفتن محصولات برای صفحه اول 
        /// </summary>
        /// <returns>IEnumerable<GetListOfProductViewModel></returns>
        Task<ListOfProductsViewModel> GetProdctsListForIndexAsync();

        /// <summary>
        /// گرفتن محصولا برای صفحه محصولات و یا گرفتن محصولات دسته بندی خاص
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>GetListOfProductViewModel</returns>
        Task<IEnumerable<GetListOfProductViewModel>> GetProductsListAsync(int categoryId);

        /// <summary>
        /// گرفتن جزئیات محصول با آیدی
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>GetProductDescriptionViewModel</returns>
        Task<GetProductDescriptionViewModel> GetProductDescriptionAsync(int productId,string userId="");

    }
}
