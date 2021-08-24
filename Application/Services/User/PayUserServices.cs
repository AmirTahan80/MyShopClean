using Application.InterFaces.User;
using Application.Utilities.TagHelper;
using Application.ViewModels;
using Application.ViewModels.User;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Dto.Payment;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarinPal.Class;

namespace Application.Services.User
{
    public class PayUserServices : IPayUserServices
    {
        #region Injections
        private readonly IPayRepository _payRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;

        private readonly Payment _payment;
        private readonly Authority _authority;
        private readonly Transactions _transactions;

        Expose expose = new Expose();

        public PayUserServices(IPayRepository payRepository, UserManager<ApplicationUser> userManager,
            ICartRepository cartRepository)
        {
            _payRepository = payRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;

            _payment = expose.CreatePayment();
            _authority = expose.CreateAuthority();
            _transactions = expose.CreateTransactions();
        }
        #endregion
        public async Task<ResultDto> AddRequestPayAsync(string userId)
        {
            try
            {
                var returnResult = new ResultDto();

                int amount = 0;

                var user = await _userManager.Users.Include(p => p.UserDetail).SingleOrDefaultAsync(p => p.Id == userId);

                if (string.IsNullOrWhiteSpace(user.UserDetail.Address) || string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    returnResult.ErrorMessage = "لطفا کد ملی و شماره تلفن را کامل کنید ...";
                    returnResult.ReturnRedirect = $"https://localhost:44373/Account/PersonalInfo";
                    returnResult.Status = false;
                    return returnResult;
                }

                var requestPaies = await _payRepository.GetRequestPaiesAsync();

                var requestPay = requestPaies.SingleOrDefault(p => p.ApplicationUser.Id == userId && !p.IsPay);


                var cart = await _cartRepository.GetCartAsync(userId);

                var discounts = cart.Discounts.ToList();

                int discountsSum = 0;

                foreach (var discount in discounts)
                {
                    discountsSum += discount.DiscountPrice;
                }

                var cartTotalPrice = 0;

                foreach (var cartDetail in cart.CartDetails)
                {
                    cartTotalPrice += cartDetail.TotalPrice;
                }

                amount = cartTotalPrice - discountsSum;

                if (amount <= 0)
                    amount = 0;


                if (requestPay == null)
                {
                    var requestPayCreate = new RequestPay()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IsPay = false,
                        ApplicationUser = user,
                        Cart = cart,
                        CreateTime = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now),
                        Amount = amount
                    };
                    await _payRepository.AddRequestPay(requestPayCreate);
                }
                else
                {
                    requestPay.Amount = amount;
                }

                await _payRepository.SaveAsync();

                var result = await CreateRequestForPayin(user, amount);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    returnResult.SuccesMessage = "ثبت در خواست با موفقیت انجام شد ...";
                    returnResult.Status = true;
                    returnResult.ReturnRedirect = result;
                }
                else
                {
                    returnResult.ErrorMessage = "ثبت در خواست با شکست مواجه شد !!!";
                    returnResult.Status = false;
                    returnResult.ShowNotFound = true;
                }

