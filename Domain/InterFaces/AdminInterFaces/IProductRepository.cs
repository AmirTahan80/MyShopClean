using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.InterFaces.AdminInterFaces
{
    public interface IProductRepository : ISaveInterFaces
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductAsync(int productId);
        Task<ProductImages> FindImageByIdAsync(int imageId);
        Task<ProductProperty> FindPropertyByIdAsync(int propertyId);



        Task AddProductAsync(Product t);
        Task AddProductImagesAsync(IEnumerable<ProductImages> t);
        Task AddProductPropertiesAsync(IEnumerable<ProductProperty> t);

        Task AddProductAdjectivesAsync(IEnumerable<Adjective> t);
        Task AddProductAdjectivesAsync(Adjective t);
        Task AddProductAdjectiveValuesAsync(IEnumerable<Value> t);
        Task AddProductAdjectiveValuesAsync(Value t);
        Task AddProductAdjectiveValueSubValueAsync(IEnumerable<SubValue> t);
        Task AddProductAdjectiveValueSubValueAsync(SubValue t);

        void DeleteProduct(Product t);
        void DeleteProduct(IEnumerable<Product> t);
        void DeletePhoto(ProductImages t);
        void DeletePhoto(IEnumerable<ProductImages> t);
        void DeleteProductProperties(ProductProperty t);
        void DeleteProductProperties(IEnumerable<ProductProperty> t);
        void EditProduct(Product t);
    }
}
