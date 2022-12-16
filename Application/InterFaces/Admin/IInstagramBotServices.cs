using Application.ViewModels;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface IInstagramBotServices
    {
        Task<ResultDto<bool>> LoginToInsta(string userName, string passWord);
        Task<ICollection<InstaMedia>> GetPostesAsync(string id);
        Task<ResultDto<InstaMediaList>> GetAllLoggedInUserPostsAsync();
        Task<ResultDto<InstaMedia>> GetMediaByLinkAsync(string mediaLink);
        Task<ResultDto<InstaComment>> InsertCommentOnThePostAsync(string mediaLink, string text);
        Task<ResultDto<bool>> DeleteCommentOnThePostAsync(string mediaLink, string commentId);
        Task<ResultDto<bool>> LikeMediaAsync(string mediaLink);
        Task<ResultDto<bool>> UnLikeMediaAsync(string mediaLink);
        Task<ResultDto<InstaCommentList>> GetCommentsOnMediaAsync(string mediaLink);
        Task<ResultDto<InstaFriendshipStatus>> UsersWhoInsertCommentOnTheMediaAndFollowThemAsync(string mediaLink);
        Task<ResultDto<ResultInfo>> UploadPhotoAsync(string mediaLink);
    }
}
