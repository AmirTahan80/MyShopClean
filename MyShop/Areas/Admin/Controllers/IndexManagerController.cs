using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IndexManagerController : Controller
    {

        #region Injections
        private readonly IBanerServices _banerServices;
        public IndexManagerController(IBanerServices banerServices)
        {
            _banerServices = banerServices;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var baners = await _banerServices.GetBanersAsync();

            return View(baners);
        }

        [HttpGet]
        public IActionResult AddBaner()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddBaner(BanerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _banerServices.AddBanerAsync(model);

            if(result.Status)
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
        public async Task<IActionResult> EditBaner(int banerId)
        {
            var baner = await _banerServices.GetBanerAsync(banerId);

            return View(baner);
        }
        [HttpPost]
        public async Task<IActionResult> EditBaner(BanerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _banerServices.EditBanerAsync(model);

            if(result.Status==true)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            return View(model);
        }
    }
}
