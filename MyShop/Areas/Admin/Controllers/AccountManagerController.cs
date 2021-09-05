using Application.InterFaces.Admin;
using Application.Utilities;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountManagerController : Controller
    {
        #region Ingections
        private readonly IAccountServices _accountServices;
        public AccountManagerController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, string search, string filter)
        {
            var userLogIn = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _accountServices.GetAllUsersListAsync(userLogIn);


            if (!string.IsNullOrWhiteSpace(search))
            {
                if (search.Contains("@"))
                {
                    users = users.Where(p => p.UserEmail.ToLower().Contains(search.ToLower())).ToList();
                }
                else
                {
                    users = users.Where(p => p.UserName.ToLower().Contains(search.ToLower())).ToList();
                }

                ViewBag.Search = search;
            }

            ViewBag.Filter = "all";
            ViewBag.FilterName = "همه";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "all":
                        users = users.ToList();
                        ViewBag.Filter = "all";
                        ViewBag.FilterName = "همه";
                        break;
                    case "admin":
                        users = users.Where(p => p.RoleName == "Manager").ToList();
                        ViewBag.Filter = "admin";
                        ViewBag.FilterName = "مدیریت کنندگان";
                        break;
                    case "admin2":
                        users = users.Where(p => p.RoleName == "Writer").ToList();
                        ViewBag.Filter = "admin2";
                        ViewBag.FilterName = "نویسندگان";
                        break;
                    case "customer":
                        users = users.Where(p => p.RoleName == "Customer").ToList();
                        ViewBag.Filter = "customer";
                        ViewBag.FilterName = "مشتریان";
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
            var userLoginId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = await _accountServices.GetRolesAsync(userLoginId);
            var modelForReturnToView = new CreateAccountViewModel()
            {
                RolesItem = roles
            };

            return View(modelForReturnToView);

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

            var userLoginId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = await _accountServices.GetRolesAsync(userLoginId);
            var modelForReturnToView = new CreateAccountViewModel()
            {
                RolesItem = roles
            };

            return View(modelForReturnToView);
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
        public async Task<IActionResult> Comments(int? pageNumber, string search, string filter)
        {
            var comments = await _accountServices.GetCommentsAsync();
            comments = comments.OrderByDescending(p => p.CommentId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                comments = comments.Where(p => p.CommentText.Contains(search) || p.CommentTopic.Contains(search)
                  || p.UserEmail.Contains(search) || p.UserName.Contains(search) || p.ProductName.Contains(search));

                ViewBag.Search = search;
            }

            ViewBag.Filter = "New";
            ViewBag.FilterName = "جدید ترین ";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "New":
                        comments = comments.OrderByDescending(p => p.CommentId);
                        ViewBag.Filter = "New";
                        ViewBag.FilterName = "جدید ترین ";
                        break;
                    case "Old":
                        comments = comments.OrderBy(p => p.CommentId);
                        ViewBag.Filter = "Old";
                        ViewBag.FilterName = "قدیمی ترین";
                        break;
                    case "NotShow":
                        comments = comments.Where(p => !p.IsShow).ToList();
                        ViewBag.Filter = "NotShow";
                        ViewBag.FilterName = "تایید نشده";
                        break;
                    case "Show":
                        comments = comments.Where(p => p.IsShow).ToList();
                        ViewBag.Filter = "Show";
                        ViewBag.FilterName = "تایید شده";
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
        public async Task<IActionResult> EditComment(int commentId = 0)
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

            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            var comment = await _accountServices.GetCommentAsync(model.CommentId);

            return View(comment);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestions(string search, int? pageNumber, string filter)
        {
            var questions = await _accountServices.GetQuestionsAsync();


            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                questions = questions.Where(p => p.Text.Contains(search)
                || p.Email.Contains(search) || p.ProductName.Contains(search)).ToList();

                ViewBag.Search = search;
            }

            ViewBag.FilterName = "جدید ترین";
            ViewBag.Filter = "newest";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "newest":
                        questions = questions.OrderByDescending(p => p.Id).ToList();
                        ViewBag.FilterName = "جدید ترین";
                        ViewBag.Filter = "newest";
                        break;
                    case "older":
                        questions = questions.OrderBy(p => p.Id).ToList();
                        ViewBag.FilterName = "قدیمی ترین";
                        ViewBag.Filter = "older";
                        break;
                    case "noAwnser":
                        questions = questions.Where(p => p.Replaise == null).ToList();
                        ViewBag.FilterName = "پاسخ داده نشده";
                        ViewBag.Filter = "noAwnser";
                        break;
                }
            }


            var paging = new PagingList<QuestionViewModel>(questions, 10, pageNumber ?? 1);
            var userPaging = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetQuestions";
            ViewBag.Controller = "AccountManager";
            #endregion


            return View(questions);
        }

        [HttpGet]
        public async Task<IActionResult> EditQuestion(int questionId)
        {

            var result = await _accountServices.GetQuestionAsync(questionId);

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditQuestion(QuestionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _accountServices.EditQuestionAsync(model, userId);

            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(IList<QuestionViewModel> models)
        {
            var resulr = await _accountServices.DeleteQuestionAsync(models);

            if (resulr.Status)
            {
                TempData["Success"] = resulr.SuccesMessage;
            }
            else
            {
                TempData["Error"] = resulr.ErrorMessage;
            }

            return RedirectToAction("GetQuestions");
        }

        [HttpGet]
        public async Task<IActionResult> GetFactors(int? pageNumber, string search = "", string filter = "")
        {
            var result = await _accountServices.GetFactorsAsync();
            result = result.OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search))
            {
                result = result.Where(p => p.RefId == int.Parse(search) || p.UserEmail.Contains(search));

                ViewBag.Search = search;
            }

            ViewBag.Filter = "New";
            ViewBag.FilterName = "جدید ترین ";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "New":
                        result = result.OrderByDescending(p => p.Id);
                        ViewBag.Filter = "New";
                        ViewBag.FilterName = "جدید ترین ";
                        break;
                    case "Old":
                        result = result.OrderBy(p => p.Id);
                        ViewBag.Filter = "Old";
                        ViewBag.FilterName = "قدیمی ترین";
                        break;
                    case "Cansel":
                        result = result.Where(p => p.FactorStatus == "لغو شده").ToList();
                        ViewBag.Filter = "Cansel";
                        ViewBag.FilterName = "لغو شده ها";
                        break;
                    case "Progssess":
                        result = result.Where(p => p.FactorStatus == "در حال انجام").ToList();
                        ViewBag.Filter = "Progssess";
                        ViewBag.FilterName = "در حال اجرا";
                        break;
                    case "Success":
                        result = result.Where(p => p.FactorStatus == "اتمام یافته").ToList();
                        ViewBag.Filter = "Success";
                        ViewBag.FilterName = "اتمام یافته";
                        break;
                }
            }


            var paging = new Paging<FactorViewModel>(result, 10, pageNumber ?? 1);
            var userPaging = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetFactors";
            ViewBag.Controller = "AccountManager";
            #endregion

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditFactor(int factorId)
        {
            var result = await _accountServices.GetFactorAsync(factorId);

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditFactor(FactorViewModel model)
        {
            var result = await _accountServices.EditFactorAsync(model);

            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserContacts(int? pageNumber, string search, string filter)
        {
            var result = await _accountServices.GetContactsAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                result = result.Where(p => p.Text.Contains(search)
                || p.UserEmail.Contains(search) || p.Topic.Contains(search)).ToList();

                ViewBag.Search = search;
            }

            ViewBag.Filter = "newest";
            ViewBag.FilterName = "جدید ترین";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                switch (filter)
                {
                    case "newest":
                        result = result.OrderByDescending(p => p.Id).ToList();
                        ViewBag.Filter = "newest";
                        ViewBag.FilterName = "جدید ترین";
                        break;
                    case "older":
                        result = result.OrderBy(p => p.Id).ToList();
                        ViewBag.Filter = "older";
                        ViewBag.FilterName = "قدیمی ترین";
                        break;
                    case "noAwnser":
                        result = result.Where(p => p.Awnser == null || p.Awnser == "").ToList();
                        ViewBag.Filter = "noAwnser";
                        ViewBag.FilterName = "پاسخ داده نشده";
                        break;
                }
            }


            var paging = new Paging<ContactViewModel>(result, 10, pageNumber ?? 1);
            result = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetUserContacts";
            ViewBag.Controller = "AccountManager";
            #endregion


            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> ContactDetail(int contactId)
        {
            var result = await _accountServices.GetContactDetailAsync(contactId);

            ViewData["Success"] = TempData["Success"];
            ViewData["Error"] = TempData["Error"];

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> AwnserContact(ContactViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountServices.AwnserAsync(model);

            if (result.Status)
            {
                TempData["Success"] = result.SuccesMessage;
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("GetUserContacts");
        }
    }
}
