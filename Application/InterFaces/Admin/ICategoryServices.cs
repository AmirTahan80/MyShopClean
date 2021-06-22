using Microsoft.AspNetCore.Mvc.Rendering;
using Application.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface ICategoryServices
    {
        Task<IList<GetCategoryViewModel>> GetAllCategoriesAsync();
        Task<GetCategoryViewModel> GetCategoryAsync(int categoryId);
        Task<AddCategoryViewModel> GetCategoriesTreeForAdd();
        Task<bool> AddCategoryAsync(AddCategoryViewModel model);
        Task<bool> EditCategoryAsync(GetCategoryViewModel model);

        Task<bool> DeleteCategoryAsync(IEnumerable<GetCategoryViewModel> categoriesForDelete);
    }
}
