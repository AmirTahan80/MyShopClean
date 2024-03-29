using Application.InterFaces.Admin;
using Application.Utilities;
using Application.ViewModels.Admin;
using InstagramApiSharp.Classes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Manager")]
    public class ProductManagerController : Controller
    {
        #region Injections
        private readonly IProudctServices _porudctServices;
        private readonly IInstagramBotServices _instagramBotServices;
        public ProductManagerController(IProudctServices porudctServices, IInstagramBotServices instagramBotServices)
        {
            _porudctServices = porudctServices;
            _instagramBotServices = instagramBotServices;
        }

        #endregion

        public async Task<IActionResult> Index(int? pageNumber, string search = "", string filter = "")
        {
            var products = await _porudctServices.GetAllProductsAsync();

            ViewBag.Filter = "newest";
            ViewBag.FilterName = "جدید ترین";

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p => p.Name.Contains(search) || p.CategoryName.Contains(search));
                ViewBag.Search = search;
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "newest":
                        products = products.OrderByDescending(p => p.Id);
                        ViewBag.FilterName = "جدید ترین";
                        ViewBag.Filter = "newest";
                        break;
                    case "older":
                        products = products.OrderBy(p => p.Id);
                        ViewBag.FilterName = "قدیمی ترین";
                        ViewBag.Filter = "older";
                        break;
                    case "lower10":
                        products = products.Where(p => p.Count <= 10);
                        ViewBag.FilterName = "کمتر از 10";
                        ViewBag.Filter = "lower10";
                        break;
                    case "lower20":
                        products = products.Where(p => p.Count <= 20);
                        ViewBag.FilterName = "کمتر از 20";
                        ViewBag.Filter = "lower20";
                        break;
                    case "finish":
                        products = products.Where(p => p.Count <= 1);
                        ViewBag.FilterName = "اتمام یافته";
                        ViewBag.Filter = "finish";
                        break;
                    case "expensive":
                        products = products.OrderByDescending(p => p.Price);
                        ViewBag.FilterName = "گران ترین";
                        ViewBag.Filter = "expensive";
                        break;
                    case "cheaper":
                        products = products.OrderBy(p => p.Price);
                        ViewBag.FilterName = "ارزان ترین";
                        ViewBag.Filter = "cheaper";
                        break;
                }
                ViewBag.FilterValue = filter;
            }

            var paging = new PagingList<GetProductsAndImageSrcViewModel>(products, 10, pageNumber ?? 1);
            var productsPaging = paging.QueryResult;


            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "Index";
            ViewBag.Controller = "ProductManager";
            #endregion



            return View(productsPaging);
        }
      
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var categoriesTreeView = await _porudctServices.GetCategoriesTreeViewForAdd();
            return View(categoriesTreeView);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddProductViewModel model)
        {
            var categoriesTreeView = await _porudctServices.GetCategoriesTreeViewForAdd();
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "تمامی مقادیر را پر کنید!";
                return View(categoriesTreeView);
            }

            var result = await _porudctServices.AddProductAsync(model);

            if (result)
                ViewData["Success"] = "ذخیره با موفقیت انجام شد!";
            else
                ViewData["Error"] = "عملیات ذخیره سازی با شکست مواجه شد!";

            model = new AddProductViewModel();

            return View(categoriesTreeView);
        }
       
        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            if (productId == 0)
                return NotFound();

            var product = await _porudctServices.GetProductAsync(productId);
            return View(product);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(GetProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var resut = await _porudctServices.EditProductAsync(model);
            if (!resut)
            {
                ViewData["Error"] = "خطایی در ویرایش محصول به وجود آمده است !!";
                return View(model);
            }
            else
                ViewData["Success"] = "ویرایش محصول با موفقیت انجام شد !!";


            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult UploadEditorFile(IFormFile upload)
        {
            var result = _porudctServices.UploadFileEditor(upload);
            return result;
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(IEnumerable<GetProductsAndImageSrcViewModel> models)
        {
            if (models == null)
            {
                TempData["Error"] = "عملیات حذف به درستی انجام نشد ! ";
                return RedirectToAction("Index");
            }
            var result = await _porudctServices.DeleteListOfProducts(models);
            if (result)
                TempData["Success"] = "عملیات حذف به درستی انجام شد !";
            else
                TempData["Error"] = "عملیات حذف به درستی انجام نشد ! ";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscount(int? pageNumber,string search="",string filter="")
        {
            var result = await _porudctServices.GetDisCountsAsync();
            result = result.OrderByDescending(p => p.Id).ToList();

            ViewBag.Filter = "newest";
            ViewBag.FilterName = "جدید ترین";

            if (!string.IsNullOrWhiteSpace(search))
            {
                result = result.Where(p => p.CodeName.Contains(search)).ToList();
                ViewBag.Search = search;
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "newest":
                        result = result.OrderByDescending(p => p.Id).ToList();
                        ViewBag.FilterName = "جدید ترین";
                        ViewBag.Filter = "newest";
                        break;
                    case "older":
                        result = result.OrderBy(p => p.Id).ToList();
                        ViewBag.FilterName = "قدیمی ترین";
                        ViewBag.Filter = "older";
                        break;
                }
                ViewBag.FilterValue = filter;
            }

            var paging = new PagingList<DiscountViewMode>(result, 10, pageNumber ?? 1);
            var discountsPaging = paging.QueryResult;


            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetDiscount";
            ViewBag.Controller = "ProductManager";
            #endregion



            return View(discountsPaging);
        }
       
        [HttpGet]
        public IActionResult CreateDiscount()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiscount(DiscountViewMode model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _porudctServices.CreateDiscountAsync(model);

            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> EditDiscount(int discountId)
        {
            var discount = await _porudctServices.GetDiscountAsync(discountId);

            return View(discount);

        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiscount(DiscountViewMode model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _porudctServices.EditDiscountAsync(model);

            if (result.Status == true)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PostProductToInstagram(int productId)
        {
            var product = await _porudctServices.GetProductAsync(productId);
            var result = await _instagramBotServices.UploadAlbumAsync(product);
            return RedirectToAction("GetPosts");
        }

        [HttpGet]
        public IActionResult LoginToInstagram()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginToInstagram(LoginToInsta loginToInsta)
        {
            var result = await _instagramBotServices.LoginToInsta(loginToInsta.UserName, loginToInsta.PassWord);
            if (result.Data)
            {
                ViewData["Success"] = "ورود به اکانت اینستاگرام با موفقیت انجام شد.";
                return View();
            }
            else
            {
                ViewData["Error"] = "ورود به اکانت اینستاگرام با شکست مواجه شد.";
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var result = await _instagramBotServices.GetPostesAsync();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> PostToProduct(string imageUri)
        {
            var categoriesTreeView = await _porudctServices.GetCategoriesTreeViewForAdd();
            var media = await _instagramBotServices.UploadPostToProduct(imageUri);
            ICollection<int> categoryIdList = new Collection<int>
            {
                20
            };
            var images = (IEnumerable<string>)media.Data.GetType().GetProperty("Images").GetValue(media.Data);
            var product = new GetProductViewModel()
            {
                Detail = (string)media.Data.GetType().GetProperty("ProductDetail").GetValue(media.Data),
                Images = images.Select(p => new GetImagesViewModel()
                {
                    ImgSrc = p
                }),
                Price = 0,
                Count = 0,
                CategoriesId = categoryIdList,
                Name = (string)media.Data.GetType().GetProperty("ProductName").GetValue(media.Data),
                IsProductHaveAttributes = false,
                Categories=categoriesTreeView.CategoriesTreeView
            };

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> PostToProduct(GetProductViewModel uploadProduct)
        {
            var addProduct = new AddProductViewModel()
            {
                Name = uploadProduct.Name,
                Detail = uploadProduct.Detail,
                Count = uploadProduct.Count,
                CategoriesId = uploadProduct.CategoriesId,
                ImagesUri = uploadProduct.ImagesSrc.ToList(),
                Price = uploadProduct.Price
            };
            var result = await _porudctServices.AddProductAsync(addProduct);

            if (result)
                ViewData["Success"] = "ذخیره با موفقیت انجام شد!";
            else
                ViewData["Error"] = "عملیات ذخیره سازی با شکست مواجه شد!";

            return RedirectToAction("GetPosts");
        }


    }
}