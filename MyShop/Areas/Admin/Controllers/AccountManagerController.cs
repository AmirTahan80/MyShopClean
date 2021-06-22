using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountManagerController : Controller
    {
        //Ingections
        private readonly IAccountServices _accountServices;
        public AccountManagerController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        //End Injections


        public async Task<IActionResult> Index()
        {
            var users = await _accountServices.GetAllUsersListAsync();
            return View(users);
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "تمامی فیلد ها اجباری هستند ! ";
                return View(model);
            }
            var result = await _accountServices.CreateUserByEmailAndPass(model, model.UserPassWord);
            if (result)
                ViewData["Success"] = "اکانت کاربری با موفقیت انجام شد ....";
            else
            {
                ViewData["Error"] = "در ساخت اکانت کاربر مشکلی به وجود آمده است لطفا دوباره تلاش کنید در صورت رفع نشدن خطا با نام کاربری و ایمیل دیگری امتحان کنید !";
                return View(model);
            }
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _accountServices.FinUserById(userId);
            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userEmail, string token)
        {
            if (userEmail == "" || token == "" || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(token))
                return NotFound();

            var result = await _accountServices.ConfirmEmailAsync(userEmail, token);
            if (!result)
                return NotFound();
            else
                ViewData["Success"] =
                    "ایمیل شما تایید شد و از این به بعد شما عضو فعال سایت هستید !";

            //ToDo: Pass User to dashboard if email confirm
            return View();
        }
    }
}
