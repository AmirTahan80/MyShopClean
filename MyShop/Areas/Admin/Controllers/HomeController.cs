using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Writer")]
    public class HomeController : Controller
    {
        #region Context,IOC
        private readonly IAdminIndexManagerServices _adminIndexManager;
        public HomeController(IAdminIndexManagerServices adminIndexManager)
        {
            _adminIndexManager = adminIndexManager;
        }
        #endregion
        public async Task<IActionResult> Index()
        {
            int todayUsersCount = await _adminIndexManager.GetTodayUserRegisterAsync();
            int usersCount = await _adminIndexManager.GetUserCountAsync();
            int todayProductsCount = await _adminIndexManager.GetTodayProductCreateAsync();
            int productsCount = await _adminIndexManager.GetProductCountAsync();
            int todayFactorsCount = await _adminIndexManager.GetTodayFactorsAsync();
            int todayCommentsCount = await _adminIndexManager.GetTodayCommentsAsync();
            int todayQuestionsCount = await _adminIndexManager.GetTodayQuestionsAsync();
            var factors = await _adminIndexManager.GetFactorsAsync();
            var model = new AdminIndexViewModel()
            {
                TodayUserCount = todayUsersCount,
                UserCount = usersCount,
                TodayProductCount = todayProductsCount,
                ProductCount = productsCount,
                TodayCommentsCount = todayCommentsCount,
                TodayQuestionsCount = todayQuestionsCount,
                TodayFactorsCount = todayFactorsCount,
                Factors= factors
            };
            return View(model);
        }
        //[HttpGet]
        //[Authorize("Writer")]
        //public async Task<IActionResult> Privacy()
        //{
        //    var privacy = await _indexManager.GetPrivacyAsync();
        //    return View(privacy);
        //}
        //[HttpGet]
        //public IActionResult PrivacyAdd()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> PrivacyAdd(PrivacySettingViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewData["Error"] = "لطفا مقادیر را خالی نزارید";
        //        return View(model);
        //    }
        //    var result = await _indexManager.CreatePrivacyAsync(model);
        //    if (result == false)
        //    {
        //        ViewData["Error"] = "لطفا مقادیر را خالی نزارید";
        //        return View(model);
        //    }
        //    ViewData["Success"] = "عملیات با موفقیت انجام شد";
        //    return View(model);
        //}
        //[HttpPost]
        //public async Task<IActionResult> PrivacyDelete(int Id, string returnUrl)
        //{
        //    var result = await _indexManager.DeletePrivacyAsync(Id);
        //    if (result == false)
        //    {
        //        return NotFound();
        //    }
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return Ok();
        //    }
        //}
        //[HttpGet]
        //public async Task<IActionResult> PrivacyEdit(int Id)
        //{
        //    var privacy = await _indexManager.GetPrivacyAsync(Id);
        //    return View(privacy);
        //}
        //[HttpPost]
        //public async Task<IActionResult> PrivacyEdit(PrivacySettingViewModel model)
        //{
        //    var result = await _indexManager.EditPrivacyAsync(model);
        //    if (result == false)
        //    {
        //        ViewData["Error"] = "خطایی پیش آمده لطفا مقادیر را خالی نزارید";
        //        return View(model);
        //    }
        //    ViewData["Success"] = "عملیات با موفقیت انجام شد";
        //    return View(model);
        //}
        //[HttpGet]
        //public async Task<IActionResult> PrivacyDetail(int Id)
        //{
        //    var privacy = await _indexManager.GetPrivacyAsync(Id);
        //    return View(privacy);
        //}


    }
}