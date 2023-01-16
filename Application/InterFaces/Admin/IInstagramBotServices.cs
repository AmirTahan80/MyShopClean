using Application.ViewModels;
using Application.ViewModels.Admin;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface IInstagramBotServices
    {
        Task<ResultDto<bool>> LoginToInsta(string userName, string passWord);
        Task<ICollection<InstaMedia>> GetPostesAsync();
        Task<ResultDto<InstaMediaList>> GetAllLoggedInUserPostsAsync();
        Task<ResultDto<InstaMedia>> GetMediaByLinkAsync(string mediaLink);
        Task<ResultDto<InstaComment>> InsertCommentOnThePostAsync(string mediaLink, string text);
        Task<ResultDto<bool>> DeleteCommentOnThePostAsync(string mediaLink, string commentId);
        Task<ResultDto<bool>> LikeMediaAsync(string mediaLink);
        Task<ResultDto<bool>> UnLikeMediaAsync(string mediaLink);
        Task<ResultDto<InstaCommentList>> GetCommentsOnMediaAsync(string mediaLink);
        Task<ResultDto<InstaFriendshipStatus>> UsersWhoInsertCommentOnTheMediaAndFollowThemAsync(string mediaLink);
        Task<ResultDto<ResultInfo>> UploadPhotoAsync(GetProductViewModel product);
        Task<ResultDto<Object>> UploadPostToProduct(string imageUri);
    }
}
