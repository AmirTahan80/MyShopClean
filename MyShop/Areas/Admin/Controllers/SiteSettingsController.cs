using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Founder")]
    public class SiteSettingsController : Controller
    {
        private readonly ISiteSettingService _siteSettingService;

        public SiteSettingsController(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Success"] = TempData["Success"];
            return View(await _siteSettingService.GetAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SiteSettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _siteSettingService.UpdateAsync(model);
            TempData["Success"] = "تنظیمات سایت با موفقیت ذخیره شد.";
            return RedirectToAction(nameof(Index));
        }
    }
}
