using Application.ViewModels.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface IAccountServices
    {
        /// <summary>
        /// ساخت اکانت کاربری با ایمیل و رمز عبور
        /// Create User With PassWord And Email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userPassword"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> CreateUserByEmailAndPass(CreateAccountViewModel user, string userPassword);
        /// <summary>
        /// گرفتن تمام کاربران از دیتابیس
        /// Get All Users From Data Base
        /// </summary>
        /// <returns>IList<UsersListViewModel></returns>
        Task<IList<UsersListViewModel>> GetAllUsersListAsync();
        /// <summary>
        /// یافتن و یا پیداکردن یک کاربر با آیدی
        /// Find an User from Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>UserDetailViewModel</returns>
        Task<UserDetailViewModel> FinUserById(string userId);
        /// <summary>
        /// ساخت و جنریت لینک برای تاییدیه ایمیل و ارسال به جیمیل کاربر
        /// Generate Link And Send To User Email For Confirm Email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> ConfirmEmailAsync(string userName, string token);
    }
}
