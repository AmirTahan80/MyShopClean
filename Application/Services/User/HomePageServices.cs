using Application.InterFaces.User;
using Application.ViewModels.User;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class HomePageServices : IHomePageServices
    {
        #region Injections
        private readonly IBanerRepository _banerRepository;
        private readonly IProductRepository _productRepository;
        public HomePageServices(IBanerRepository banerRepository, IProductRepository productRepository)
        {
            _banerRepository = banerRepository;
            _productRepository = productRepository;
        }
        #endregion
        
        public async Task<HomePageViewModel> GetBanersAndProductsAsync()
        {
            var baners = await _banerRepository.GetBanersAsync();
            var products = await _productRepository.GetAllProductsAsync();
            var newProducts = products.OrderByDescending(p => p.Id).ToList();
            var cheapProduct = products.OrderBy(p => p.Price).ToList();
            var mostSalerProduct = products.OrderByDescending(p => p.Id).ToList();

            string GetImagePath(Domain.Models.Product product)
            {
                var image = product.ProductImages?.FirstOrDefault();
                if (image == null)
                {
                    return "placeholder.svg";
                }

                return string.IsNullOrWhiteSpace(image.ImgFile)
                    ? image.ImgSrc
                    : $"{image.ImgFile}/{image.ImgSrc}";
            }

            var banersReturn = new HomePageViewModel()
            {
                Baners = baners.Select(p => new BanerViewModel()
                {
                    Text = p.Text,
                    Link = p.Link,
                    ImageSrc = p.Image,
                    ImageLocation = p.BanerPlace
                }).ToList(),
                CheapestProducts = cheapProduct.Select(p=> new GetListOfProductViewModel()
                {
                    Id=p.Id,
                    Count=p.Count,
                    ImageSrc = GetImagePath(p),
                    Name=p.Name,
                    Price=p.Price
                }).ToList(),
                NewtProducts=newProducts.Select(p=> new GetListOfProductViewModel()
                {
                    Id = p.Id,
                    Count = p.Count,
                    ImageSrc = GetImagePath(p),
                    Name = p.Name,
                    Price = p.Price
                }).ToList(),
                MostSalerProducts= mostSalerProduct.Select(p => new GetListOfProductViewModel()
                {
                    Id = p.Id,
                    Count = p.Count,
                    ImageSrc = GetImagePath(p),
                    Name = p.Name,
                    Price = p.Price
                }).ToList()
            };

            return banersReturn;
        }
    }
}
