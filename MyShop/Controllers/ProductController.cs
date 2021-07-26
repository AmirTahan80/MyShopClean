using Application.InterFaces.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.Controllers
{
    public class ProductController : Controller
    {
        #region Injections
        private readonly IProductUserServices _productUserServices;
        public ProductController(IProductUserServices productUserServices)
        {
            _productUserServices = productUserServices;
        }
        #endregion

        public async Task<IActionResult> Index(int categoryId=0)
        {
            var products = await _productUserServices.GetProductsListAsync(categoryId);
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Description(int productId=0)
        {
            if (productId == 0) return NotFound();

            var result = await _productUserServices.GetProductDescriptionAsync(productId);
            if (result == null) return NotFound();
            
            return View(result);
        }
    }
}
