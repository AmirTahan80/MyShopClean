using Application.InterFaces.Admin;
using Application.Utilities;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize("Writer")]
    public class ProductManagerController : Controller
    {
        #region Injections
        private readonly IProudctServices _porudctServices;
        public ProductManagerController(IProudctServices porudctServices)
        {
            _porudctServices = porudctServices;
        }

        #endregion

        public async Task<IActionResult> Index(int? pageNumber,string searchProduct="",string filter="")
        {
            var products = await _porudctServices.GetAllProductsAsync();

            ViewBag.Filter = "جدید ترین";

            if(!string.IsNullOrWhiteSpace(searchProduct))
            {
                products = products.Where(p => p.Name.Contains(searchProduct) || p.CategoryName.Contains(searchProduct));
                ViewBag.SearchProduct = searchProduct;
            }
            if(!string.IsNullOrWhiteSpace(filter))
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
                    case "lower10":
                        products = products.Where(p => p.Count <= 10);
                        ViewBag.Filter = "کمتر از 10";
                        break;
                    case "lower20":
                        products = products.Where(p => p.Count <= 20);
                        ViewBag.Filter = "کمتر از 20";
                        break;
                    case "finish":
                        products = products.Where(p => p.Count <= 1);
                        ViewBag.Filter = "اتمام یافته";
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
            }

            var paging = new PagingList<GetProductsAndImageSrcViewModel>(products,10, pageNumber?? 1);
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
        public async Task<IActionResult> EditProduct(GetProductViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var resut = await _porudctServices.EditProductAsync(model);
            if (!resut)
                ViewData["Error"] = "خطایی در ویرایش محصول به وجود آمده است !!";
            else
                ViewData["Success"] = "ویرایش محصول با موفقیت انجام شد !!";

            var product = await _porudctServices.GetProductAsync(model.Id);
            return View(product);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult UploadEditorFile(IFormFile upload)
        {
            var result = _porudctServices.UploadFileEditor(upload);
            return result;
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(IEnumerable<GetProductsAndImageSrcViewModel> models)
        {
            if(models==null)
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


    }
}