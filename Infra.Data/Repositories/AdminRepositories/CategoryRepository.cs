using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.AdminRepositories
{
    public class CategoryRepository : ICategoryRepository
    {
        #region DbContext
        private readonly AppWebContext _context;
        public CategoryRepository(AppWebContext context)
        {
            _context = context;
        }
        #endregion
        #region Implements
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.Include(c => c.Parent)
              .Include(c => c.Children).ToListAsync();
            return categories;
        }
        public async Task<Category> GetCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
            .Include(c => c.Parent).Include(c => c.Children)
            .SingleOrDefaultAsync(p => p.Id == categoryId);
            return category;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddCategoryAsync(Category t)
        {
            await _context.Categories.AddAsync(t);
        }

        public void UpdateCategory(Category t)
        {
            _context.Categories.Update(t);
        }

        public void DeleteCategory(Category t)
        {
            _context.Categories.Remove(t);
        }
        public void DeleteCategoryRange(IEnumerable<Category> t)
        {
            _context.Categories.RemoveRange(t);
        }

        public async Task DeleteCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(p => p.Id == categoryId);
            _context.Categories.Remove(category);
        }

        #endregion
    }
}
