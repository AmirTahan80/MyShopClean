using Application.ViewModels;
using Application.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        Task<IList<UsersListViewModel>> GetAllUsersListAsync(string userLoginId);
        /// <summary>
        /// یافتن و یا پیداکردن یک کاربر با آیدی
        /// Find an User from Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>UserDetailViewModel</returns>
        Task<UserDetailViewModel> FinUserById(string userId,string userLoginId);
        /// <summary>
        /// ساخت و جنریت لینک برای تاییدیه ایمیل و ارسال به جیمیل کاربر
        /// Generate Link And Send To User Email For Confirm Email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> ConfirmEmailAsync(string userName, string token);
        /// <summary>
        /// ویرایش کاربر
        /// Edit User
        /// </summary>
        /// <param name="editUser"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> EditUserAsync(UserDetailViewModel editUser);
        /// <summary>
        /// حذف لیستی از کاربران
        /// Delete Users
        /// </summary>
        /// <param name="editUser"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> DeleteUsersAsync(IEnumerable<UsersListViewModel> deleteUser);
        /// <summary>
        /// گرفتن تمامی مقام با شرط کاربران بر اساس مقام آنها
        /// GetAllRoles
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <returns>List of selectListItem Of Roles</returns>
        Task<List<SelectListItem>> GetRolesAsync(string userLoginId);
        /// <summary>
        /// گرفتن تمامی نظرات
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CommentsViewModel>> GetCommentsAsync();
        /// <summary>
        /// جزئیات نظر
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        Task<CommentsViewModel> GetCommentAsync(int commentId);
        /// <summary>
        /// ویرایش نظر
        /// </summary>
        /// <param name="editComment"></param>
        /// <returns></returns>
        Task<ResultDto> EditCommentAsync(CommentsViewModel editComment);
    }
}
