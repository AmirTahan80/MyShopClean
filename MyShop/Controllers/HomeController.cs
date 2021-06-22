using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace Controllers
{
    public class HomeController : Controller
    {
        #region InterFaces

        private readonly ILogger<HomeController> _logger;
        //private readonly IGetProductRepository _iGetProductRepository;
        //private readonly IGetIndexImageRepository _indexRepository;
        //private readonly IGetContactRepository _getContactRepository;
        //private readonly IGetCategoryRepository _getCategoryRepository;
        public HomeController(/*IGetIndexImageRepository indexRepository,
            IGetContactRepository getContactRepository,
            IGetProductRepository iGetProductRepository,
            IGetCategoryRepository getCategoryRepository,*/ILogger<HomeController> logger)
        {
            //_iGetProductRepository = iGetProductRepository;
            //_indexRepository = indexRepository;
            //_getContactRepository = getContactRepository;
            //_getCategoryRepository = getCategoryRepository;
            _logger = logger;
        }
        #endregion
        public IActionResult Index(long CatId=0)
        {
            //var images = await _indexRepository.GetImageAsync();
            //var contactPhone = await _getContactRepository.GetContactAsync();
            //var model = await _iGetProductRepository.GetProductsAsync(CatId);
            //var categories = await _getCategoryRepository.GetAllParentAsync();
            //var vm = new GetProductsForUserViewModel()
            //{
            //    HomePage = new HomePageViewModel()
            //    {
            //        IndexImages = images,
            //        Products = model.Products,
            //        AdminPhone = contactPhone.AdminPhoneNumber,
            //    },
            //    Categories = categories,
            //};
            //if(vm!=null)
            //return View(vm);

            return View();
        }
        public IActionResult Privacy()
        {
            //var privacy = await _indexRepository.GetPrivacySettingAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Errors(string errCode)
        {
            if (errCode == "500" | errCode == "404")
            {
                return View($"~/Views/Home/Errors/{errCode}.cshtml");
            }

            return View("~/Views/Shared/Errors.cshtml");
        }
    }
}
