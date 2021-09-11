using Application.InterFaces.Admin;
using Application.Utilities;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Writer")]
    public class CategoryManagerController : Controller
    {
        #region Ingections
        private readonly ICategoryServices _categoryServices;
        public CategoryManagerController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber,string search="",string filter="")
        {
            var categories = await _categoryServices.GetAllCategoriesAsync();

            var categoriesSearch=new List<GetCategoryViewModel>();

            if(!string.IsNullOrWhiteSpace(search))
            {
                categories = categories.Where(p => p.Name.Contains(search) || (p.Parent!=null?p.Parent.Name.Contains(search) :p.Name.Contains(search))).ToList();
                
                ViewBag.Search = search;
            }

            ViewBag.Filter = "newest";
            ViewBag.FilterName = "جدید ترین";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "newest":
                        categories = categories.OrderByDescending(p => p.Id).ToList();
                        ViewBag.Filter = "newest";
                        ViewBag.FilterName = "جدید ترین";
                        break;
                    case "older":
                        categories = categories.OrderBy(p => p.Id).ToList();
                        ViewBag.Filter = "older";
                        ViewBag.FilterName = "قدیمی ترین";
                        break;                    
                    case "parent":
                        categories = categories.Where(p => p.Parent!=null).ToList();
                        ViewBag.Filter = "parent";
                        ViewBag.FilterName = "دسته های پدر";
                        break;
                    case "sub":
                        categories = categories.Where(p => p.Parent==null).ToList();
                        ViewBag.Filter = "sub";
                        ViewBag.FilterName = "دسته های فرزند";
                        break;
                }
            }

            var paging = new PagingList<GetCategoryViewModel>(categories, 10, pageNumber ?? 1);
            var categoriesPaging = paging.QueryResult;


            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "Index";
            ViewBag.Controller = "CategoryManager";
            #endregion

            ViewData["Error"] = TempData["Error"];
            ViewData["Success"] = TempData["Success"];

            return View(categoriesPaging);
        }
        [HttpGet]
        public async Task<IActionResult> AddCategory()
        {
            var categories = await _categoryServices.GetCategoriesTreeForAdd();
            return View(categories);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _categoryServices.AddCategoryAsync(model);
            if (!result)
                ViewData["Error"] = "خطایی در افزودن دسته بندی پیش آمده است";
            else
                ViewData["Success"] = "افزودن دسته بندی با موفقیت انجا شد";

            model = null;
            var categories = await _categoryServices.GetCategoriesTreeForAdd();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> EditCategory(int categoryId)
        {
            var category = await _categoryServices.GetCategoryAsync(categoryId);
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(GetCategoryViewModel model)
        {
            if(!ModelState.IsValid)
            {
                ViewData["Error"] = "مقادیر نام نمیتواند خالی باشد";
                return View(model);
            }

            var result = await _categoryServices.EditCategoryAsync(model);

            if (!result)
                ViewData["Error"] = "در ویرایش دسته بندی خطایی پیش آمده است";
            else
                ViewData["Success"] = "ویرایش دسته بندی با موفقیت انجام شد";

            var category = await _categoryServices.GetCategoryAsync(model.Id);
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(IEnumerable<GetCategoryViewModel> models)
        {
            if (models.Count() <= 0) return NotFound();
            var result = await _categoryServices.DeleteCategoryAsync(models);

            if (!result)
                TempData["Error"] = "در حذف دسته بندی خطایی پیش آمده است";
            else
                TempData["Success"] = "حذف دسته بندی با موفقیت انجام شد";

            return RedirectToAction("Index");
        }
    }
}
