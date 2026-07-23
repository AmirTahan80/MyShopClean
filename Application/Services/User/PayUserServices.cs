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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayUserServices> _logger;

        Expose expose = new Expose();

        public PayUserServices(IPayRepository payRepository, UserManager<ApplicationUser> userManager,
            ICartRepository cartRepository, IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ILogger<PayUserServices> logger)
        {
            _payRepository = payRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;

            _payment = expose.CreatePayment();
            _authority = expose.CreateAuthority();
            _transactions = expose.CreateTransactions();

            _httpClient = httpClientFactory.CreateClient("Payments");
            _configuration = configuration;
            _logger = logger;
        }
        #endregion

        public async Task<ResultDto> AddRequestPayZarinPallAsync(string userId)
        {
            try
            {
                var returnResult = new ResultDto();

                int amount = 0;
                var user = await _userManager.Users.Include(p => p.UserDetail).SingleOrDefaultAsync(p => p.Id == userId);

                if (string.IsNullOrWhiteSpace(user?.UserDetail?.Address) || string.IsNullOrWhiteSpace(user?.PhoneNumber??""))
                {
                    returnResult.ErrorMessage = "برای ادامه‌ی پرداخت، لطفاً شماره تلفن و آدرس خود را در بخش اطلاعات حساب تکمیل کنید.";
                    returnResult.ReturnRedirect = _configuration["ReturnsUrl:PassIdPayToUrl"];
                    returnResult.Status = false;
                    return returnResult;
                }

                var requestPaies = await _payRepository.GetRequestPaiesAsync();

                var requestPay = requestPaies.SingleOrDefault(p => p.ApplicationUser.Id == userId && !p.IsPay);


                var cart = await _cartRepository.GetCartAsync(userId);
                if (!TryPrepareCart(cart, out amount, out var cartError))
                {
                    returnResult.ErrorMessage = cartError;
                    returnResult.Status = false;
                    return returnResult;
                }


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
                    returnResult.SuccesMessage = "درخواست پرداخت ایجاد شد؛ در حال انتقال به درگاه هستید.";
                    returnResult.Status = true;
                    returnResult.ReturnRedirect = result;
                }
                else
                {
                    returnResult.ErrorMessage = "ایجاد درخواست پرداخت انجام نشد. لطفاً دوباره تلاش کنید.";
                    returnResult.Status = false;
                    returnResult.ShowNotFound = true;
                }

                return returnResult;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Creating Zarinpal payment request failed for user {UserId}", userId);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در حال حاضر امکان اتصال به درگاه پرداخت وجود ندارد. لطفاً کمی بعد دوباره تلاش کنید.",
                    Status = false
                };
                return returnResult;
            }
        }
        public async Task<ResultDto> AddRquestPayIdPayAsync(string userId)
        {
            try
            {
                var returnResult = new ResultDto();

                int amount = 0;

                var user = await _userManager.Users.Include(p => p.UserDetail).SingleOrDefaultAsync(p => p.Id == userId);

                if (string.IsNullOrWhiteSpace(user.UserDetail.Address) || string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    returnResult.ErrorMessage = "برای ادامه‌ی پرداخت، لطفاً شماره تلفن و آدرس خود را در بخش اطلاعات حساب تکمیل کنید.";
                    returnResult.ReturnRedirect = _configuration["ReturnsUrl:PassIdPayToUrl"];
                    returnResult.Status = false;
                    return returnResult;
                }

                var requestPaies = await _payRepository.GetRequestPaiesAsync();

                var requestPay = requestPaies.SingleOrDefault(p => p.ApplicationUser.Id == userId && !p.IsPay);


                var cart = await _cartRepository.GetCartAsync(userId);
                if (!TryPrepareCart(cart, out amount, out var cartError))
                {
                    returnResult.ErrorMessage = cartError;
                    returnResult.Status = false;
                    return returnResult;
                }


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
                    requestPay.Cart = cart;
                    requestPay.CreateTime = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now);
                }

                await _payRepository.SaveAsync();

                var result = await CreateRequestForPayingIdPayAsync(amount, user, cart);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    returnResult.SuccesMessage = "درخواست پرداخت ایجاد شد؛ در حال انتقال به درگاه هستید.";
                    returnResult.Status = true;
                    returnResult.ReturnRedirect = result;
                }
                else
                {
                    returnResult.ErrorMessage = "ایجاد درخواست پرداخت انجام نشد. لطفاً دوباره تلاش کنید.";
                    returnResult.Status = false;
                    returnResult.ShowNotFound = true;
                }

                return returnResult;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Creating IDPay payment request failed for user {UserId}", userId);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در حال حاضر امکان اتصال به درگاه پرداخت وجود ندارد. لطفاً کمی بعد دوباره تلاش کنید.",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<VerificationPayViewModel> VerificationIdPay(GetResponseIdPayValueViewModel response)
        {
            try
            {
                var apiKey = _configuration["Payments:IdPay:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return PaymentError("تنظیمات درگاه پرداخت کامل نیست. لطفاً با پشتیبانی فروشگاه تماس بگیرید.");
                }

                string apiUrl = "https://api.idpay.ir/v1.1/payment/verify";

                var validateValues = new GetResponseIdPayValueViewModel()
                {
                    order_id = response.order_id,
                    id = response.id
                };

                var json = JsonConvert.SerializeObject(validateValues);

                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                stringContent.Headers.Add("X-API-KEY", apiKey);
                if (_configuration.GetValue("Payments:IdPay:Sandbox", true))
                {
                    stringContent.Headers.Add("X-SANDBOX", "1");
                }

                var retunrResponse = await _httpClient.PostAsync(apiUrl, stringContent);

                var returnContent = await retunrResponse.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<GetResponseIdPayValueViewModel>(returnContent);

                if (retunrResponse.IsSuccessStatusCode)
                {
                    var requestPays = await _payRepository.GetRequestPaiesAsync();
                    var requestPay = requestPays.SingleOrDefault(request =>
                        request.Cart?.CartId == Convert.ToInt32(content.order_id));
                    if (requestPay == null)
                    {
                        return PaymentNotFound();
                    }

                    var finalizedFactor = await _payRepository.FinalizePaymentAsync(
                        requestPay.Id,
                        Convert.ToInt32(content.track_id));
                    if (finalizedFactor == null)
                    {
                        return PaymentError("تأیید پرداخت انجام شد، اما ثبت سفارش کامل نشد. لطفاً با پشتیبانی تماس بگیرید.");
                    }

                    return BuildVerificationResult(requestPay, finalizedFactor);

#pragma warning disable CS0162
                    var carts = await _cartRepository.GetCartsAsync();
                    var payingCart = carts.SingleOrDefault(p => p.CartId == Convert.ToInt32(content.order_id));
                   
                    var user = await _userManager.Users.Include(p=>p.UserDetail)
                        .SingleOrDefaultAsync(p=>p.Id== payingCart.UserId);


                    var factor = new Factor()
                    {
                        CartId = Convert.ToInt32(response.order_id),
                        CreateTime =  ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now),
                        Discounts = payingCart.Discounts,
                        RefId = content.track_id,
                        Status = FactorStatus.Progssess,
                        TotalPrice = content.amount,
                        User = user,
                        UserAddress = user.UserDetail.Address,
                        UserEmail = user.Email,
                        UserFamilly = user.UserDetail.LastName,
                        UserName = user.UserDetail.FirstName,
                        UserPhone = user.PhoneNumber,
                        UserId = user.Id,
                    };

                    await _payRepository.AddFactor(factor);

                    var templateNames = "";
                    var templateValues = "";

                    int i = 0;
                    foreach (var item in payingCart.CartDetails)
                    {
                        if (item.Product.IsProductHaveAttributes)
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

                    var factorDetails = payingCart.CartDetails.Select(p => new FactorDetail()
                    {
                        ImageSrc = p.Product.ProductImages.FirstOrDefault().ImgFile + "/" +
                             p.Product.ProductImages.FirstOrDefault().ImgSrc,
                        AttributesName = templateNames != "" ? templateNames : "",
                        AttributesValue = templateValues != "" ? templateValues : "",
                        ProductCount = p.ProductCount,
                        ProductName = p.Product.Name,
                        ProductPrice = p.ProductPrice,
                        TotalPrice = p.TotalPrice,
                        Factor=factor
                    });

                    await _payRepository.AddFactorDetails(factorDetails);

                    payingCart.IsFinally = true;

                    foreach (var item in payingCart.CartDetails)
                    {
                        if(item.Product.IsProductHaveAttributes)
                        {
                            item.Product.AttributeTemplates.SingleOrDefault(p => p.Template == item.Templates.Template).AttrinbuteTemplateCount -= item.ProductCount;
                        }
                        else
                        {
                            item.Product.Count -= item.ProductCount;
                        }
                    }

                    await _payRepository.SaveAsync();

                    var returnResult = new VerificationPayViewModel()
                    {
                        Id = content.id,
                        RefId = content.track_id,
                        TotalPrice = content.amount,
                        UserAddress = user.UserDetail.Address,
                        UserPhoneNumber = user.PhoneNumber,
                        UserPostCode = user.UserDetail.Province != null ? user.UserDetail.Province : "",
                        UserEmail = user.Email,
                        UserFirstName = user.UserDetail.FirstName,
                        UserLastName = user.UserDetail.LastName,
                        DisCounts = payingCart.Discounts.Count() > 0 ? payingCart.Discounts.Select(p => new DisCountViewModel()
                        {
                            Name = p.CodeName,
                            Price = p.DiscountPrice
                        }) : null,
                        Products = payingCart.CartDetails.Select(p => new VerficationProductsViewModel()
                        {
                            ProductName = p.Product.Name,
                            ProductPrice = p.Product.Price,
                            TotalPrice = p.TotalPrice
                        }),
                        RetrunResult = new ResultDto()
                        {
                            SuccesMessage = $"کاربر گرامی {user.UserName} خرید شما با موفقیت ثبت شد ... شماره رهگیری {content.track_id} میباشد لطفا یادداشت کنید ...",
                            Status = true
                        }
                    };
                    return returnResult;
#pragma warning restore CS0162
                }
                else
                {
                    return PaymentError("درگاه پرداخت تراکنش را تأیید نکرد. اگر مبلغی از حساب شما کسر شده است، وضعیت آن را در سوابق بانکی بررسی کنید.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "IDPay verification failed for order {OrderId}", response?.order_id);
                return PaymentError("بررسی نتیجه‌ی پرداخت کامل نشد. لطفاً پیش از پرداخت دوباره، وضعیت سفارش خود را بررسی کنید.");
            }
        }

        public async Task<VerificationPayViewModel> VerificationZarinPall(string requestPayId, string authority, string status)
        {
            try
            {
                var requestPay = await _payRepository.GetRequestPayAsync(requestPayId);
                if (requestPay == null)
                {
                    return PaymentNotFound();
                }

                if (requestPay.IsPay)
                {
                    var existingFactor = await _payRepository.GetFactorByCartAsync(
                        requestPay.Cart.CartId,
                        requestPay.ApplicationUser.Id);
                    if (existingFactor != null)
                    {
                        return BuildVerificationResult(requestPay, existingFactor);
                    }
                }

                if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
                {
                    return PaymentError("پرداخت لغو شد یا توسط درگاه تأیید نشد. مبلغی از حساب شما کسر نخواهد شد.");
                }

                var merchantId = _configuration["Payments:Zarinpal:MerchantId"];
                if (string.IsNullOrWhiteSpace(merchantId))
                {
                    return PaymentError("تنظیمات درگاه پرداخت کامل نیست. لطفاً با پشتیبانی فروشگاه تماس بگیرید.");
                }

                var sandbox = _configuration.GetValue("Payments:Zarinpal:Sandbox", true);
                var verification = await _payment.Verification(new DtoVerification
                {
                    Amount = requestPay.Amount,
                    MerchantId = merchantId,
                    Authority = authority
                }, sandbox ? Payment.Mode.sandbox : Payment.Mode.zarinpal);

                if (verification.Status != 100 && verification.Status != 101)
                {
                    return PaymentError(GetZarinpalErrorMessage(verification.Status));
                }

                if (verification.Status == 100 || verification.Status == 101)
                {
                    var finalizedFactor = await _payRepository.FinalizePaymentAsync(
                        requestPay.Id,
                        verification.RefId);
                    if (finalizedFactor == null)
                    {
                        return PaymentError("پرداخت تأیید شد، اما ثبت سفارش کامل نشد. لطفاً با پشتیبانی تماس بگیرید.");
                    }

                    return BuildVerificationResult(requestPay, finalizedFactor);

#pragma warning disable CS0162
                    var cart = await _cartRepository.GetCartAsync(requestPay.ApplicationUser.Id);

                    requestPay.IsPay = true;
                    requestPay.DatePay = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now);
                    requestPay.RefId = verification.RefId;

                    cart.IsFinally = true;

                    foreach (var item in cart.CartDetails)
                    {
                        item.Product.Count -= item.ProductCount;
                    }

                    await _cartRepository.SaveAsync();

                    var factor = new Factor()
                    {
                        CartId = cart.CartId,
                        RefId = requestPay.RefId,
                        TotalPrice = requestPay.Amount,
                        UserAddress = requestPay.ApplicationUser.UserDetail.Address,
                        UserEmail = requestPay.ApplicationUser.Email,
                        UserName = requestPay.ApplicationUser.UserDetail.FirstName,
                        UserFamilly = requestPay.ApplicationUser.UserDetail.LastName,
                        UserPhone = requestPay.ApplicationUser.PhoneNumber,
                        UserId = requestPay.ApplicationUser.Id,
                        User = requestPay.ApplicationUser,
                        Status = FactorStatus.Progssess,
                        CreateTime = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now)
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
                        TotalPrice = requestPay.Amount,
                        UserAddress = requestPay.ApplicationUser.UserDetail.Address,
                        UserPhoneNumber = requestPay.ApplicationUser.PhoneNumber,
                        UserPostCode = requestPay.ApplicationUser.UserDetail.Province != null ? requestPay.ApplicationUser.UserDetail.Province : "",
                        UserEmail = requestPay.ApplicationUser.Email,
                        UserFirstName = requestPay.ApplicationUser.UserDetail.FirstName,
                        UserLastName = requestPay.ApplicationUser.UserDetail.LastName,
                        DisCounts = cart.Discounts.Count() > 0 ? cart.Discounts.Select(p => new DisCountViewModel()
                        {
                            Name = p.CodeName,
                            Price = p.DiscountPrice
                        }) : null,
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
#pragma warning restore CS0162

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
                        ErrorMessage = "پرداخت تکمیل نشد. لطفاً دوباره تلاش کنید یا با پشتیبانی تماس بگیرید.",
                        Status = false
                    }
                };

                return returnResult1;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Zarinpal verification failed for request {RequestPayId}", requestPayId);
                return PaymentError("بررسی نتیجه‌ی پرداخت کامل نشد. لطفاً پیش از پرداخت دوباره، وضعیت سفارش خود را بررسی کنید.");
            }
        }


        #region Privates Methode
        private static VerificationPayViewModel PaymentNotFound()
        {
            return new VerificationPayViewModel
            {
                RetrunResult = new ResultDto
                {
                    ErrorMessage = "درخواست پرداخت پیدا نشد یا منقضی شده است.",
                    ShowNotFound = true,
                    Status = false
                }
            };
        }

        private static VerificationPayViewModel PaymentError(string message)
        {
            return new VerificationPayViewModel
            {
                RetrunResult = new ResultDto
                {
                    ErrorMessage = message,
                    Status = false
                }
            };
        }

        private static string GetZarinpalErrorMessage(int status)
        {
            return status switch
            {
                -9 => "اطلاعات ارسال‌شده به درگاه معتبر نیست.",
                -10 or -11 or -15 or -16 => "درگاه پرداخت فروشگاه در دسترس نیست. لطفاً با پشتیبانی تماس بگیرید.",
                -12 => "تعداد درخواست‌های پرداخت بیش از حد مجاز است. لطفاً چند دقیقه دیگر دوباره تلاش کنید.",
                -50 => "مبلغ پرداخت‌شده با مبلغ سفارش مطابقت ندارد. لطفاً با پشتیبانی تماس بگیرید.",
                -51 => "پرداخت توسط درگاه ناموفق اعلام شد.",
                -52 or -53 or -54 => "شناسه‌ی پرداخت معتبر نیست یا به این فروشگاه تعلق ندارد.",
                _ => "درگاه پرداخت تراکنش را تأیید نکرد. لطفاً دوباره تلاش کنید."
            };
        }

        private static VerificationPayViewModel BuildVerificationResult(RequestPay requestPay, Factor factor)
        {
            var user = requestPay.ApplicationUser;
            var userDetail = user.UserDetail;
            return new VerificationPayViewModel
            {
                Id = requestPay.Id,
                RefId = factor.RefId,
                TotalPrice = factor.TotalPrice,
                UserAddress = factor.UserAddress,
                UserPhoneNumber = factor.UserPhone,
                UserPostCode = userDetail?.Province ?? string.Empty,
                UserEmail = factor.UserEmail,
                UserFirstName = factor.UserName,
                UserLastName = factor.UserFamilly,
                DisCounts = factor.Discounts?.Select(discount => new DisCountViewModel
                {
                    Name = discount.CodeName,
                    Price = discount.DiscountPrice
                }),
                Products = factor.FactorDetails?.Select(detail => new VerficationProductsViewModel
                {
                    ProductName = detail.ProductName,
                    ProductPrice = detail.ProductPrice,
                    TotalPrice = detail.TotalPrice
                }),
                RetrunResult = new ResultDto
                {
                    SuccesMessage = $"پرداخت شما با موفقیت ثبت شد. شماره پیگیری: {factor.RefId}",
                    Status = true
                }
            };
        }

        private static bool TryPrepareCart(Cart cart, out int amount, out string errorMessage)
        {
            amount = 0;
            errorMessage = null;

            if (cart?.CartDetails == null || cart.CartDetails.Count == 0)
            {
                errorMessage = "سبد خرید شما خالی است.";
                return false;
            }

            try
            {
                var cartTotal = 0;
                foreach (var detail in cart.CartDetails)
                {
                    if (detail.ProductCount <= 0)
                    {
                        errorMessage = "تعداد یکی از محصولات سبد خرید معتبر نیست.";
                        return false;
                    }

                    var availableStock = detail.Templates?.AttrinbuteTemplateCount ?? detail.Product.Count;
                    if (availableStock < detail.ProductCount)
                    {
                        errorMessage = $"موجودی «{detail.Product.Name}» برای تعداد انتخاب‌شده کافی نیست.";
                        return false;
                    }

                    var currentPrice = detail.Templates?.AttrinbuteTemplatePrice ?? detail.Product.Price;
                    if (currentPrice <= 0)
                    {
                        errorMessage = $"قیمت «{detail.Product.Name}» معتبر نیست؛ لطفاً با پشتیبانی تماس بگیرید.";
                        return false;
                    }

                    detail.ProductPrice = currentPrice;
                    detail.TotalPrice = checked(currentPrice * detail.ProductCount);
                    cartTotal = checked(cartTotal + detail.TotalPrice);
                }

                var discountTotal = cart.Discounts?.Sum(discount => discount.DiscountPrice) ?? 0;
                amount = checked(cartTotal - discountTotal);
                if (amount <= 0)
                {
                    errorMessage = "مبلغ نهایی سبد خرید معتبر نیست؛ لطفاً کد تخفیف را بررسی کنید.";
                    return false;
                }

                return true;
            }
            catch (OverflowException)
            {
                errorMessage = "مبلغ سبد خرید از محدوده‌ی مجاز بیشتر است.";
                return false;
            }
        }

        private async Task<string> CreateRequestForPayin(ApplicationUser user, int amount)
        {
            var merchantId = _configuration["Payments:Zarinpal:MerchantId"];
            if (string.IsNullOrWhiteSpace(merchantId))
            {
                throw new InvalidOperationException("Payments:Zarinpal:MerchantId is required.");
            }

            var requestPaies = await _payRepository.GetRequestPaiesAsync();
            var requestPay = requestPaies.SingleOrDefault(p => p.ApplicationUser.Id == user.Id && !p.IsPay);

            var cart = await _cartRepository.GetCartAsync(user.Id);
            var callbackBaseUrl = (_configuration["Payments:CallbackBaseUrl"] ?? string.Empty).TrimEnd('/');
            if (!Uri.TryCreate(callbackBaseUrl, UriKind.Absolute, out _))
            {
                throw new InvalidOperationException("Payments:CallbackBaseUrl must be an absolute URL.");
            }

            var sandbox = _configuration.GetValue("Payments:Zarinpal:Sandbox", true);

            var result = await _payment.Request(new DtoRequest()
            {
                Mobile = user.PhoneNumber,
                CallbackUrl = $"{callbackBaseUrl}/Account/Validate?id={Uri.EscapeDataString(requestPay.Id)}",
                Description = $"پرداخت فاکتور {cart.CartId}",
                Email = user.Email,
                Amount = Convert.ToInt32(amount),
                MerchantId = merchantId
            }, sandbox ? Payment.Mode.sandbox : Payment.Mode.zarinpal);

            var gatewayBaseUrl = sandbox
                ? "https://sandbox.zarinpal.com/pg/StartPay"
                : "https://www.zarinpal.com/pg/StartPay";
            var createReturn = $"{gatewayBaseUrl}/{result.Authority}";

            return createReturn;
        }
        private async Task<string> CreateRequestForPayingIdPayAsync(int amount, ApplicationUser user, Cart cart)
        {
            var apiKey = _configuration["Payments:IdPay:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Payments:IdPay:ApiKey is required.");
            }

            var apiUrlIdPay = @"https://api.idpay.ir/v1.1/payment";

            var callbackBaseUrl = (_configuration["Payments:CallbackBaseUrl"] ?? string.Empty).TrimEnd('/');
            if (!Uri.TryCreate(callbackBaseUrl, UriKind.Absolute, out _))
            {
                throw new InvalidOperationException("Payments:CallbackBaseUrl must be an absolute URL.");
            }

            var callBackString = $"{callbackBaseUrl}/Account/ValidateIdPay";


            var IdPayContent = new IdPaySendApiViewModel()
            {
                order_id = cart.CartId.ToString(),
                amount = amount,
                desc = $"پرداخت فاکتور {cart.CartId}",
                mail = user.Email,
                name = user.UserName,
                phone = user.PhoneNumber,
                callback = callBackString
            };

            string jsonContent = JsonConvert.SerializeObject(IdPayContent);

            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            content.Headers.Add("X-API-KEY", apiKey);
            if (_configuration.GetValue("Payments:IdPay:Sandbox", true))
            {
                content.Headers.Add("X-SANDBOX", "1");
            }

            var result = await _httpClient.PostAsync(apiUrlIdPay, content);


            var stringContent = await result.Content.ReadAsStringAsync();
            var contentReturn = JsonConvert.DeserializeObject<IdPayResponseViewModel>(stringContent);

            if (result.IsSuccessStatusCode)
            {
                var requestPaies = await _payRepository.GetRequestPaiesAsync();
                var requestPay = requestPaies.SingleOrDefault(p => p.Cart.CartId == cart.CartId);
                requestPay.IdReturnIdPay = contentReturn.id;
                requestPay.ReturnLinkIdPay = contentReturn.link;

                await _payRepository.SaveAsync();

                return contentReturn.link;
            }
            else
            {
                return contentReturn.link;
            }
        }

        #endregion


    }
}
