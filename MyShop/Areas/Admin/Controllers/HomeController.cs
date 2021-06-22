using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        #region Context,IOC
        //private readonly IIndexManagerRepository _indexManager;
        //public HomeController(IIndexManagerRepository indexManager)
        //{
        //    _indexManager = indexManager;
        //}
        #endregion
        public IActionResult Index()
        {
            return View();
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