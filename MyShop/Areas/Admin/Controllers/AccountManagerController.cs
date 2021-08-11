using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Application.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

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


        public async Task<IActionResult> Index(int? pageNumber, string searchUser, string filter)
        {
            var userLogIn = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _accountServices.GetAllUsersListAsync(userLogIn);


            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                if (searchUser.Contains("@"))
                {
                    users = users.Where(p => p.UserEmail.ToLower().Contains(searchUser.ToLower())).ToList();
                }
                else
                {
                    users = users.Where(p => p.UserName.ToLower().Contains(searchUser.ToLower())).ToList();
                }

                ViewBag.SearchUser = searchUser;
            }

            ViewBag.Filter = "همه";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "all":
                        users = users.ToList();
                        ViewBag.Filter = "همه";
                        break;
                    case "admin":
                        users = users.Where(p => p.RoleName == "Manager").ToList();
                        ViewBag.Filter = "مدیریت کنندگان";
                        break;
                    case "admin2":
                        users = users.Where(p => p.RoleName == "Writer").ToList();
                        ViewBag.Filter = "نویسندگان";
                        break;
                    case "customer":
                        users = users.Where(p => p.RoleName == "Customer").ToList();
                        ViewBag.Filter = "مشتریان";
                        break;
                }
            }


            var paging = new PagingList<UsersListViewModel>(users, 10, pageNumber ?? 1);
            var userPaging = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "Index";
            ViewBag.Controller = "AccountManager";
            #endregion

            ViewData["Error"] = TempData["Error"];
            ViewData["Success"] = TempData["Success"];

            return View(userPaging);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var userLoginId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = await _accountServices.GetRolesAsync(userLoginId);
            var modelForReturnToView = new CreateAccountViewModel()
            {
                RolesItem = roles
            };

            return View(modelForReturnToView);
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
            var userLogIn = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _accountServices.FinUserById(userId, userLogIn);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _accountServices.EditUserAsync(model);

            if (result)
                ViewData["Success"] = "عملیات ویرایش با موفقیت انجام شد !";
            else
                ViewData["Error"] = "عملیات ویرایش با موفقیت انجام نشد !";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers(IEnumerable<UsersListViewModel> model)
        {
            if (model.Count() <= 0) return NotFound();

            var result = await _accountServices.DeleteUsersAsync(model);

            if (!result)
                TempData["Error"] = "مشکلی در حذف کاربر یا کابران به وجود آمده است !!";
            else
                TempData["Success"] = "حذف کاربر یا کاربران با موفقیت انجام شد !";

            return RedirectToAction("Index");
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

        [HttpGet]
        public async Task<IActionResult> Comments(int? pageNumber,string searchComment,string filter)
        {
            var comments = await _accountServices.GetCommentsAsync();
            comments = comments.OrderByDescending(p => p.CommentId);

            if (!string.IsNullOrWhiteSpace(searchComment))
            {
                comments = comments.Where(p => p.CommentText.Contains(searchComment) || p.CommentTopic.Contains(searchComment)
                  || p.UserEmail.Contains(searchComment) || p.UserName.Contains(searchComment) || p.ProductName.Contains(searchComment));

                ViewBag.SearchComment = searchComment;
            }

            ViewBag.Filter = "جدید ترین";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "New":
                        comments = comments.OrderByDescending(p => p.CommentId);
                        ViewBag.Filter = "جدید ترین";
                        break;
                    case "Old":
                        comments = comments.OrderBy(p => p.CommentId);
                        ViewBag.Filter = "قدیمی ترین";
                        break;
                    case "NotShow":
                        comments = comments.Where(p => !p.IsShow).ToList();
                        ViewBag.Filter = "تایید نشده";
                        break;
                    case "Show":
                        comments = comments.Where(p => p.IsShow).ToList();
                        ViewBag.Filter = "تایید شده";
                        break;
                }
            }


            var paging = new PagingList<CommentsViewModel>(comments, 10, pageNumber ?? 1);
            var userPaging = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "Comments";
            ViewBag.Controller = "AccountManager";
            #endregion

            return View(comments.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> EditComment(int commentId=0)
        {
            if (commentId == 0) return NotFound();

            var result = await _accountServices.GetCommentAsync(commentId);

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditComment(CommentsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountServices.EditCommentAsync(model);

            if(result.Status)
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
