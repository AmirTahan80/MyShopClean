using Application.InterFaces.User;
using Application.Utilities;
using Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyShop.Controllers
{
    public class AccountController : Controller
    {
        #region Injections
        private readonly IAccountUserServices _accountUserServices;
        private readonly ICommentUserServices _commentUserServices;
        private readonly IProductUserServices _productUserServices;
        private readonly IPayUserServices _payUserServices;
        public AccountController(IAccountUserServices accountUserServices,
            ICommentUserServices commentUserServices,
            IProductUserServices productUserServices, IPayUserServices payUserServices)
        {
            _accountUserServices = accountUserServices;
            _commentUserServices = commentUserServices;
            _productUserServices = productUserServices;
            _payUserServices = payUserServices;
        }
        #endregion

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisetUserForLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountUserServices.RegisterUserWithGmail(model);
            if (!result)
                ViewData["Error"] = "مشکلی در ثبت نام به وجود آمده !! لطفا با نام کاربری و ایمیل دیگری امتحان کنید و تکرار رمز عبور با رمز عبور یکی باشد !!";
            else
                ViewData["Success"] = "با تشکر از ثبت نام شما ... لطفا به ایمیل خود بروید و آن را تایید کنید !";

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userEmail, string token)
        {
            var result = await _accountUserServices.ConfirmEmailAsync(userEmail, token);

            if (!result)
                ViewData["Error"] = $"ایمیل {userEmail} تایید نشد لطفا دوباره امتحان کنید !";
            else
                ViewData["Success"] = $"ایمیل {userEmail} تایید شد !";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountUserServices.LoginAsync(model);
            if (result.Status)
            {
                return RedirectToAction("Profile");
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _accountUserServices.GetDescriptionAsync(userId);

            if (user == null) return NotFound();

            ViewBag.Class = 0;

            return View(user);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PersonalInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userPersonalInfo = await _accountUserServices.GetPersonalInformationAsync(userId);

            ViewBag.Class = 6;

            if (userPersonalInfo == null)
                return NotFound();
            else
                return View(userPersonalInfo);
        }
        [HttpPost]
        public async Task<IActionResult> PersonalInfo(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var editResult = await _accountUserServices.EditPersonalInfo(model, userId);

            if (editResult == null) return NotFound();
            else if (editResult.Status == false)
            {
                if (editResult.ShowNotFound)
                    return NotFound();
                else
                {
                    ViewData["Error"] = editResult.ErrorMessage;
                    return View(model);
                }
            }
            else if (editResult.Status == true)
            {
                ViewData["Success"] = editResult.SuccesMessage;
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(NewPassWordViewModel model)
        {

            if (!ModelState.IsValid) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountUserServices.ChangePassWord(model, userId);

            if (result.ShowNotFound == true)
                return NotFound();

            if (result.Status == true)
            {
                ViewData["Success"] = result.SuccesMessage;
                return View(model);
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassWord()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassWord(ForgotPassWordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountUserServices.ForgotPassWordAsync(model);
            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
                return View();
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string userEmail, string token)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(token)) return NotFound();

            ViewBag.Email = userEmail;
            ViewBag.Token = token;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountUserServices.ResetPassWordAsync(model);
            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
                return View(model);
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOutWeb()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return NotFound();

            await _accountUserServices.SignOutAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int templateId, int count, string returnUrl = "")
        {
            if (productId == 0) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountUserServices.AddToCartAsync(productId, templateId, count, userId);

            if (result.Status)
            {
                ViewData["Success"] = result.SuccesMessage;
                //if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                //{
                //    return Redirect(returnUrl+"?productId="+productId);
                //}
                //else
                return RedirectToAction("ShowCart");
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
                //if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                //{
                //    return Redirect(returnUrl + "/" + productId);
                //}
                //else
                return RedirectToAction("ShowCart");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShowCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            var cart = await _accountUserServices.GetCartAsync(userId);

            ViewData["Success"] = TempData["Success"];
            ViewData["Error"] = TempData["Error"];

            return View(cart);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoveCartDetail(int cartDetailId = 0)
        {
            if (cartDetailId == 0) return NotFound();

            var result = await _accountUserServices.RemoveCartDetail(cartDetailId);
            if (result == false)
            {
                TempData["Error"] = "در حذف محصول مشکلی پیش آمده است !!!";
            }
            else
            {
                TempData["Success"] = "حذف محصول با موفقیت انجام شد .";
            }

            return RedirectToAction("ShowCart");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LowOffProduct(int cartDetailId = 0)
        {
            if (cartDetailId == 0) return NotFound();

            var result = await _accountUserServices.LowOffProduct(cartDetailId);

            return RedirectToAction("ShowCart");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> IncreaseProduct(int cartDetailId = 0)
        {
            if (cartDetailId == 0) return NotFound();

            var result = await _accountUserServices.IncreaseProduct(cartDetailId);

            return RedirectToAction("ShowCart");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddToFavorite(int productId = 0, string returnUrl = "")
        {
            if (productId == 0)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountUserServices.AddOrRemoveToFavotieAsync(productId, userId);

            if (result.Status)
            {
                TempData["Success"] = result.SuccesMessage;
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                    return View();
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                    return View();
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShowFavorite()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return NotFound();

            var result = await _accountUserServices.GetFavoriteAsync(userId);

            ViewData["Error"] = TempData["Error"];
            ViewData["Success"] = TempData["Success"];

            return View(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(int favoriteDetailId = 0)
        {
            if (favoriteDetailId == 0) return NotFound();

            var result = await _accountUserServices.RemoveFavoriteDetailAsync(favoriteDetailId);

            return RedirectToAction("ShowFavorite");

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ProductComment(int id = 0)
        {
            if (id == 0) return NotFound();

            var result = await _productUserServices.GetProductDescriptionAsync(id);

            ViewData["Success"] = TempData["Success"];
            ViewData["Error"] = TempData["Error"];

            return View(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(GetProductDescriptionViewModel model, string returnUrl = "")
        {
            if (model.Comment.ProductId == 0 || string.IsNullOrWhiteSpace(model.Comment.Topic) || string.IsNullOrWhiteSpace(model.Comment.Text))
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _commentUserServices.AddUserCommentAsync(model.Comment, userId);

            if (result.Status)
                TempData["Success"] = result.SuccesMessage;
            else
                TempData["Error"] = result.ErrorMessage;

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return View();

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserComments(int? pageNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound();

            var result = await _commentUserServices.GetUserComments(userId);

            var paging = new PagingList<UserCommentsViewModel>(result.Comments, 5, pageNumber ?? 1);
            result.Comments = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetUserComments";
            ViewBag.Controller = "Account";
            #endregion

            ViewBag.Class = 4;

            return View(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserQuestion(int? pageNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound();

            var result = await _accountUserServices.GetQuestionsAsync(userId);

            var paging = new PagingList<QuestionViewModel>(result.Questions, 5, pageNumber ?? 1);
            result.Questions = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetUserQuestion";
            ViewBag.Controller = "Account";
            #endregion

            ViewBag.Class = 5;

            return View(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Discount(CartViewModel model, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(model.CodeName))
                return NotFound();

            var result = await _accountUserServices.DiscountCartAsync(model);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return NotFound();
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PayCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _payUserServices.AddRequestPayAsync(userId);

            if (result.Status)
            {
                return Redirect(result.ReturnRedirect);
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(result.ReturnRedirect))
                {
                    return Redirect(result.ReturnRedirect);
                }
                else if (result.ShowNotFound)
                    return NotFound();
                else
                {
                    return RedirectToAction("ShowCart");
                }
            }

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Validate(string id, string authority, string status)
        {
            if (status == "" || authority == "")
                return NotFound();

            var result = await _payUserServices.Verification(id, authority, status);

            if(result.RetrunResult.ShowNotFound)
            {
                return NotFound();
            }
            else if(result.RetrunResult.Status)
            {
                ViewData["Success"] = result.RetrunResult.SuccesMessage;
                return View(result);
            }
            else
            {
                ViewData["Error"] = result.RetrunResult.ErrorMessage;
                return View();
            }

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFactors(int? pageNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _accountUserServices.GetFactorsAsync(userId);

            var paging = new PagingList<FactorViewModel>(result.Factors, 5, pageNumber ?? 1);
            result.Factors = paging.QueryResult;

            #region ViewBagForPaging
            ViewBag.PageNumber = pageNumber ?? 1;
            ViewBag.FirstPage = paging.FirstPage;
            ViewBag.LastPage = paging.LastPage;
            ViewBag.PrevPage = paging.PreviousPage;
            ViewBag.NextPage = paging.NextPage;
            ViewBag.Count = paging.LastPage;
            ViewBag.Action = "GetFactors";
            ViewBag.Controller = "Account";
            #endregion

            ViewBag.Class = 1;

            return View(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FactorDetail(int factorId=0)
        {
            if (factorId == 0)
                return NotFound();

            var result = await _accountUserServices.GetFactorAsync(factorId);

            return View(result);
        }
    }
}
