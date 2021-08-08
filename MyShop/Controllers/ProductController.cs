using Application.InterFaces.User;
using Application.Utilities;
using Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
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

        public async Task<IActionResult> Index(int categoryId = 0, int? pageNumber = 1, string searchProduct = "", string filter = "")
        {
            var products = await _productUserServices.GetProductsListAsync(categoryId);
            products = products.OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(searchProduct))
            {
                products = products.Where(p => p.Name.Contains(searchProduct));
                ViewBag.SearchProduct = searchProduct;
            }

            ViewBag.Filter = "جدید ترین";
            ViewBag.FilterValue = "newest";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "newest":
                        products = products.OrderByDescending(p => p.Id);
                        ViewBag.Filter = "جدید ترین";
                        break;
                    case "older":
                        products = products.OrderBy(p => p.Id);
                        ViewBag.Filter = "قدیمی ترین";
                        break;
                    case "expensive":
                        products = products.OrderByDescending(p => p.Price);
                        ViewBag.Filter = "گران ترین";
                        break;
                    case "cheaper":
                        products = products.OrderBy(p => p.Price);
                        ViewBag.Filter = "ارزان ترین";
                        break;
                }
                ViewBag.FilterValue = filter;
            }

            var paging = new Paging<GetListOfProductViewModel>(products, 6, pageNumber ?? 1);
            var productsPaging = paging.QueryResult;


            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "Index";
            ViewBag.Controller = "Product";
            #endregion

            ViewBag.CategoryId = categoryId;

            return View(productsPaging);
        }

        [HttpGet]
        public async Task<IActionResult> Description(int productId = 0)
        {
            if (productId == 0) return NotFound();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _productUserServices.GetProductDescriptionAsync(productId,userId);
            if (result == null) return NotFound();

            return View(result);
        }
    }
}
