using Application.InterFaces.Admin;
using Application.ViewModels;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class InstagramBotServices : IInstagramBotServices
    {
        private static IInstaApi _instaApi;
        //private readonly KeepValueServices _keepValue;
        private readonly IInstaLogger _logger;
        private UserSessionData _user;

        public async Task<ResultDto<bool>> LoginToInsta(string userName, string passWord)
        {
            try
            {
                var user = new UserSessionData()
                {
                    UserName = userName,
                    Password = passWord
                };

                _instaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(user)
                    .Build();

                var loginStatus = await _instaApi.LoginAsync();

                if (loginStatus.Succeeded)
                {
                    Console.WriteLine("LoggedIn");
                    return new()
                    {
                        Data = true,
                        SuccesMessage = "ورود با موفقیت انجام شد.",
                        Status = true
                    };
                }
                else
                {
                    Console.WriteLine("Error in LoggedIn");
                    return new()
                    {
                        Data = false,
                        SuccesMessage = "خطایی در ورود به اکانت ایجاد شده است!",
                        Status = false
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error In Login: " + e.Message);
                return new()
                {
                    Data = false,
                    ErrorMessage = e.Message,
                    Status = false
                };
            }
        }

        public async Task<ICollection<InstaMedia>> GetPostesAsync(string id)
        {
            var userName = await _instaApi.GetCurrentUserAsync();

            Console.WriteLine("user: " + userName);

            IResult<InstaMediaList> medias = await _instaApi.GetUserMediaAsync(userName.Value.UserName, PaginationParameters.MaxPagesToLoad(5));
            ICollection<InstaMedia> mediasList = medias.Value.ToList();
            return mediasList;
        }

        public async Task<ResultDto<bool>> DeleteCommentOnThePostAsync(string mediaLink, string commentId)
        {
            try
            {
                Uri uri = new(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                var deleteComment = await _instaApi.DeleteCommentAsync(mediaId.Value, commentId);

                if (deleteComment.Succeeded)
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "حذف کامنت با موفقیت انجام شد.",
                        Data = deleteComment.Value
                    };
                else
                    return new()
                    {
                        Status = false,
                        ErrorMessage = "حدر حذف کامنت مشکلی پیش آمده است لطفا دوباره امتحان کنید."
                    };
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    Status = false,
                    ErrorMessage = "حذف کامنت با مشکل مواحه شد " + e.Message
                };
            }
        }

        //public Task<ResultDto<T>> DeleteMediaAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ResultDto<InstaMediaList>> GetAllLoggedInUserPostsAsync()
        {
            try
            {
                var posts = await _instaApi.GetUserMediaAsync(_user.LoggedInUser.UserName, PaginationParameters.MaxPagesToLoad(5));

                if (posts.Succeeded)
                {
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "پست ها با موفقیت دریافت شدند.",
                        Data = posts.Value
                    };
                }
                else
                {
                    return new()
                    {
                        Status = false,
                        ErrorMessage = "در روند دریافت پست ها مشکلی به وجود آمد دوباره امتحان کنید."
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    ErrorMessage = "در روند اجرا مشکلی به وجود آمد. پیام خطا: " + e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResultDto<InstaMedia>> GetMediaByLinkAsync(string mediaLink)
        {
            try
            {
                var uri = new Uri(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                var media = await _instaApi.GetMediaByIdAsync(mediaId.Value);
                if (media.Succeeded)
                {
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "پست با موفقیت یافت شد.",
                        Data = media.Value
                    };
                }
                else
                {
                    return new()
                    {
                        Status = false,
                        ErrorMessage = "پستی با این لینک یافت نشد."
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    Status = false,
                    ErrorMessage = "در روند اجرای کار مشکلی پیش آمده است. متن خطا: " + e.Message
                };
            }
        }

        public async Task<ResultDto<InstaComment>> InsertCommentOnThePostAsync(string mediaLink, string text)
        {
            try
            {
                Uri uri = new Uri(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                var commnetOnMedia = await _instaApi.CommentMediaAsync(mediaId.Value, text);

                if (commnetOnMedia.Succeeded)
                    return new()
                    {
                        SuccesMessage = "درج کامنت با موفقیت انجام شد.",
                        Status = true,
                        Data = commnetOnMedia.Value
                    };
                else
                    return new()
                    {
                        ErrorMessage = "درج کامنت با موفقیت انجام نشد. دوباره امتحان کنید.",
                        Status = false
                    };
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    Status = false,
                    ErrorMessage = "درج کامنت با مشکل مواحه شد " + e.Message
                };
            }
        }

        //public async Task<ResultDto<T>> InsertMediaAsync()
        //{
        //    try
        //    {
        //        var result=await _keepValue.Object.UploadPhotoAsync
        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //}

        public async Task<ResultDto<ResultInfo>> UploadPhotoAsync(string mediaLink)
        {
            try
            {
                var mediaUri = new Uri(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(mediaUri);
                var media = await _instaApi.GetMediaByIdAsync(mediaId.Value);

                string ssss = media.Value.Images.Select(p => p.URI).FirstOrDefault()!;
                InstaImage instaImage = new()
                {
                    URI = new Uri(Path.GetFullPath(ssss), UriKind.Absolute).LocalPath,
                    Height = media.Value.Images.First().Height,
                    Width = media.Value.Images.First().Width
                };

                var result = await _instaApi.UploadPhotoAsync(instaImage, media.Value.Caption.Text);


                if (result.Succeeded)
                    return new()
                    {
                        SuccesMessage = "عکس با موفقیت آپلود شد.",
                        Status = true,
                        Data = result.Info
                    };
                else
                    return new()
                    {
                        ErrorMessage = "آپلود عکس با شکست موجه شد.",
                        Status = false,
                        Data = result.Info
                    };
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    ErrorMessage = "در آپلود عکس مشکلی پیش آمده است. پیام خطا : " + e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResultDto<bool>> LikeMediaAsync(string mediaLink)
        {
            try
            {
                Uri uri = new Uri(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                var likeMedia = await _instaApi.LikeMediaAsync(mediaId.Value);

                if (likeMedia.Succeeded)
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "لایک کردن مدیا با موفقیت انجام شد.",
                        Data = likeMedia.Value
                    };
                else
                    return new()
                    {
                        Status = false,
                        ErrorMessage = "لایک کردن مدیا با موفقیت انجام نشد. لطفا دوباره امتحان کنید."
                    };
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    ErrorMessage = "خطایی در لایک کردن پیش آمده است.",
                    Status = false
                };
            }
        }

        public async Task<ResultDto<bool>> UnLikeMediaAsync(string mediaLink)
        {
            try
            {
                Uri uri = new Uri(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                var unLikeMedia = await _instaApi.UnLikeMediaAsync(mediaId.Value);

                if (unLikeMedia.Succeeded)
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "آن لایک کردن مدیا با موفقیت انجام شد.",
                        Data = unLikeMedia.Value
                    };
                else
                    return new()
                    {
                        Status = false,
                        ErrorMessage = "آن لایک کردن مدیا با موفقیت انجام نشد. لطفا دوباره امتحان کنید."
                    };
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    ErrorMessage = "خطایی در درج آن لایک کردن مدیا پیش آمده است.",
                    Status = false
                };
            }
        }

        public async Task<ResultDto<InstaFriendshipStatus>> UsersWhoInsertCommentOnTheMediaAndFollowThemAsync(string mediaLink)
        {
            try
            {
                Uri uri = new(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                if (!mediaId.Succeeded)
                    return new()
                    {
                        ErrorMessage = "مدیا یافت نشد لطفا لینک را به درستی وارد کنید و دوباره امتحان کنید.",
                        Status = false
                    };

                var comments = await _instaApi.GetMediaCommentsAsync(mediaId.Value, PaginationParameters.MaxPagesToLoad(2));
                if (comments.Value.Comments.Count == 0)
                    return new()
                    {
                        ErrorMessage = "این پست کامنتی ندارد. لطفا لینک پستی را وارد کنید که حداقل یک کامنت داشته باشد.",
                        Status = false                         
                    };

                var usersId = comments.Value.Comments.Select(p => p.UserId).ToList();
                IResult<InstaFriendshipStatus> result = null;
                foreach (var userId in usersId)
                {
                    var followUsers = await _instaApi.FollowUserAsync(userId);
                    if (!followUsers.Succeeded)
                    {
                        result = followUsers;
                        break;
                    }
                    Thread.Sleep(5000);
                    result = followUsers;
                }

                if (result.Succeeded)
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "کاربرانی که کامنت گذاشتن با موفقیت فالو شدند. تعداد این کاربران: " + usersId.Count() + ". توجه کنید که فقط پنجاه نفر از این افراد فالو میشوند. در صورتی که میخواهید دوباره این کار را تکرار کنید بعد از گذشت چند دقیقه انجام دهید تا برای پیجتون مشکلی پیش نیاد.",
                        Data = result.Value
                    };
                else
                    return new()
                    {
                        Status = false,
                        ErrorMessage = "در فالو کردن کاربران مشکلی پیش آمده است لطفا دوباره امتحان کنید.  متن خطا: " + result.Info
                    };

            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    ErrorMessage = "در انجام عملیات مشکلی پیش آمده است. متن خطا: " + e.Message,
                    Status = false
                };
            }
        }

        public async Task<ResultDto<InstaCommentList>> GetCommentsOnMediaAsync(string mediaLink)
        {
            try
            {
                Uri uri = new(mediaLink, UriKind.Absolute);
                var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
                var commentsList = await _instaApi.GetMediaCommentsAsync(mediaId.Value, PaginationParameters.MaxPagesToLoad(0));

                if (commentsList.Succeeded)
                    return new()
                    {
                        Status = true,
                        SuccesMessage = "کامنت های پست با موفقیت دریافت شد.",
                        Data = commentsList.Value
                    };
                else
                    return new()
                    {
                        Status = false,
                        ErrorMessage = " در گرفتن کامنت های پست مشکلی پیش آمده است. لطفا دوباره امتحان کنید."
                    };
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new()
                {
                    Status = false,
                    ErrorMessage = "در گرفتن کامنت های محصول به مشکل خوردیم. پیام خطا: " + e.Message
                };
            }
        }
    }
}
