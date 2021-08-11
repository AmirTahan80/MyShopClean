using Application.ViewModels;
using Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface ICommentUserServices
    {
        /// <summary>
        /// افزودن نظر کاربر
        /// </summary>
        /// <param name="addComment"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> AddUserCommentAsync(CommentViewModel addComment, string userId);
        /// <summary>
        /// گرفتن نظرات
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<CommentsViewModel>> GetProductComments(int id);
        /// <summary>
        /// گرفتن تمام نظرات کاربران
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ProfileViewModel> GetUserComments(string userId);
    }
}
