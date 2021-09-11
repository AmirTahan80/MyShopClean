using Application.InterFaces.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Controllers
{
    public class HomeController : Controller
    {
        #region Injections
        private readonly IHomePageServices _hoemPageServices;
        private readonly HttpContextAccessor _httpContextaccessor;
        public HomeController(IHomePageServices hoemPageServices)
        {
            _hoemPageServices = hoemPageServices;
            _httpContextaccessor = new HttpContextAccessor();
        }
        #endregion

        public async Task<IActionResult> Index()
        {
            var getHomePage = await _hoemPageServices.GetBanersAndProductsAsync();
            //ViewData["Ip"] = _httpContextaccessor.HttpContext.Connection.RemoteIpAddress;
            return View(getHomePage);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

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
