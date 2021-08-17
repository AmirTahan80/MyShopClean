using Application.InterFaces.Both;
using Application.InterFaces.User;
using Application.ViewModels;
using Application.ViewModels.User;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class AccountUserServices : IAccountUserServices
    {
        #region Injections
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageSenderServices _messageSender;
        private static IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;
        private readonly LinkGenerator _linkGenerator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IQuestionRepository _questionReposiotry;

        public AccountUserServices(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment env, LinkGenerator linkGenerator, IMessageSenderServices messageSender,
            SignInManager<ApplicationUser> signInManager, IProductRepository productRepository,
            ICartRepository cartRepository, IQuestionRepository questionReposiotry)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _linkGenerator = linkGenerator;
            _messageSender = messageSender;
            _signInManager = signInManager;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _questionReposiotry = questionReposiotry;
        }
        #endregion

        public async Task<bool> RegisterUserWithGmail(RegisetUserForLoginViewModel register)
        {
            try
            {
                if (register.PassWord != register.RePassWord) return false;

                var userEmailExist = await IsUserEmailExist(register.Email);
                if (userEmailExist) return false;

                var userNameExist = await IsUserNameExist(register.UserName);
                if (userNameExist) return false;

                var userDetail = new UserDetail();

                var userCreate = new ApplicationUser()
                {
                    UserName = register.UserName,
                    Email = register.Email,
                    UserDetail = userDetail
                };

                var result = await _userManager.CreateAsync(userCreate, register.PassWord);

                if (result.Succeeded)
                {
                    var resultSendEmail = await SendConfirmEmailAsync(userCreate);
                    if (!resultSendEmail)
                        return false;
                }
                else return false;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public async Task<bool> ConfirmEmailAsync(string userEmail, string token)
        {
            try
            {
                var findUser = await _userManager.FindByEmailAsync(userEmail);
                if (findUser == null) return false;
                var result = await _userManager.ConfirmEmailAsync(findUser, token);
                if (!result.Succeeded) return false;
                await _userManager.AddToRoleAsync(findUser, "Customer");

                await _signInManager.SignInAsync(findUser, false);

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public async Task<ResultDto> LoginAsync(LoginViewModel login)
        {
            try
            {
                var findUser = new ApplicationUser();
                if (login.EmailOrName.Contains("@"))
                {
                    findUser = await _userManager.FindByEmailAsync(login.EmailOrName);
                }
                else
                {
                    findUser = await _userManager.FindByNameAsync(login.EmailOrName);
                }

                if (findUser == null)
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "کاربری با این مشخصات یافت نشد !!!",
                        Status = false
                    };
                    return returnResult;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(findUser, login.PassWord, true);

                if (result.IsLockedOut)
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "اکانت شما به دلیل اشتباه وارد کردن گذرواژه تا دقایقی از دسترس خارج شده است !!!",
                        Status = false
                    };
                    return returnResult;
                }

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(findUser, login.RememberMe);

                    var returnResult = new ResultDto()
                    {
                        SuccesMessage = "به فروشگاه من خوش آمدید...",
                        Status = true
                    };
                    return returnResult;
                }
                else
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "رمز عبور یا ایمیل و یا نام کاربری اشتباه است !!!!",
                        Status = false
                    };
                    return returnResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در صورتی که از درست بودن اطلاعات مطمئن هستید برای رفع مشکل با پشتیبانی تماس بگیرید !!!!",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<ProfileViewModel> GetDescriptionAsync(string userId)
        {
            var findUser = await _userManager.FindByIdAsync(userId);
            if (findUser == null) return null;

            var returnUserDescription = new ProfileViewModel()
            {
                Email = findUser.Email,
                Name = findUser.UserName,
                RoleName = _userManager.GetRolesAsync(findUser).GetAwaiter().GetResult().FirstOrDefault(),
                PhoneNumber = findUser.PhoneNumber
            };

            return returnUserDescription;
        }
        public async Task<ProfileViewModel> GetPersonalInformationAsync(string userId)
        {
            var findUser = await _userManager.Users.Include(p => p.UserDetail).SingleOrDefaultAsync(p => p.Id == userId);
            if (findUser == null) return null;

            var returnPersonalInfo = new ProfileViewModel()
            {
                Name = findUser.UserName,
                Email = findUser.Email,
                Address = findUser.UserDetail.Address,
                FirstName = findUser.UserDetail.FirstName,
                LastName = findUser.UserDetail.LastName,
                PhoneNumber = findUser.PhoneNumber
            };

            return returnPersonalInfo;
        }

        public async Task<ResultDto> EditPersonalInfo(ProfileViewModel editPersonalInfo, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(editPersonalInfo.Name) || string.IsNullOrWhiteSpace(editPersonalInfo.Email))
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "فیلدهایی که دارای ستاره قرمز هستند نمیتوانند خالی باشند !!!!",
                        Status = false
                    };
                    return returnResult;
                }

                var findUser = await _userManager.Users.Include(p => p.UserDetail).SingleOrDefaultAsync(p => p.Id == userId);
                if (findUser == null)
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "کاربری یافت نشد !!!",
                        Status = false,
                        ShowNotFound = true
                    };
                    return returnResult;
                }

                findUser.UserName = editPersonalInfo.Name;
                if (findUser.Email != editPersonalInfo.Email)
                {
                    findUser.Email = editPersonalInfo.Email;
                    var sendMailResult = await SendConfirmEmailAsync(findUser);
                    if (!sendMailResult)
                    {
                        var returnResult = new ResultDto()
                        {
                            ErrorMessage = "مشکلی در ارسال پیام به ایمیل  به وجود آمد !! لطفا دوباره امتحان کنید.",
                            Status = false,
                        };
                        return returnResult;
                    }
                }
                findUser.PhoneNumber = editPersonalInfo.PhoneNumber;
                findUser.UserDetail.FirstName = editPersonalInfo.FirstName;
                findUser.UserDetail.LastName = editPersonalInfo.LastName;
                findUser.UserDetail.Address = editPersonalInfo.Address;


                var result = await _userManager.UpdateAsync(findUser);
                if (result.Succeeded)
                {
                    var returnResult = new ResultDto()
                    {
                        SuccesMessage = "ویرایش حسابتان با موفقیت انجام شد ...",
                        Status = true,
                    };
                    return returnResult;
                }
                else
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "ویرایش حسابتان با شکست مواجه شد !!! در صورتی که اقدام به ویرایش ایمیل یا نام کاربری کرده اید ممکن است نام کاربری یا ایمیل توسط شخص دیگری مورد استفاده قرار گرفته باشد ... لطفا با ایمیل یا نام کاربی دیگری امتحان کنید ...",
                        Status = false,
                    };
                    return returnResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "ویرایش حسابتان با شکست مواجه شد !!! بعد از مدتی کوتاه امتحان کنید و اگر باز هم با خطا مواجه شدید با پشتیبانی سایت تماس بگیرید ... با تشکر.",
                    Status = false,
                };
                return returnResult;
            }
        }

        public async Task<ResultDto> ChangePassWord(NewPassWordViewModel changePassWord, string userId)
        {
            try
            {
                var findUser = await _userManager.FindByIdAsync(userId);
                if (findUser == null)
                {
                    var returnResult = new ResultDto()
                    {
                        Status = false,
                        ShowNotFound = true
                    };
                }

                var result = await _userManager.ChangePasswordAsync(findUser, changePassWord.OldPassWord, changePassWord.NewPassword);

                if (result.Succeeded)
                {
                    var returnResult = new ResultDto()
                    {
                        SuccesMessage = "تغییر رمز عبور با موفقیت انجام شد ...",
                        Status = true
                    };
                    return returnResult;
                }
                else
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "تغییر رمز عبور با شکست مواجه شد !!!",
                        Status = false
                    };
                    return returnResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در تغییر رمز عبور مشکلی پیش آمده است !!! دقایقی دیگر امتحان کنید و اگر باز هم با این مشکل مواجه شدید با پشتیبانی تماس بگیرید ...",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<ResultDto> ForgotPassWordAsync(ForgotPassWordViewModel forgotPassword)
        {
            try
            {
                var findUser = await _userManager.FindByEmailAsync(forgotPassword.Email);
                if (findUser == null)
                {
                    await Task.Delay(2000);
                    var returnResult = new ResultDto()
                    {
                        SuccesMessage = "در صورت درست بودن ایمیل لینک نغییر رمز عبور برای شما ارسال شد.",
                        Status = true
                    };
                    return returnResult;
                }
                else
                {
                    var result = await SendForgotPasswordAsync(findUser);
                    var returnResult = new ResultDto()
                    {
                        SuccesMessage = "در صورت درست بودن ایمیل لینک نغییر رمز عبور برای شما ارسال شد.",
                        Status = true
                    };
                    return returnResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "مشکلی در ارسال لینک به ایمیل شما به وجو آمده است لطفا لحظاتی دیگر دوباره امتحان کنید . در صورت مواجه شدن با مشکل با پشتیبانی تماس بگیرید !",
                    Status = false
                };
                return returnResult;
            }
        }
        public async Task<ResultDto> ResetPassWordAsync(ResetPasswordViewModel resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "کاربری  با این مشخصات یافت نشد !!!!",
                    Status = false,
                };
                return returnResult;
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (result.Succeeded)
            {
                var returnResult = new ResultDto()
                {
                    SuccesMessage = "رمز عبور با موفقیت تغییر یافت .",
                    Status = true,
                };
                return returnResult;
            }
            else
            {
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در تغییر رمز عبور مشکلی پیش آمده است لطفا دوباره تلاش کنید !",
                    Status = false,
                };
                return returnResult;
            }
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ResultDto> AddToCartAsync(int productId, int templateId, int count, string userId)
        {
            try
            {
                var productPrice = 0;

                var product = await _productRepository.GetProductAsync(productId);
                if (product == null)
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "محصولی با این مشخصات یافت نشد !!!!!",
                        Status = false
                    };
                    return returnResult;
                }

                var template = new AttributeTemplate();

                if (templateId == 0)
                {
                    if (product.Count < count)
                    {
                        var returnResult = new ResultDto()
                        {
                            ErrorMessage = "تعداد خواسته شده از تعداده موجو در انبار بیشتر است !!!",
                            Status = false
                        };
                        return returnResult;
                    }
                    template = null;
                    productPrice = product.Price;
                }
                else
                {
                    var findTemplate = product.AttributeTemplates.SingleOrDefault(p => p.AttributeTemplateId == templateId);
                    if (findTemplate.AttrinbuteTemplateCount < count)
                    {
                        var returnResult = new ResultDto()
                        {
                            ErrorMessage = "تعداد خواسته شده از تعداده موجو در انبار بیشتر است !!!",
                            Status = false
                        };
                        return returnResult;
                    }
                    template = findTemplate;
                    productPrice = findTemplate.AttrinbuteTemplatePrice;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    var returnResult = new ResultDto()
                    {
                        ErrorMessage = "لطفا ابتدا وارد سایت شوید !",
                        Status = false
                    };
                    return returnResult;
                }

                var cart = await _cartRepository.GetCartAsync(user.Id);

                if (cart == null)
                {
                    var cartCreate = new Cart()
                    {
                        CreateTime = DateTime.Now,
                        IsFinally = false,
                        UserId = user.Id,
                        User = user,
                    };
                    await _cartRepository.AddCart(cartCreate);

                    var cartDetailCreate = new CartDetail()
                    {
                        CartId = cartCreate.CartId,
                        Cart = cartCreate,
                        ProductCount = count,
                        ProductId = product.Id,
                        Product = product,
                        ProductPrice = productPrice,
                        TotalPrice = Convert.ToInt32(productPrice * count),
                        Templates = template
                    };

                    await _cartRepository.AddCartDetail(cartDetailCreate);

                    cartCreate.TotalPrice += cartDetailCreate.TotalPrice;
                }
                else
                {
                    var cartDetail = cart.CartDetails.SingleOrDefault(p => p.ProductId == product.Id);

                    if (cartDetail == null)
                    {
                        var cartDetailCreate = new CartDetail()
                        {
                            CartId = cart.CartId,
                            Cart = cart,
                            Product = product,
                            ProductId = product.Id,
                            ProductCount = count,
                            ProductPrice = productPrice,
                            TotalPrice = productPrice * count,
                            Templates = templateId == 0 ? null : template
                        };

                        await _cartRepository.AddCartDetail(cartDetailCreate);

                        cart.TotalPrice += cartDetailCreate.TotalPrice;
                    }
                    else
                    {

                        if (product.AttributeTemplates == null)
                        {
                            if (product.Count < count)
                            {
                                var returnResult = new ResultDto()
                                {
                                    ErrorMessage = "تعداد خواسته شده از تعداده موجو در انبار بیشتر است !!!",
                                    Status = false
                                };
                                return returnResult;
                            }
                        }
                        else
                        {
                            if (template.AttrinbuteTemplateCount < count)
                            {
                                var returnResult = new ResultDto()
                                {
                                    ErrorMessage = "تعداد خواسته شده از تعداده موجو در انبار بیشتر است !!!",
                                    Status = false
                                };
                                return returnResult;
                            }
                        }

                        cartDetail.ProductCount = count;
                        cartDetail.ProductPrice = productPrice;
                        cartDetail.TotalPrice = productPrice * count;
                        cartDetail.Templates = templateId == 0 ? null : template;

                        cart.TotalPrice += cartDetail.Product.Price;
                    }
                }
                await _cartRepository.SaveAsync();

                var returnResult1 = new ResultDto()
                {
                    SuccesMessage = "محصول به درستی به سبد خرید شما افزوده شد.",
                    Status = true
                };
                return returnResult1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "مشکلی در افزودن محصول به سبد خرید به وجود آمده است ! لطفا دوباره امتحان کنید . در صورت بروز مشکل با پشتیبانی تماس بگیرید ...",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<CartViewModel> GetCartAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return null;

                var cart = await _cartRepository.GetCartAsync(userId);
                if (cart == null || cart.CartDetails.Count == 0)
                    return null;

                var cartDetails = cart.CartDetails.Select(p => new CartDetailViewModel()
                {
                    Id = p.CartDetailId,
                    TotalPrice = p.TotalPrice,
                    ProductCount = p.ProductCount,
                    ProductPrice = p.ProductPrice,
                    ProductName = p.Product.Name,
                    ImgSrc = p.Product.ProductImages.Select(p => p.ImgFile + "/" + p.ImgSrc).FirstOrDefault(),
                    AttributeNames = p.Product.ProductAttributes != null ? p.Product.ProductAttributes.Select(c => new AttributeNamesViewModel()
                    {
                        Name = c.AttributeName
                    }).ToList() : null,
                    AttributeValues = p.Templates != null ? p.Templates.Template.Split("_").ToList() : null,
                    Properties = p.Product.Properties != null ? p.Product.Properties.Select(c => new PropertyViewModel()
                    {
                        PropertyName = c.ValueType,
                        PropertyValue = c.ValueName
                    }) : null,
                }).ToList();

                var returnCart = new CartViewModel()
                {
                    Id = cart.CartId,
                    TotalPrice = cart.TotalPrice,
                    CreateTime = cart.CreateTime,
                    CountOfProduct = cart.CartDetails.Count,
                    CartDetails = cartDetails
                };

                return returnCart;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> RemoveCartDetail(int cartDetailId)
        {
            try
            {
                var cartDetail = await _cartRepository.GetCartDetailAsync(cartDetailId);
                if (cartDetail == null)
                    return false;

                cartDetail.Cart.TotalPrice -= cartDetail.TotalPrice;

                _cartRepository.RemoveCartDetail(cartDetail);

                await _cartRepository.SaveAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public async Task<bool> LowOffProduct(int cartDetailId)
        {
            try
            {
                var cartDetail = await _cartRepository.GetCartDetailAsync(cartDetailId);
                if (cartDetail == null) return false;

                if (cartDetail.ProductCount <= 1)
                {
                    cartDetail.Cart.TotalPrice -= cartDetail.TotalPrice;
                    _cartRepository.RemoveCartDetail(cartDetail);
                }
                else
                {
                    var productPrice = 0;
                    cartDetail.ProductCount -= 1;
                    if (cartDetail.Product.AttributeTemplates.Count > 0)
                    {
                        var template = cartDetail.Product.AttributeTemplates.Where(p => p.AttributeTemplateId == cartDetail.Templates.AttributeTemplateId).SingleOrDefault();
                        productPrice = template.AttrinbuteTemplatePrice;
                    }
                    else
                    {
                        productPrice = cartDetail.Product.Price;
                    }
                    cartDetail.TotalPrice -= productPrice;
                    cartDetail.Cart.TotalPrice -= productPrice;
                }

                await _cartRepository.SaveAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> IncreaseProduct(int cartDetailId)
        {
            try
            {
                var cartDetail = await _cartRepository.GetCartDetailAsync(cartDetailId);
                if (cartDetail == null) return false;

                if (cartDetail.ProductCount < cartDetail.Product.Count)
                {
                    cartDetail.ProductCount += 1;
                    var productPrice = 0;
                    if (cartDetail.Product.AttributeTemplates.Count > 0)
                    {
                        var template = cartDetail.Product.AttributeTemplates.Where(p => p.AttributeTemplateId == cartDetail.Templates.AttributeTemplateId).SingleOrDefault();
                        productPrice = template.AttrinbuteTemplatePrice;
                    }
                    else
                    {
                        productPrice = cartDetail.Product.Price;
                    }
                    cartDetail.TotalPrice += productPrice;
                    cartDetail.Cart.TotalPrice += productPrice;
                }

                await _cartRepository.SaveAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<ResultDto> AddOrRemoveToFavotieAsync(int productId, string userId)
        {
            try
            {
                var returnResult = new ResultDto();

                var product = await _productRepository.GetProductAsync(productId);
                if (product == null)
                {
                    returnResult.ErrorMessage = "مشکلی در افزودن محصول به علاقه مندی ها به وجود آمده است !!!";
                    returnResult.Status = false;
                    return returnResult;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    returnResult.ErrorMessage = "لطفا ابتدا وارد سایت شوید !!!";
                    returnResult.Status = false;
                    return returnResult;
                }

                var favorite = await _cartRepository.GetFavoriteAsync(userId);

                if (favorite == null)
                {
                    var favoriteCreate = new UserFavorite()
                    {
                        UserId = user.Id,
                        User = user,
                    };

                    await _cartRepository.AddFavorite(favoriteCreate);

                    var favoriteDetailCreate = new UserFavoritesDetail()
                    {
                        ProductId = product.Id,
                        Product = product,
                        UserFavorit = favoriteCreate,
                        UserFavoriteId = favoriteCreate.UserFavoriteId
                    };

                    await _cartRepository.AddFavoriteDetail(favoriteDetailCreate);

                    returnResult.SuccesMessage = "محصول با موفقیت به لیست علاقه مندی ها افزوده شد .";
                    returnResult.Status = true;


                }
                else
                {
                    var favoriteDetail = favorite.UserFavoritesDetails.Where(p => p.ProductId == product.Id).SingleOrDefault();

                    if (favoriteDetail == null)
                    {
                        var favoriteDetailCreate = new UserFavoritesDetail()
                        {
                            ProductId = product.Id,
                            Product = product,
                            UserFavorit = favorite,
                            UserFavoriteId = favorite.UserFavoriteId
                        };

                        await _cartRepository.AddFavoriteDetail(favoriteDetailCreate);

                        returnResult.SuccesMessage = "محصول با موفقیت به لیست علاقه مندی ها افزوده شد .";
                        returnResult.Status = true;
                    }
                    else
                    {
                        _cartRepository.RemoveFavoriteDetail(favoriteDetail);

                        returnResult.SuccesMessage = "محصول از لیست علاقه مندی ها حذف شد !!";
                        returnResult.Status = true;
                    }

                }

                await _cartRepository.SaveAsync();
                return returnResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "مشکلی در افزودن محصول به لیست علاقه مندی ها به وجود آمده است !!! لطفا دوباره تلاش کنید و درصورت وجود مشکل با پشتیبانی تماس بگیرید !!!",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<FavoriteViewModel> GetFavoriteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var favorite = await _cartRepository.GetFavoriteAsync(userId);
            if (favorite == null) return null;

            var favoriteDetail = favorite.UserFavoritesDetails.Select(p => new FavoriteDetailViewModel()
            {
                Id = p.UserFavoritesDetailId,
                ProductId = p.ProductId,
                ProductCount = p.Product.Count,
                ProductImage = p.Product.ProductImages.FirstOrDefault().ImgFile + "/" + p.Product.ProductImages.FirstOrDefault().ImgSrc,
                ProductName = p.Product.Name,
                ProductPrice = p.Product.Price
            });

            var favoriteReturn = new FavoriteViewModel()
            {
                Id = favorite.UserFavoriteId,
                FavoriteDetails = favoriteDetail
            };

            return favoriteReturn;

        }
        public async Task<bool> RemoveFavoriteDetailAsync(int favoriteDetailId)
        {
            try
            {
                var favoriteDetail = await _cartRepository.GetFavoriteDetailAsync(favoriteDetailId);
                if (favoriteDetail == null) return false;

                _cartRepository.RemoveFavoriteDetail(favoriteDetail);

                await _cartRepository.SaveAsync();

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<ResultDto> AskQuestionAsync(QuestionViewModel question)
        {
            try
            {
                var returnResult = new ResultDto();

                var user = await _userManager.FindByEmailAsync(question.Email);
                if (user == null)
                {
                    returnResult.ErrorMessage = "کاربری با این ایمیل یافت نشد !!!";
                    returnResult.Status = false;
                    return returnResult;
                }

                if (question.ProductId == 0)
                {
                    returnResult.ErrorMessage = "مشکلی در ثبت سوال پیش آمده است لطفا دوباره تلاش کنید !!!";
                    returnResult.Status = false;
                    return returnResult;
                }
                var product = await _productRepository.GetProductAsync(question.ProductId);
                if (product == null)
                {
                    returnResult.ErrorMessage = "مشکلی در ثبت سوال پیش آمده است لطفا دوباره تلاش کنید !!!";
                    returnResult.Status = false;
                    return returnResult;
                }

                var parent = new Question();
                if (question.ReplayId != 0)
                {
                    parent = await _questionReposiotry.GetQuestionAsync(question.ReplayId);
                    if (parent == null)
                    {
                        returnResult.ErrorMessage = "مشکلی در ثبت سوال پیش آمده است لطفا دوباره تلاش کنید !!!";
                        returnResult.Status = false;
                        return returnResult;
                    }
                }
                else
                {
                    parent = null;
                }

                var quesTionCreate = new Question()
                {
                    QuestionText = question.Text,
                    Topic = question.Topic,
                    User = user,
                    Product = product,
                    ReplayOn = parent != null ? parent : null
                };

                await _questionReposiotry.AddQuestionAsync(quesTionCreate);

                await _questionReposiotry.SaveAsync();

                returnResult.SuccesMessage = "سوال شما با موفقیت ثبت شد در صورت جواب دادن از طریق ایمیل به شما اعلام خواهیم کرد ...";
                returnResult.Status = true;

                return returnResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "مشکلی پیش آمده است و سوال شما ثبت نشسده است لطفا دوباره تلاش کنید !!! در صورت بروز مشکل با پشتیبانی تماس بگیرید ...",
                    Status = false
                };
                return returnResult;
            }
        }
        public async Task<ProfileViewModel> GetQuestionsAsync(string userId)
        {
            var questions = await _questionReposiotry.GetQuestionsAsync();
            if (questions == null)
                return null;

            var userQuestions = questions.Where(p => p.User.Id == userId);
            if (userQuestions == null)
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            var getQuestions = userQuestions.Select(p => new QuestionViewModel()
            {
                Id = p.Id,
                Text = p.QuestionText,
                Topic = p.Topic,
                ProductId = p.Product.Id,
                ReplayId = p.ReplayOn!=null? p.ReplayOn.Id:0
            }).ToList();

            var addToProfile = new ProfileViewModel()
            {
                Id=user.Id,
                Name=user.UserName,
                Email=user.Email,
                PhoneNumber=user.PhoneNumber,
                Questions= getQuestions
            };

            return addToProfile;
        }

        //Private Mehode
        private async Task<bool> IsUserEmailExist(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return false;
            else
                return true;
        }
        private async Task<bool> IsUserNameExist(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return false;
            else
                return true;
        }
        private async Task<bool> SendConfirmEmailAsync(ApplicationUser user, string passForReturn = "ConfirmEmail")
        {

            var emailConfiguration = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callBackUrl = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
        action: passForReturn, "Account",
        new { userEmail = user.Email, token = emailConfiguration }, _httpContextAccessor.HttpContext.Request.Scheme);

            string message = "<a href=\"" + callBackUrl + "\" target='_blank' style='font-size: 20px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; color: #ffffff; text-decoration: none; padding: 15px 25px; border-radius: 2px; border: 1px solid #FFA73B; display: inline-block;'>تایید اکانت</a>";
            //Get TemplateFile located at wwwroot/Templates/EmailTemplate/Register_EmailTemplate.html  
            var pathToFile = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailConfirmTemplate.html";
            string builder;
            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder = SourceReader.ReadToEnd();
            }
            //{0} : Subject  
            //{1} : Current DateTime  
            //{2} : Email  
            //{3} : Username  
            //{4} : Password  
            //{5} : Message  
            //{6} : callbackURL  
            //string messageBody = string.Format(builder.HtmlBody,
            //            passForReturn,
            //            String.Format(ConverToShamsi.GetDate(DateTime.Now).ToString()),
            //            user.UserName,
            //            user.Email,
            //            message,
            //            callBackUrl
            //           );
            //builder = builder.Replace("{subject}", passForReturn);
            //builder = builder.Replace("{dateTime}", ConverToShamsi.GetDate(DateTime.Now).ToString());
            //builder = builder.Replace("{userName}", user.UserName);
            //builder = builder.Replace("{userEmail}", user.Email);
            builder = builder.Replace("{linkForConfirm}", message);
            builder = builder.Replace("{link}", callBackUrl);
            builder = builder.Replace("{userName}", user.UserName);
            builder = builder.Replace("{userEmail}", user.Email);
            await _messageSender.SendEmailAsync(user.Email, passForReturn, builder, true);

            return true;
        }
        private async Task<bool> SendForgotPasswordAsync(ApplicationUser user)
        {

            var emailConfiguration = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callBackUrl = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
        action: "ResetPassword", "Account",
        new { userEmail = user.Email, token = emailConfiguration }, _httpContextAccessor.HttpContext.Request.Scheme);

            string message = "<a href=\"" + callBackUrl + "\" target='_blank' style='font-size: 20px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; color: #ffffff; text-decoration: none; padding: 15px 25px; border-radius: 2px; border: 1px solid #FFA73B; display: inline-block;'>تغییر رمز عبور</a>";
            //Get TemplateFile located at wwwroot/Templates/EmailTemplate/Register_EmailTemplate.html  
            var pathToFile = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate"
                    + Path.DirectorySeparatorChar.ToString()
                    + "ForgotPasswordTemplate.html";
            string builder;
            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder = SourceReader.ReadToEnd();
            }
            //{0} : Subject  
            //{1} : Current DateTime  
            //{2} : Email  
            //{3} : Username  
            //{4} : Password  
            //{5} : Message  
            //{6} : callbackURL  
            //string messageBody = string.Format(builder.HtmlBody,
            //            passForReturn,
            //            String.Format(ConverToShamsi.GetDate(DateTime.Now).ToString()),
            //            user.UserName,
            //            user.Email,
            //            message,
            //            callBackUrl
            //           );
            //builder = builder.Replace("{subject}", passForReturn);
            //builder = builder.Replace("{dateTime}", ConverToShamsi.GetDate(DateTime.Now).ToString());
            //builder = builder.Replace("{userName}", user.UserName);
            //builder = builder.Replace("{userEmail}", user.Email);
            builder = builder.Replace("{linkForChangePassword}", message);
            builder = builder.Replace("{link}", callBackUrl);
            builder = builder.Replace("{userName}", user.UserName);
            builder = builder.Replace("{userEmail}", user.Email);
            await _messageSender.SendEmailAsync(user.Email, "ResetPassword", builder, true);

            return true;
        }

    }
}
