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
        /// <summary>
        /// گرفتن تمام دسته ها از دیتابیس
        /// Get All Categories From Data Base
        /// </summary>
        /// <returns>IList<GetCategoryViewModel></returns>
        Task<IList<GetCategoryViewModel>> GetAllCategoriesAsync();
        /// <summary>
        /// یافتن و یا گرفتن یک دسته از دیتابیس از طریق آیدی دسته
        /// Fin Category From Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>GetCategoryViewModel</returns>
        Task<GetCategoryViewModel> GetCategoryAsync(int categoryId);
        /// <summary>
        /// گرفتن درختی تمام دسته ها برای ویو مدل اد برای بخش افزودن دسته جدید
        /// Get All Categories like tree View for add new category
        /// </summary>
        /// <returns>AddCategoryViewModel</returns>
        Task<AddCategoryViewModel> GetCategoriesTreeForAdd();
        /// <summary>
        /// افزودن دسته جدید
        /// Add New Category
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> AddCategoryAsync(AddCategoryViewModel model);
        /// <summary>
        /// ویرایش دسته بندی
        /// Edit Category
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> EditCategoryAsync(GetCategoryViewModel model);
        /// <summary>
        /// حذف دسته (ها) از لیست و دیتابیس
        /// Delete Category (Categories) from list and database
        /// </summary>
        /// <param name="categoriesForDelete"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> DeleteCategoryAsync(IEnumerable<GetCategoryViewModel> categoriesForDelete);
    }
}
