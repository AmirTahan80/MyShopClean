using Application.InterFaces.Admin;
using Application.InterFaces.Both;
using Application.ViewModels.Admin;
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

                    var findRoleById = await _roleManager.FindByIdAsync(user.RoleId);
                    if(findRoleById!=null)
                    {
                        var resultAddToRole=await _userManager.AddToRoleAsync(applicationUser, findRoleById.Name);
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
                users = users.Where(p => _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault()!= "Founder" &&
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
                RoleName = _userManager.GetRolesAsync(p).GetAwaiter().GetResult().FirstOrDefault(),
                UserId=p.Id
            }).ToList();
            return usersReturn;
        }
        public async Task<UserDetailViewModel> FinUserById(string userId, string userLoginId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _roleManager.Roles.ToListAsync();

            var findUserLogIn = await _userManager.FindByIdAsync(userLoginId);
            var userLogInRoles = await _userManager.GetRolesAsync(findUserLogIn);
            var userLogInRole = userLogInRoles.FirstOrDefault();

            var correctRolesGet = new List<RoleModel>();

            if (userLogInRole == "Manager")
            {
                correctRolesGet = roles.Where(p => p.Name != "Founder" && p.Name!="Manager").ToList();
            }
            else if(userLogInRole == "Writer")
            {
                correctRolesGet = roles.Where(p => p.Name != "Founder" && p.Name!="Manager" && p.Name!="Writer").ToList();
            }
            else if (userLogInRole == "Customer")
            {
                correctRolesGet = roles.Where(p => p.Name != "Founder" && p.Name != "Manager" && p.Name != "Writer" && p.Name != "Customer").ToList();
            }
            else if (userLogInRole == "Founder")
            {
                correctRolesGet = roles.ToList();
            }

            var rolesItem = new List<SelectListItem>();
            foreach (var role in correctRolesGet)
            {
                rolesItem.Add(new SelectListItem()
                {
                    Value = role.Id,
                    Text = role.Name == "Manager" ? "ادمین | مدیریت کننده" : role.Name == "Writer" ? "نویسنده" : role.Name == "Customer" ? "مشتری" : role.Name == "Founder" ? "سازنده سایت" : ""
                });
            }

            var userReturn = new UserDetailViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                RoleName = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault(),
                RolesName=rolesItem
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
                var userRole = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().SingleOrDefault();
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
                    return true;
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
                    var result=await _userManager.DeleteAsync(findUserById);
                    if(!result.Succeeded)
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
                    Value=role.Id,
                    Text = role.Name == "Manager" ? "ادمین | مدیریت کننده" : role.Name == "Writer" ? "نویسنده" : role.Name == "Customer" ? "مشتری" : role.Name == "Founder" ? "سازنده سایت" : ""
                });
            }

            return listOfRoleItem;
        }

    }
}
