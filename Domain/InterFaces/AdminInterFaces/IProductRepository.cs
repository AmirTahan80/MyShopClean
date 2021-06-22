using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Domain.Models;
using Domain.InterFaces;

namespace Domain.InterFaces.AdminInterFaces
{
    public interface IProductRepository: ISaveInterFaces
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductAsync(int productId);
        Task<ProductImages> FindImageByIdAsync(int imageId);
        Task<ProductProperty> FindPropertyByIdAsync(int propertyId);



        Task AddProductAsync(Product t);
        Task AddProductImagesAsync(IEnumerable<ProductImages> t);
        Task AddProductProperties(IEnumerable<ProductProperty> t);
        void DeleteProduct(Product t);
        void DeleteProduct(IEnumerable<Product> t);
        void DeletePhoto(ProductImages t);
        void DeletePhoto(IEnumerable<ProductImages> t);
        void DeleteProductProperties(ProductProperty t);
        void DeleteProductProperties(IEnumerable<ProductProperty> t);
        void EditProduct(Product t);
    }
}
