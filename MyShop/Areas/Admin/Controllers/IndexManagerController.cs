﻿using Application.InterFaces.Admin;
using Application.Utilities;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Writer")]
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
        public async Task<IActionResult> Index(string search, int? pageNumber, string filter)
        {
            var baners = await _banerServices.GetBanersAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                baners = baners.Where(p => p.Link.Contains(search) || p.Text.Contains(search)).ToList();

                ViewBag.Search = search;
            }

            ViewBag.Filter = "newest";
            ViewBag.FilterName = "جدید ترین";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "slider":
                        baners = baners.Where(p => p.ImageLocation == "Slider").ToList();
                        ViewBag.Filter = "slider";
                        ViewBag.FilterName = "اسلایدر";
                        break;
                    case "newest":
                        baners = baners.OrderByDescending(p => p.Id).ToList();
                        ViewBag.Filter = "newest";
                        ViewBag.FilterName = "جدید ترین";
                        break;
                    case "older":
                        baners = baners.OrderBy(p => p.Id).ToList();
                        ViewBag.Filter = "older";
                        ViewBag.FilterName = "قدیمی ترین";
                        break;
                }
            }


            var paging = new PagingList<BanerViewModel>(baners, 10, pageNumber ?? 1);
            var banersPaging = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "Index";
            ViewBag.Controller = "IndexManager";
            #endregion

            ViewData["Error"] = TempData["Error"];
            ViewData["Success"] = TempData["Success"];

            return View(baners);
        }

        [HttpGet]
        public IActionResult AddBaner()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBaner(BanerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _banerServices.AddBanerAsync(model);

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
        public async Task<IActionResult> EditBaner(int banerId)
        {
            var baner = await _banerServices.GetBanerAsync(banerId);

            return View(baner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBaner(BanerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _banerServices.EditBanerAsync(model);

            if (result.Status == true)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            var baner = await _banerServices.GetBanerAsync(model.Id);

            return View(baner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBaner(IList<BanerViewModel> models)
        {
            var result = await _banerServices.DeleteBanersAsync(models);

            if (result.Status)
            {
                TempData["Success"] = result.SuccesMessage;
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Index");
        }
    }
}
