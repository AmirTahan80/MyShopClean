using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize("Manager")]
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
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryServices.GetAllCategoriesAsync();
            ViewData["Error"] = TempData["Error"];
            ViewData["Success"] = TempData["Success"];
            return View(categories);
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
