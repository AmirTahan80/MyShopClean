using Application.InterFaces.User;
using Application.ViewModels.User;
using Domain.InterFaces.AdminInterFaces;
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
        public async Task<IEnumerable<GetListOfProductViewModel>> GetProdctsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var returnProduct = products.Select(p => new GetListOfProductViewModel()
            {
                Id=p.Id,
                Name = p.Name,
                Count = p.Count,
                Price = p.Price,
                ImageSrc = p.ProductImages.FirstOrDefault().ImgFile+"/"+p.ProductImages.FirstOrDefault().ImgSrc
            });
            return returnProduct;
        }

    }
}
