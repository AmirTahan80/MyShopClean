using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.AdminRepositories
{
    public class ProductRepository : IProductRepository
    {
        #region DbContext
        private readonly AppWebContext _context;
        public ProductRepository(AppWebContext context)
        {
            _context = context;
        }

        #endregion
        #region Implements

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
            return products;
        }
        public async Task<Product> GetProductAsync(int productId)
        {
            var product = await _context.Products.Include(p => p.ProductImages).Include(p => p.Properties)
                .SingleOrDefaultAsync(p => p.Id == productId);
            return product;
        }
        public async Task<ProductImages> FindImageByIdAsync(int imageId)
        {
            var photo = await _context.ProductImages.SingleOrDefaultAsync(p => p.Id == imageId);
            return photo;
        }
        public async Task<ProductProperty> FindPropertyByIdAsync(int propertyId)
        {
            var property = await _context.ProductProperties.SingleOrDefaultAsync(p => p.PropertyId == propertyId);
            return property;
        }





        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddProductAsync(Product t)
        {
            await _context.Products.AddAsync(t);
        }
        public async Task AddProductImagesAsync(IEnumerable<ProductImages> t)
        {
            await _context.ProductImages.AddRangeAsync(t);
        }
        public async Task AddProductPropertiesAsync(IEnumerable<ProductProperty> t)
        {
            await _context.ProductProperties.AddRangeAsync(t);
        }
        public async Task AddProductAdjectivesAsync(IEnumerable<Adjective> t)
        {
            await _context.Adjectives.AddRangeAsync(t);
        }

        public async Task AddProductAdjectivesAsync(Adjective t)
        {
            await _context.Adjectives.AddAsync(t);
        }

        public async Task AddProductAdjectiveValuesAsync(IEnumerable<Value> t)
        {
            await _context.Values.AddRangeAsync(t);
        }

        public async Task AddProductAdjectiveValuesAsync(Value t)
        {
            await _context.Values.AddAsync(t);
        }

        public async Task AddProductAdjectiveValueSubValueAsync(IEnumerable<SubValue> t)
        {
            await _context.SubValues.AddRangeAsync(t);
        }

        public async Task AddProductAdjectiveValueSubValueAsync(SubValue t)
        {
            await _context.SubValues.AddAsync(t);
        }

        public void DeleteProduct(Product t)
        {
            _context.Products.Remove(t);
        }
        public void DeleteProduct(IEnumerable<Product> t)
        {
            _context.Products.RemoveRange(t);
        }
        public void DeletePhoto(ProductImages t)
        {
            _context.ProductImages.Remove(t);
        }
        public void DeletePhoto(IEnumerable<ProductImages> t)
        {
            _context.ProductImages.RemoveRange(t);
        }
        public void DeleteProductProperties(ProductProperty t)
        {
            _context.ProductProperties.Remove(t);
        }
        public void DeleteProductProperties(IEnumerable<ProductProperty> t)
        {
            _context.ProductProperties.RemoveRange(t);
        }
        public void EditProduct(Product t)
        {
            _context.Products.Update(t);
        }





        #endregion
    }
}
