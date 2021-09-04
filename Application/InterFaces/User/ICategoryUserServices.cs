using Application.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface ICategoryUserServices
    {
        /// <summary>
        /// گرفتن تمامی دسته ها به صورت درختی
        /// </summary>
        /// <returns>GetCategoriesTreeViewViewModel</returns>
        Task<IEnumerable<GetCategoriesTreeViewViewModel>> GetCategoriesTreeViewAsync();
        Task<IEnumerable<GetCategoriesTreeViewViewModel>> GetResponsiveCategoriesAsync();
    }
}
