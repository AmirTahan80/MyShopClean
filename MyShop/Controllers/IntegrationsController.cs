using Application.InterFaces.Admin;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyShop.Controllers
{
    [ApiController]
    [Route("integrations")]
    public class IntegrationsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ISiteSettingService _siteSettingService;

        public IntegrationsController(IProductRepository productRepository, ISiteSettingService siteSettingService)
        {
            _productRepository = productRepository;
            _siteSettingService = siteSettingService;
        }

        [HttpGet("torob/products")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> TorobProducts()
        {
            var setting = await _siteSettingService.GetAsync();
            var accessResult = await ValidateAccessAsync(
                setting.TorobEnabled,
                await _siteSettingService.IsTorobTokenValidAsync(Request.Headers["X-Integration-Token"]));
            if (accessResult != null) return accessResult;

            var products = await BuildProductsAsync(setting.PublicBaseUrl);
            return Ok(new { count = products.Count, products });
        }

        [HttpGet("emalls/products")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> EmallsProducts()
        {
            var setting = await _siteSettingService.GetAsync();
            var accessResult = await ValidateAccessAsync(
                setting.EmallsEnabled,
                await _siteSettingService.IsEmallsTokenValidAsync(Request.Headers["X-Integration-Token"]));
            if (accessResult != null) return accessResult;

            var products = await BuildProductsAsync(setting.PublicBaseUrl);
            return Ok(new { count = products.Count, products });
        }

        [HttpGet("emalls/products.xml")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> EmallsProductsXml()
        {
            var setting = await _siteSettingService.GetAsync();
            var accessResult = await ValidateAccessAsync(
                setting.EmallsEnabled,
                await _siteSettingService.IsEmallsTokenValidAsync(Request.Headers["X-Integration-Token"]));
            if (accessResult != null) return accessResult;

            var products = await BuildProductsAsync(setting.PublicBaseUrl);
            var document = new XDocument(
                new XElement("products",
                    new XAttribute("generated_at", DateTimeOffset.UtcNow.ToString("O")),
                    products.Select(product => new XElement("product",
                        new XElement("product_id", product.product_id),
                        new XElement("title", product.title),
                        new XElement("price", product.price),
                        new XElement("currency", product.currency),
                        new XElement("availability", product.availability),
                        new XElement("page_url", product.page_url),
                        new XElement("image_url", product.image_url),
                        new XElement("description", product.description)))));

            return Content(document.ToString(), "application/xml; charset=utf-8");
        }

        private Task<IActionResult> ValidateAccessAsync(bool enabled, bool tokenIsValid)
        {
            if (!enabled) return Task.FromResult<IActionResult>(NotFound());
            if (!tokenIsValid)
            {
                return Task.FromResult<IActionResult>(Unauthorized());
            }

            return Task.FromResult<IActionResult>(null);
        }

        private async Task<List<MarketplaceProduct>> BuildProductsAsync(string configuredBaseUrl)
        {
            var requestBaseUrl = $"{Request.Scheme}://{Request.Host}";
            var baseUrl = string.IsNullOrWhiteSpace(configuredBaseUrl) ? requestBaseUrl : configuredBaseUrl.TrimEnd('/');
            var products = await _productRepository.GetAllProductsAsync();

            return products.Select(product =>
            {
                var image = product.ProductImages?.FirstOrDefault();
                var imagePath = image == null
                    ? "/Images/ProductImages/placeholder.svg"
                    : $"/Images/ProductImages/{(string.IsNullOrWhiteSpace(image.ImgFile) ? string.Empty : image.ImgFile.Trim('/') + "/")}{image.ImgSrc}";

                return new MarketplaceProduct
                {
                    product_id = product.Id.ToString(),
                    title = product.Name,
                    price = product.Price,
                    currency = "TOMAN",
                    old_price = null,
                    availability = product.Count > 0 ? "instock" : "outofstock",
                    page_url = $"{baseUrl}/Product/Description?productId={product.Id}",
                    image_url = $"{baseUrl}{imagePath}",
                    description = product.Detail,
                    stock = product.Count
                };
            }).ToList();
        }

        private sealed class MarketplaceProduct
        {
            public string product_id { get; set; }
            public string title { get; set; }
            public int price { get; set; }
            public string currency { get; set; }
            public int? old_price { get; set; }
            public string availability { get; set; }
            public string page_url { get; set; }
            public string image_url { get; set; }
            public string description { get; set; }
            public int stock { get; set; }
        }
    }
}
