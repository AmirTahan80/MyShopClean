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
        private readonly IAccountUserServices _accountServices;
        public ProductController(IProductUserServices productUserServices,
            IAccountUserServices accountServices)
        {
            _productUserServices = productUserServices;
            _accountServices = accountServices;
        }
        #endregion

        public async Task<IActionResult> Index(int categoryId = 0, int? pageNumber = 1, string searchProduct = "", string filter = "")
        {
            var retrunModel = await _productUserServices.GetProductsListAsync(categoryId);
            retrunModel.Products = retrunModel.Products.OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(searchProduct))
            {
                retrunModel.Products = retrunModel.Products.Where(p => p.Name.Contains(searchProduct));
                ViewBag.SearchProduct = searchProduct;
            }

            ViewData["Filter"] = "1";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "New":
                        retrunModel.Products = retrunModel.Products.OrderByDescending(p => p.Id);
                        ViewData["Filter"] = "1";
                        break;
                    case "Expensive":
                        retrunModel.Products = retrunModel.Products.OrderByDescending(p => p.Price);
                        ViewData["Filter"] = "2";
                        break;
                    case "Cheaper":
                        retrunModel.Products = retrunModel.Products.OrderBy(p => p.Price);
                        ViewData["Filter"] = "3";
                        break;
                }
            }

            var paging = new Paging<GetListOfProductViewModel>(retrunModel.Products, 6, pageNumber ?? 1);
            retrunModel.Products = paging.QueryResult;


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

            ViewData["CategoryId"]= categoryId;



            return View(retrunModel);
        }

        [HttpGet]
        public async Task<IActionResult> Description(int productId = 0)
        {
            if (productId == 0) return NotFound();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _productUserServices.GetProductDescriptionAsync(productId, userId);
            if (result == null) return NotFound();

            ViewData["Success"] = TempData["Success"];
            ViewData["Error"] = TempData["Error"];

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion(GetProductDescriptionViewModel model, string returnUrl = "")
        {
            if (string.IsNullOrEmpty(model.Question.Email) || string.IsNullOrEmpty(model.Question.Text))
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return NotFound();
            }

            var result = await _accountServices.AskQuestionAsync(model.Question);

            if (result.Status)
            {
                TempData["Success"] = result.SuccesMessage;
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return View();
        }
    }
}
