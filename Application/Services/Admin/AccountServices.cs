using Application.InterFaces.Admin;
using Application.InterFaces.Both;
using Application.ViewModels.Admin;
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

namespace Application.Services.Admin
{
    public class AccountServices : IAccountServices
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageSenderServices _messageSender;
        private static IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;
        private readonly LinkGenerator _linkGenerator;
        private readonly RoleManager<RoleModel> _roleManager;
        public AccountServices(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IMessageSenderServices messageSender, IHostingEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator, RoleManager<RoleModel> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _messageSender = messageSender;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _roleManager = roleManager;
        }


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
                var applicationUser = new ApplicationUser()
                {
                    UserName = user.UserName,
                    Email = user.UserEmail,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(applicationUser, userPassword);
                if (result.Succeeded)
                {
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

        public async Task<IList<UsersListViewModel>> GetAllUsersListAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersReturn = users.Select(p => new UsersListViewModel()
            {
                UserName = p.UserName,
                UserEmail = p.Email,
                RoleName = _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault()
            }).ToList();
            return usersReturn;
        }
        public async Task<UserDetailViewModel> FinUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userReturn = new UserDetailViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                RoleName = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()
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

        //Tag Helpers
        async Task SendEmailAsync(ApplicationUser user, string passForReturn = "ConfirmEmail")
        {

            var emailConfiguration = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callBackUrl = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
    action: passForReturn, "AccountManager",
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
        }

    }
}
