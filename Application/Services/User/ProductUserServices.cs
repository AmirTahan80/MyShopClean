using Application.InterFaces.User;
using Application.ViewModels.User;
using Domain.InterFaces.AdminInterFaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class ProductUserServices : IProductUserServices
    {
        private readonly IProductRepository _productRepository;
        public ProductUserServices(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        //Implements
        public async Task<ListOfProductsViewModel> GetProdctsListForIndexAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var returnProduct = new ListOfProductsViewModel()
            {
                CheapestProducts=products.OrderBy(p=>p.Price).Take(10).Select(p=>new GetListOfProductViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Count = p.Count,
                    Price = p.Price,
                    ImageSrc = p.ProductImages.FirstOrDefault().ImgFile + "/" + p.ProductImages.FirstOrDefault().ImgSrc
                }),
                MostSalerProducts = products.OrderBy(p => p.Count).Take(10).Select(p => new GetListOfProductViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Count = p.Count,
                    Price = p.Price,
                    ImageSrc = p.ProductImages.FirstOrDefault().ImgFile + "/" + p.ProductImages.FirstOrDefault().ImgSrc
                }),
                NewstProducts = products.OrderByDescending(p => p.Id).Take(10).Select(p => new GetListOfProductViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Count = p.Count,
                    Price = p.Price,
                    ImageSrc = p.ProductImages.FirstOrDefault().ImgFile + "/" + p.ProductImages.FirstOrDefault().ImgSrc
                })
            };

            return returnProduct;
        }

        public async Task<IEnumerable<GetListOfProductViewModel>> GetProductsListAsync(int categoryId)
        {
            var products = await _productRepository.GetAllProductsAsync();

            if(categoryId!=0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
            }
            var returnCorrentProduct = products.Select(p => new GetListOfProductViewModel()
            {
                Id=p.Id,
                Name=p.Name,
                Count=p.Count,
                ImageSrc=p.ProductImages.FirstOrDefault().ImgFile+"/"+p.ProductImages.FirstOrDefault().ImgSrc,
                Price=p.Price
            });

            return returnCorrentProduct;
        }

        public async Task<GetProductDescriptionViewModel> GetProductDescriptionAsync(int productId)
        {
            var product = await _productRepository.GetProductAsync(productId);
            if (product == null)
                return null;

            var productReturn = new GetProductDescriptionViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Count = product.Count,
                Price = product.Price,
                Images = product.ProductImages.Select(p => new ProductImageViewModel()
                {
                    ImgSrc = p.ImgFile + "/" + p.ImgSrc,
                }),
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Description = product.Detail,
                IsProductHaveAttributes = product.IsProductHaveAttributes,
                PropertyName = product.Properties.Select(p => p.ValueName).ToList(),
                PropertyValue = product.Properties.Select(p => p.ValueType).ToList(),
                Attributes = product.IsProductHaveAttributes == true ? product.ProductAttributes.Select(p => new ProductAttributeViewModel()
                {
                    AttributesName = p.AttributeName,
                    AttributesValue = p.AttributeValues.Select(p => new SelectListItem()
                    {
                        Value = p.ValueId.ToString(),
                        Text = p.ValueName
                    })
                }).ToList() : null,
                Templates = product.AttributeTemplates.Select(p => new ProductAttributesTemplate()
                {
                    Template = p.Template,
                    Count = p.AttrinbuteTemplateCount,
                    Price = p.AttrinbuteTemplatePrice
                })
            };

            return productReturn;
        }

    }
}
