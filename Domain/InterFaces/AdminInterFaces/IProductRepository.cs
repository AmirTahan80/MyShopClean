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
        Task AddProductAttributes(IEnumerable<ProductAttribute> t);
        Task AddProductAttribute(ProductAttribute t);
        Task AddAttributeValues(IEnumerable<AttributeValue> t);
        Task AddAttributeValue(AttributeValue t);
        Task AddAttributeTemplates(IEnumerable<AttributeTemplate> t);
        Task AddAttributeTemplate(AttributeTemplate t);


        void DeleteProduct(Product t);
        void DeleteProduct(IEnumerable<Product> t);
        void DeletePhoto(ProductImages t);
        void DeletePhoto(IEnumerable<ProductImages> t);
        void DeleteProductProperties(ProductProperty t);
        void DeleteProductProperties(IEnumerable<ProductProperty> t);
        void DeleteProductAttributeValues(IEnumerable<AttributeValue> t);
        void DeleteAttributesNamesAndtemplates(IEnumerable<ProductAttribute> tNames, IEnumerable<AttributeTemplate> tTemplates);

        void EditProduct(Product t);
    }
}
