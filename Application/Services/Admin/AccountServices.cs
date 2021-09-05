using Application.InterFaces.Admin;
using Application.InterFaces.Both;
using Application.Utilities.TagHelper;
using Application.ViewModels;
using Application.ViewModels.Admin;
using Domain.InterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class AccountServices : IAccountServices
    {
        #region Injections
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageSenderServices _messageSender;
        private readonly IHostingEnvironment _env;
        private readonly RoleManager<RoleModel> _roleManager;
        private readonly ICommentRepository _commentRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IPayRepository _payRepository;
        private readonly IContactUsRepository _contactUsrepository;

        public AccountServices(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IMessageSenderServices messageSender, IHostingEnvironment env, RoleManager<RoleModel> roleManager,
            ICommentRepository commentRepository,
            IQuestionRepository questionRepository, IPayRepository payRepository, IContactUsRepository contactUsrepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _messageSender = messageSender;
            _env = env;
            _roleManager = roleManager;
            _commentRepository = commentRepository;
            _questionRepository = questionRepository;
            _payRepository = payRepository;
            _contactUsrepository = contactUsrepository;
        }
        #endregion


        //Impeliments
        public async Task<bool> CreateUserByEmailAndPass(CreateAccountViewModel user, string userPassword)
        {
            try
            {
                var isUserNameExist = await _userManager.FindByNameAsync(user.UserName);
                var isUserEmailExist = await _userManager.FindByEmailAsync(user.UserEmail);
                if (isUserEmailExist != null || isUserNameExist != null)
                {
                    return false;
                }

                var userDetail = new UserDetail();

                var applicationUser = new ApplicationUser()
                {
                    UserName = user.UserName,
                    Email = user.UserEmail,
                    EmailConfirmed = true,
                    UserDetail= userDetail
                };
                var result = await _userManager.CreateAsync(applicationUser, userPassword);
                if (result.Succeeded)
                {

                    var findRoleById = await _roleManager.FindByNameAsync(user.RoleId);
                    if (findRoleById != null)
                    {
                        var resultAddToRole = await _userManager.AddToRoleAsync(applicationUser, findRoleById.Name);
                        if (!resultAddToRole.Succeeded)
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public async Task<IList<UsersListViewModel>> GetAllUsersListAsync(string userLoginId)
        {

            var users = await _userManager.Users.ToListAsync();

            var userLogin = await _userManager.FindByIdAsync(userLoginId);
            var userLoginRoles = await _userManager.GetRolesAsync(userLogin);
            var userLoginRole = userLoginRoles.FirstOrDefault();

            if (userLoginRole == "Manager")
            {
                users = users.Where(p => _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Founder" &&
                 _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Manager").ToList();
            }
            else if (userLoginRole == "Writer")
            {
                users = users.Where(p => _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Founder" &&
                 _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Manager" &&
                 _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Writer").ToList();
            }
            else if (userLoginRole == "Customer")
            {
                users = users.Where(p => _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Founder" &&
                 _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Manager" &&
                 _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Writer" &&
                 _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault() != "Customer").ToList();
            }

            var usersReturn = users.Select(p => new UsersListViewModel()
            {
                UserName = p.UserName,
                UserEmail = p.Email,
                RoleName = _roleManager.Roles.SingleOrDefault(c=>c.Name==_userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault())
                .RoleNamePersian,
                UserId = p.Id
            }).ToList();
            return usersReturn;
        }
        public async Task<UserDetailViewModel> FinUserById(string userId, string userLoginId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var correctRolesGet = await GetRoles(userLoginId);

            var userRole = await _userManager.GetRolesAsync(user);

            var userReturn = new UserDetailViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                RoleName = userRole.FirstOrDefault(),
                RolesName = correctRolesGet
            };
            return userReturn;
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
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public async Task<bool> EditUserAsync(UserDetailViewModel editUser)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(editUser.UserId);
                if (user == null) return false;
                var updateUser = new ApplicationUser()
                {
                    UserName = editUser.UserName,
                    Email = editUser.UserEmail,
                };
                var userRoles =await _userManager.GetRolesAsync(user);
                var userRole = userRoles.SingleOrDefault();
                if (userRole != editUser.RoleName)
                {
                    var resultFordeleteRole = await _userManager.RemoveFromRoleAsync(user, userRole);
                    if (!resultFordeleteRole.Succeeded)
                        return false;
                    else
                    {
                        var resultForAddToRole = await _userManager.AddToRoleAsync(user, editUser.RoleName);
                        if (!resultForAddToRole.Succeeded)
                            return false;
                    }
                }
                var result = await _userManager.UpdateAsync(updateUser);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }
        public async Task<bool> DeleteUsersAsync(IEnumerable<UsersListViewModel> deleteUsers)
        {
            try
            {
                var deleteUsersSelected = deleteUsers.Where(p => p.IsSelected).Select(p => p.UserId).ToList();

                foreach (var userId in deleteUsersSelected)
                {
                    var findUserById = await _userManager.FindByIdAsync(userId);
                    var result = await _userManager.DeleteAsync(findUserById);
                    if (!result.Succeeded)
                    {
                        return false;
                    }
                }


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<List<SelectListItem>> GetRolesAsync(string userLoginId)
        {
            var reslut = await GetRoles(userLoginId);

            return reslut;
        }

        public async Task<IEnumerable<CommentsViewModel>> GetCommentsAsync()
        {
            var comments = await _commentRepository.GetCommentsAsync();

            var resturnComment = comments.Select(p => new CommentsViewModel()
            {
                CommentId = p.CommentId,
                CommentText = p.CommentText,
                CommentTopic = p.CommentTopic,
                InsertTime = p.CommentInsertTime,
                IsShow = p.IsShow,
                ProductImage = p.Product.ProductImages.FirstOrDefault().ImgFile + "/" + p.Product.ProductImages.FirstOrDefault().ImgSrc,
                ProductName = p.Product.Name,
                Suggest = p.Suggest,
                UserEmail = p.User.Email,
                UserName = p.User.UserName,
                UserId = p.User.Id
            });

            return resturnComment;
        }
        public async Task<CommentsViewModel> GetCommentAsync(int commentId)
        {
            var comment = await _commentRepository.GetCommentAsync(commentId);

            var commentReturn = new CommentsViewModel()
            {
                CommentId = comment.CommentId,
                CommentText = comment.CommentText,
                CommentTopic = comment.CommentTopic,
                InsertTime = comment.CommentInsertTime,
                IsShow = comment.IsShow,
                ProductImage = comment.Product.ProductImages.FirstOrDefault().ImgFile + "/" + comment.Product.ProductImages.FirstOrDefault().ImgSrc,
                ProductName = comment.Product.Name,
                Suggest = comment.Suggest,
                UserEmail = comment.User.Email,
                UserId = comment.UserId,
                UserName = comment.User.UserName,
                Goodness = comment.ProductGoodNess != null ? comment.ProductGoodNess.Split(",") : null,
                Bads = comment.ProductGoodNess != null ? comment.ProductGoodNess.Split(",") : null,
                ProductId = comment.ProductId
            };

            return commentReturn;
        }
        public async Task<ResultDto> EditCommentAsync(CommentsViewModel editComment)
        {
            try
            {
                var returnResultDto = new ResultDto();

                var comment = await _commentRepository.GetCommentAsync(editComment.CommentId);
                if (comment == null)
                {
                    returnResultDto.ErrorMessage = "ویرایش کامنت با شکست مواجه شد !!! صفحه را رفرش کنید و دوباره سعی کنید !!!";
                    returnResultDto.Status = false;
                    return returnResultDto;
                }

                comment.CommentTopic = editComment.CommentTopic;
                comment.CommentText = editComment.CommentText;
                comment.IsShow = true;

                await _commentRepository.SaveAsync();

                returnResultDto.SuccesMessage = "ویرایش و نمایش نظر در سایت با موفقیت اعمال شد ...";
                returnResultDto.Status = true;

                return returnResultDto;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResultDto = new ResultDto()
                {
                    ErrorMessage = "مشکلی در ویرایش نظر به وجود آمده است لطفا دقایقی دیگر امتحان کنید و در صورت وجود مشکل با پشتیبانی تماس بگیرید !!!",
                    Status = false
                };
                return returnResultDto;
            }
        }

        public async Task<IList<QuestionViewModel>> GetQuestionsAsync()
        {
            var questions = await _questionRepository.GetQuestionsAsync();

            var retunrQuestions = questions.Select(p => new QuestionViewModel()
            {
                Id = p.Id,
                Email = p.User.Email,
                UserId = p.User.Id,
                ProductName = p.Product.Name,
                ProductImage = p.Product.ProductImages.FirstOrDefault().ImgFile + "/" + p.Product.ProductImages.FirstOrDefault().ImgSrc,
                ProductId = p.Product.Id,
                Text = p.QuestionText,
                Topic = p.Topic,
                Replaise = p.Replais.Count() > 0 ? p.Replais.Select(c => new QuestionViewModel()
                {
                    Id = c.Id,
                    Email = c.User.Email,
                    UserId = c.User.Id,
                    ProductName = c.Product.Name,
                    ProductImage = c.Product.ProductImages.FirstOrDefault().ImgFile + "/" + p.Product.ProductImages.FirstOrDefault().ImgSrc,
                    ProductId = c.Product.Id,
                    Text = c.QuestionText,
                    Topic = c.Topic
                }).ToList() : null
            }).ToList();

            return retunrQuestions;
        }
        public async Task<QuestionViewModel> GetQuestionAsync(int questionId)
        {
            var question = await _questionRepository.GetQuestionAsync(questionId);

            var retrunQuestion = new QuestionViewModel()
            {
                Id = question.Id,
                UserId = question.User.Id,
                Email = question.User.Email,
                Text = question.QuestionText,
                Topic = question.Topic
            };

            return retrunQuestion;
        }
        public async Task<ResultDto> EditQuestionAsync(QuestionViewModel editQuestion, string userId)
        {
            try
            {
                var question = await _questionRepository.GetQuestionAsync(editQuestion.Id);
                var user = await _userManager.FindByIdAsync(userId);

                var createQuestion = new Question()
                {
                    Product = question.Product,
                    QuestionText = editQuestion.Awnser,
                    User = user,
                    ReplayOn = question,
                    Topic = ""
                };

                await _questionRepository.AddQuestionAsync(createQuestion);

                question.Replais.Add(createQuestion);

                await _questionRepository.SaveAsync();


                var retrunResult = new ResultDto()
                {
                    SuccesMessage = "ثبت تغییرا با موفقیت انجام شد .",
                    Status = true
                };
                return retrunResult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var retrunResult = new ResultDto()
                {
                    ErrorMessage = "مشکلی در ثبت پاسخ به وجود آمده است !!!",
                    Status = false
                };
                return retrunResult;
            }
        }
        public async Task<ResultDto> DeleteQuestionAsync(IList<QuestionViewModel> deleteQuestions)
        {
            try
            {
                var questionSelect = deleteQuestions.Where(p => p.IsSelected).Select(p => p.Id).ToList();

                var questions = new List<Question>();
                foreach (var questionId in questionSelect)
                {
                    var questionFind = await _questionRepository.GetQuestionAsync(questionId);
                    questions.Add(questionFind);
                    if (questionFind.Replais.Count() > 0)
                    {
                        foreach (var questionReplay in questionFind.Replais)
                        {
                            questions.Add(questionReplay);
                        }
                    }
                }

                questions = questions.OrderByDescending(p => p.Id).ToList();

                _questionRepository.DeleteQuestionsAsync(questions);

                await _questionRepository.SaveAsync();

                var returnResultDto = new ResultDto()
                {
                    SuccesMessage = "حذف سوالات با موفقیت انحام شد ...",
                    Status = true
                };
                return returnResultDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResultDto = new ResultDto()
                {
                    ErrorMessage = "مشکلی در حذف سوالات به وجود آمده است لطفا دوباره تلاش کنید !!!",
                    Status = false
                };
                return returnResultDto;
            }
        }

        public async Task<IEnumerable<FactorViewModel>> GetFactorsAsync()
        {
            var factors = await _payRepository.GetFactors();

            var returnFactors = factors.Select(p => new FactorViewModel()
            {
                Id = p.Id,
                UserName = p.UserName,
                UserEmail = p.UserEmail,
                FactorStatus = p.Status.ToString() == "Progssess" ? "در حال انجام" : p.Status.ToString() == "Cansel" ? "لغو شده" : p.Status.ToString() == "Success" ? "اتمام یافته" : "",
                UserId = p.UserId,
                UserAddress = p.UserAddress,
                PhoneNumber = p.UserPhone,
                DisCountNames = p.Discounts != null ? p.Discounts.Select(p => p.CodeName).ToList() : null,
                UseDisCount = p.Discounts != null ? true : false,
                RefId = p.RefId,
                TotalPrice = p.TotalPrice
            });

            return returnFactors;
        }
        public async Task<FactorViewModel> GetFactorAsync(int id)
        {
            var factors = await _payRepository.GetFactors();
            var factor = factors.SingleOrDefault(p => p.Id == id);

            var returnFactor = new FactorViewModel()
            {
                Id = factor.Id,
                UserName = factor.UserName,
                UserEmail = factor.UserEmail,
                FactorStatus = factor.Status.ToString(),
                UserId = factor.UserId,
                UserAddress = factor.UserAddress,
                PhoneNumber = factor.UserPhone,
                DisCountNames = factor.Discounts != null ? factor.Discounts.Select(p => p.CodeName).ToList() : null,
                DisCountPrices = factor.Discounts != null ? factor.Discounts.Select(p => p.DiscountPrice).ToList() : null,
                FactorDetails = factor.FactorDetails.Select(c => new FactorDetailViewModel()
                {
                    Id = c.Id,
                    Count = c.ProductCount,
                    ProducPrice = c.ProductPrice,
                    ProductName = c.ProductName,
                    ProductTemplate = c.AttributesValue,
                    TotalPrice = c.TotalPrice,
                    ProductImage = c.ImageSrc
                }).ToList(),
                RefId = factor.RefId,
                TotalPrice = factor.TotalPrice,
                UseDisCount = factor.Discounts != null ? true : false
            };

            return returnFactor;
        }
        public async Task<ResultDto> EditFactorAsync(FactorViewModel editFactor)
        {
            try
            {
                var factors = await _payRepository.GetFactors();
                var factor = factors.SingleOrDefault(p => p.Id == editFactor.Id);

                factor.Status = editFactor.FactorStatus == "Progssess" ? FactorStatus.Progssess : editFactor.FactorStatus == "Cansel" ? FactorStatus.Cansel : editFactor.FactorStatus == "Success" ? FactorStatus.Success : FactorStatus.Progssess;

                await _payRepository.SaveAsync();

                var returnResult = new ResultDto()
                {
                    SuccesMessage = "تغییرا با موفقیت اعمال شد ...",
                    Status = true
                };
                return returnResult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در ویرایش سفارش مشکلی پیش آمده است لطفا دوباره تلاش کنید !!!",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<IEnumerable<ContactViewModel>> GetContactsAsync()
        {
            var contactsUs = await _contactUsrepository.GetContactsUsAsync();

            var returnContactUs = contactsUs.Select(p => new ContactViewModel()
            {
                UserEmail = p.User.Email,
                Text = p.Text,
                Topic = p.Topic,
                Id = p.Id,
                Awnser = p.Awnser
            });

            return returnContactUs;
        }
        public async Task<ContactViewModel> GetContactDetailAsync(int id)
        {
            var contactsUs = await _contactUsrepository.GetContactsUsAsync();
            var contact = contactsUs.SingleOrDefault(p => p.Id == id);

            var returnContact = new ContactViewModel()
            {
                UserEmail = contact.User.Email,
                Text = contact.Text,
                Topic = contact.Topic,
                UserId = contact.User.Id,
                Id = contact.Id,
                Awnser = contact.Awnser
            };

            return returnContact;
        }
        public async Task<ResultDto> AwnserAsync(ContactViewModel awnserContact)
        {
            try
            {
                var contacts = await _contactUsrepository.GetContactsUsAsync();
                var contact = contacts.SingleOrDefault(p => p.Id == awnserContact.Id);

                contact.IsHaveAwnser = true;
                contact.Awnser = awnserContact.Awnser;
                contact.AwnserTime = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now);

                await SendEmailAsync(awnserContact);

                await _contactUsrepository.SaveAsync();

                var returnResult = new ResultDto()
                {
                    SuccesMessage = "پاسخ به درستی برای ایمیل کاربر ارسال شد ...",
                    Status = true
                };
                return returnResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "ثبت درخواست با مشکل مواجه شد ... دوباره امتحان کنید و اگر به مشکل خوردید به پشتیبان سایت تماس بگیرید !!!",
                    Status = false
                };
                return returnResult;
            }
        }

        #region PrivateMethodes
        async Task SendEmailAsync(ContactViewModel contact)
        {
            string message = contact.Awnser;

            var pathToFile = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate"
                    + Path.DirectorySeparatorChar.ToString()
                    + "SendAwnserEmailTemplate.html";
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
            builder = builder.Replace("{Awnser}", message);
            await _messageSender.SendEmailAsync(contact.UserEmail, "", builder, true);
        }
        private async Task<List<SelectListItem>> GetRoles(string userLoginId)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var userLoginSite = await _userManager.FindByIdAsync(userLoginId);
            var userLoginRoles = await _userManager.GetRolesAsync(userLoginSite);
            var userLoginRole = userLoginRoles.FirstOrDefault();

            var correctRolesGet = new List<RoleModel>();

            if (userLoginRole == "Manager")
            {
                correctRolesGet = roles.Where(p => p.Name != "Founder" && p.Name != "Manager").ToList();
            }
            else if (userLoginRole == "Writer")
            {
                correctRolesGet = roles.Where(p => p.Name != "Founder" && p.Name != "Manager" && p.Name != "Writer").ToList();
            }
            else if (userLoginRole == "Customer")
            {
                correctRolesGet = roles.Where(p => p.Name != "Founder" && p.Name != "Manager" && p.Name != "Writer" && p.Name != "Customer").ToList();
            }
            else if (userLoginRole == "Founder")
            {
                correctRolesGet = roles.ToList();
            }

            var listOfRoleItem = new List<SelectListItem>();

            foreach (var role in correctRolesGet)
            {
                listOfRoleItem.Add(new SelectListItem()
                {
                    Value = role.Name,
                    Text = role.RoleNamePersian
                });
            }

            return listOfRoleItem;
        }
        #endregion

    }
}