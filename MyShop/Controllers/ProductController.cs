using Application.InterFaces.User;
using Application.Utilities;
using Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyShop.Controllers
{
    public class ProductController : Controller
    {
        #region Injections
        private readonly IProductUserServices _productUserServices;
        private readonly IAccountUserServices _accountServices;
        private readonly IConfiguration _configuration;
        public ProductController(IProductUserServices productUserServices,
            IAccountUserServices accountServices, IConfiguration configuration)
        {
            _productUserServices = productUserServices;
            _accountServices = accountServices;
            _configuration = configuration;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Index(ICollection<int> categoriesId, int? pageNumber = 1, string searchProduct = "", string filter = "")
        {
            var retrunModel = await _productUserServices.GetProductsListAsync(categoriesId);
            retrunModel.Products = retrunModel.Products.OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(searchProduct))
            {
                retrunModel.Products = retrunModel.Products.Where(p => p.Name.Contains(searchProduct));
                ViewBag.SearchProduct = searchProduct;
            }

            ViewData["Filter"] = "1";
            ViewBag.FilterName = "New";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "New":
                        retrunModel.Products = retrunModel.Products.OrderByDescending(p => p.Id);
                        ViewData["Filter"] = "1";
                        ViewBag.FilterName = "New";
                        break;
                    case "Expensive":
                        retrunModel.Products = retrunModel.Products.OrderByDescending(p => p.Price);
                        ViewData["Filter"] = "2";
                        ViewBag.FilterName = "Expensive";
                        break;
                    case "Cheaper":
                        retrunModel.Products = retrunModel.Products.OrderBy(p => p.Price);
                        ViewData["Filter"] = "3";
                        ViewBag.FilterName = "Cheaper";
                        break;
                }
            }

            var paging = new Paging<GetListOfProductViewModel>(retrunModel.Products, 8, pageNumber ?? 1);
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

            if (categoriesId.Count() > 0)
            {
                if (categoriesId.Any(p => p == 0))
                {
                    categoriesId.Clear();
                    categoriesId.Add(0);
                }
            }
            ViewData["CategoryId"] = categoriesId;
            ViewBag.CategoriesId = categoriesId;


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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion(GetProductDescriptionViewModel model, string returnUrl = "")
        {
            string recaptchaResponse = this.Request.Form["g-recaptcha-response"];
            var client = HttpClientFactory.Create();
            var parameters = new Dictionary<string, string>
            {
                {"secret", _configuration["reCAPTCHA:SecretKey"]},
                {"response", recaptchaResponse},
                {"remoteip", this.HttpContext.Connection.RemoteIpAddress.ToString()}
            };

            HttpResponseMessage response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(parameters));
            response.EnsureSuccessStatusCode();

            string apiResponse = await response.Content.ReadAsStringAsync();
            dynamic apiJson = JObject.Parse(apiResponse);


            if (string.IsNullOrEmpty(model.Question.Email) || string.IsNullOrEmpty(model.Question.Text) || apiJson.success != true)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    if (apiJson.success != true)
                    {
                        ViewData["Error"] = "لطفا احراز هویت را تکمیل کنید !";
                    }
                    return Redirect(returnUrl);
                }
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
