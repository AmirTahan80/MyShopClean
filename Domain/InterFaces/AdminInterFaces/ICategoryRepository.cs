
using Domain.InterFaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.InterFaces.AdminInterFaces
{
    public interface ICategoryRepository:ISaveInterFaces
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryAsync(int categoryId);

        Task AddCategoryAsync(Category t);
        void DeleteCategory(Category t);
        Task DeleteCategoryByIdAsync(int categoryId);
        void DeleteCategoryRange(IEnumerable<Category> t);
        void UpdateCategory(Category t);
    }
}