                return returnResult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "مشکلی در ثبت درخواست پیش آمده لطفا دوباره امتحان کنید !!!",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<VerificationPayViewModel> Verification(string requestPayId, string authority, string status)
        {
            try
            {

                var requestPaies = await _payRepository.GetRequestPaiesAsync();
                var requestPay = requestPaies.SingleOrDefault(p => p.Id == requestPayId);
                if (requestPay == null)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ShowNotFound = true,
                            Status = false
                        }
                    };
                    return returnResult;
                }

                var verification = await _payment.Verification(new DtoVerification
                {
                    Amount = requestPay.Amount,
                    MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                    Authority = authority
                }, Payment.Mode.sandbox);



                if (verification.Status == 100)
                {

                    var cart = await _cartRepository.GetCartAsync(requestPay.ApplicationUser.Id);

                    requestPay.IsPay = true;
                    requestPay.DatePay = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now);
                    requestPay.RefId = verification.RefId;

                    cart.IsFinally = true;

                    foreach(var item in cart.CartDetails)
                    {
                        item.Product.Count -= item.ProductCount;
                    }

                    await _cartRepository.SaveAsync();

                    var factor = new Factor()
                    {
                        CartId = cart.CartId,
                        RefId= requestPay.RefId,
                        TotalPrice = requestPay.Amount,
                        UserAddress = requestPay.ApplicationUser.UserDetail.Address,
                        UserEmail = requestPay.ApplicationUser.Email,
                        UserName = requestPay.ApplicationUser.UserDetail.FirstName,
                        UserFamilly = requestPay.ApplicationUser.UserDetail.LastName,
                        UserPhone = requestPay.ApplicationUser.PhoneNumber,
                        UserId = requestPay.ApplicationUser.Id,
                        User = requestPay.ApplicationUser,
                        Status = FactorStatus.Progssess,
                    };

                    await _payRepository.AddFactor(factor);

                    var templateNames = "";
                    var templateValues = "";

                    int i = 0;
                    foreach (var item in cart.CartDetails)
                    {
                        if (item.Product.ProductAttributes.Count() > 0)
                        {
                            foreach (var attribute in item.Product.ProductAttributes)
                            {
                                if (i == 0)
                                {
                                    templateNames = attribute.AttributeName;
                                }
                                else
                                {
                                    templateNames += "," + attribute.AttributeName;
                                }
                                i++;
                            }
                            templateValues = item.Templates.Template;
                        }
                    }

                    var factorDetail = cart.CartDetails.Select(p => new FactorDetail()
                    {
                        Factor = factor,
                        ImageSrc = p.Product.ProductImages.FirstOrDefault().ImgFile + "/" + p.Product.ProductImages.FirstOrDefault().ImgSrc,
                        ProductCount = p.ProductCount,
                        ProductName = p.Product.Name,
                        ProductPrice = p.Product.Price,
                        TotalPrice = p.TotalPrice,
                        AttributesName = templateNames != "" ? templateNames : "",
                        AttributesValue = templateValues != "" ? templateValues : ""
                    });

                    await _payRepository.AddFactorDetails(factorDetail);

                    await _payRepository.SaveAsync();

                    var returnResult = new VerificationPayViewModel()
                    {
                        Id = requestPay.Id,
                        RefId = requestPay.RefId,
                        TotalPrice= requestPay.Amount,
                        UserAddress = requestPay.ApplicationUser.UserDetail.Address,
                        UserPhoneNumber = requestPay.ApplicationUser.PhoneNumber,
                        UserPostCode = requestPay.ApplicationUser.UserDetail.Province!=null? requestPay.ApplicationUser.UserDetail.Province:"",
                        UserEmail = requestPay.ApplicationUser.Email,
                        UserFirstName = requestPay.ApplicationUser.UserDetail.FirstName,
                        UserLastName = requestPay.ApplicationUser.UserDetail.LastName,
                        DisCounts = cart.Discounts.Count()>0?cart.Discounts.Select(p => new DisCountViewModel()
                        {
                            Name = p.CodeName,
                            Price = p.DiscountPrice
                        }):null,
                        Products = cart.CartDetails.Select(p => new VerficationProductsViewModel()
                        {
                            ProductName = p.Product.Name,
                            ProductPrice = p.Product.Price,
                            TotalPrice = p.TotalPrice
                        }),
                        RetrunResult = new ResultDto()
                        {
                            SuccesMessage = $"کاربر گرامی {requestPay.ApplicationUser.UserName} خرید شما با موفقیت ثبت شد ... شماره رهگیری {requestPay.RefId} میباشد لطفا یادداشت کنید ...",
                            Status = true
                        }
                    };
                    return returnResult;

                }
                else if (verification.Status == -9)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "خطای اعتبار سنجی",
                            Status = false
                        }
                    };
                    return returnResult;
                }
                else if (verification.Status == -10)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "ای پی و يا مرچنت كد پذيرنده صحيح نيست",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -11)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "مرچنت کد فعال نیست لطفا با تیم پشتیبانی ما تماس بگیرید",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -12)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "تلاش بیش از حد در یک بازه زمانی کوتاه",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -15)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "ترمینال شما به حالت تعلیق در آمده با تیم پشتیبانی تماس بگیرید",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -16)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "سطح تاييد پذيرنده پايين تر از سطح نقره اي است",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -30)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "اجازه دسترسی به تسویه اشتراکی شناور ندارید",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -31)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "حساب بانکی تسویه را به پنل اضافه کنید مقادیر وارد شده واسه تسهیم درست نیست",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -32)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -33)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "درصد های وارد شده درست نیست",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -34)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "مبلغ از کل تراکنش بیشتر است",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -35)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "تعداد افراد دریافت کننده تسهیم بیش از حد مجاز است",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -40)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -50)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "مبلغ پرداخت شده با مقدار مبلغ در وریفای متفاوت است",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -51)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "پرداخت ناموفق",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -52)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "خطای غیر منتظره با پشتیبانی تماس بگیرید",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -53)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "اتوریتی برای این مرچنت کد نیست",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == -54)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "اتوریتی نامعتبر است",
                            Status = false
                        }
                    };

                    return returnResult;
                }
                else if (verification.Status == 101)
                {
                    var returnResult = new VerificationPayViewModel()
                    {
                        RetrunResult = new ResultDto()
                        {
                            ErrorMessage = "تراکنش وریفای شده",
                            Status = false
                        }
                    };

                    return returnResult;
                }

                var returnResult1 = new VerificationPayViewModel()
                {
                    RetrunResult = new ResultDto()
                    {
                        ErrorMessage = "خطایی غیره منتظره رخ داده است لطفا دوباره تلاش کنید و در صورت مواحه شدن با این خطا با پشتیبانی تماس بگیرید !!!",
                        Status = false
                    }
                };

                return returnResult1;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new VerificationPayViewModel()
                {
                    RetrunResult = new ResultDto()
                    {
                        ErrorMessage = "پرداخت با شکست مواجه شد !!!",
                        Status = false
                    }
                };
                return returnResult;
            }
        }


        private async Task<string> CreateRequestForPayin(ApplicationUser user, int amount)
        {
            var requestPaies = await _payRepository.GetRequestPaiesAsync();
            var requestPay = requestPaies.SingleOrDefault(p => p.ApplicationUser.Id == user.Id && !p.IsPay);

            var cart = await _cartRepository.GetCartAsync(user.Id);

            var result = await _payment.Request(new DtoRequest()
            {
                Mobile = user.PhoneNumber,
                CallbackUrl =
                $"https://localhost:44373/Account/Validate?id={requestPay.Id}",
                Description = $"پرداخت فاکتور {cart.CartId}",
                Email = user.Email,
                Amount = Convert.ToInt32(amount),
                MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
            }, ZarinPal.Class.Payment.Mode.sandbox);

            var createReturn = $"https://sandbox.zarinpal.com/pg/StartPay/{result.Authority}";

            return createReturn;
        }


    }
}
