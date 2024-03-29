﻿using Application.InterFaces.Admin;
using Application.Utilities.TagHelper;
using Application.ViewModels;
using Application.ViewModels.Admin;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
                    .UseLogger(new DebugLogger(LogLevel.All))
                    .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                    //.SetSessionHandler(new FileSessionHandler { FilePath = userName + ".state" })
                    .Build();

                _instaApi.SetApiVersion(InstaApiVersionType.Version180);

                //var loginMember = await _instaApi.LoginAsync();

                //if (loginMember.Succeeded)
                //{
                //    Console.WriteLine("LoggedIn");
                //    return new()
                //    {
                //        Data = true,
                //        SuccesMessage = "ورود با موفقیت انجام شد.",
                //        Status = true
                //    };
                //}
                //else
                //{
                //    Console.WriteLine("Error in LoggedIn");
                //    return new()
                //    {
                //        Data = false,
                //        SuccesMessage = "خطایی در ورود به اکانت ایجاد شده است!",
                //        Status = false
                //    };
                //}
                // Call this function before calling LoginAsync
                await _instaApi.SendRequestsBeforeLoginAsync();
                // wait 5 seconds
                await Task.Delay(5000);
                var logInResult = await _instaApi.LoginAsync();
                Debug.WriteLine(logInResult.Value);
                if (logInResult.Succeeded)
                {
                    // Call this function after a successful login
                    await _instaApi.SendRequestsAfterLoginAsync();
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
                    if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                    {
                        var challenge = await _instaApi.GetChallengeRequireVerifyMethodAsync();
                        if(challenge.Succeeded)
                        {

                            Console.WriteLine("Error in LoggedIn");
                            return new()
                            {
                                Data = false,
                                SuccesMessage = "خطایی در ورود به اکانت ایجاد شده است!",
                                Status = false
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

        public async Task<ICollection<InstaMedia>> GetPostesAsync()
        {
            var userName = await _instaApi.UserProcessor.GetCurrentUserAsync();

            var medias = await _instaApi.UserProcessor.GetUserMediaAsync(userName.Value.UserName, null);
            var mediasList = medias.Value.ToList();
            return mediasList;
        }

        public async Task<ResultDto<bool>> DeleteCommentOnThePostAsync(string mediaLink, string commentId)
        {
            //try
            //{
            //    Uri uri = new(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    var deleteComment = await _instaApi.DeleteCommentAsync(mediaId.Value, commentId);

            //    if (deleteComment.Succeeded)
            //        return new()
            //        {
            //            Status = true,
            //            SuccesMessage = "حذف کامنت با موفقیت انجام شد.",
            //            Data = deleteComment.Value
            //        };
            //    else
            //        return new()
            //        {
            //            Status = false,
            //            ErrorMessage = "حدر حذف کامنت مشکلی پیش آمده است لطفا دوباره امتحان کنید."
            //        };
            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        Status = false,
            //        ErrorMessage = "حذف کامنت با مشکل مواحه شد " + e.Message
            //    };
            //}
            return null;
        }

        //public Task<ResultDto<T>> DeleteMediaAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ResultDto<InstaMediaList>> GetAllLoggedInUserPostsAsync()
        {
            try
            {
                var posts = await _instaApi.BusinessProcessor.GetPromotableMediaFeedsAsync();

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
            //try
            //{
            //    var uri = new Uri(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    var media = await _instaApi.GetMediaByIdAsync(mediaId.Value);
            //    if (media.Succeeded)
            //    {
            //        return new()
            //        {
            //            Status = true,
            //            SuccesMessage = "پست با موفقیت یافت شد.",
            //            Data = media.Value
            //        };
            //    }
            //    else
            //    {
            //        return new()
            //        {
            //            Status = false,
            //            ErrorMessage = "پستی با این لینک یافت نشد."
            //        };
            //    }
            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        Status = false,
            //        ErrorMessage = "در روند اجرای کار مشکلی پیش آمده است. متن خطا: " + e.Message
            //    };
            //}
            return null;
        }

        public async Task<ResultDto<InstaComment>> InsertCommentOnThePostAsync(string mediaLink, string text)
        {
            //try
            //{
            //    Uri uri = new Uri(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    var commnetOnMedia = await _instaApi.CommentMediaAsync(mediaId.Value, text);

            //    if (commnetOnMedia.Succeeded)
            //        return new()
            //        {
            //            SuccesMessage = "درج کامنت با موفقیت انجام شد.",
            //            Status = true,
            //            Data = commnetOnMedia.Value
            //        };
            //    else
            //        return new()
            //        {
            //            ErrorMessage = "درج کامنت با موفقیت انجام نشد. دوباره امتحان کنید.",
            //            Status = false
            //        };
            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        Status = false,
            //        ErrorMessage = "درج کامنت با مشکل مواحه شد " + e.Message
            //    };
            //}
            return null;
        }

        //public async Task<ResultDto<T>> InsertMediaAsync()
        //{
        //    try
        //    {
        //        var result=await _keepValue.Object.UploadAlbumAsync
        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //}

        public async Task<ResultDto<ResultInfo>> UploadAlbumAsync(GetProductViewModel product)
        {
            try
            {
                var images = product.Images;
                IResult<InstaMedia> result;
                if (images != null && images.Count() > 1)
                {
                    IList<InstaAlbumUpload> instaImage = new List<InstaAlbumUpload>();
                    foreach (var item in images)
                    {
                        instaImage.Add(new()
                        {
                            //URI = new Uri(Path.GetFullPath(item.ImgSrc), UriKind.Absolute).LocalPath,
                            ImageToUpload = new InstaImageUpload()
                            {
                                Uri = new Uri(Path.GetFullPath("wwwroot\\Images\\ProductImages\\" + item.ImgSrc), UriKind.Absolute).LocalPath,
                                Width = 1080,
                                Height = 1080
                            }
                        });
                    }
                    result = await _instaApi.MediaProcessor.UploadAlbumAsync(instaImage.ToArray(), product.Name + " \n " + product.Detail + " \n "  + "\n #فروش_محصول #فروش_آنلاین");
                }
                else
                {
                    var mediaUpload = new InstaImageUpload()
                    {
                        Width = 1080,
                        Height = 1080,
                        Uri = new Uri(Path.GetFullPath("wwwroot\\Images\\ProductImages\\" + images.FirstOrDefault().ImgSrc), UriKind.Absolute).LocalPath,
                    };
                    result = await _instaApi.MediaProcessor.UploadPhotoAsync(mediaUpload, product.Name + "\n" + product.Detail + " \n " + "\n #فروش_محصول #فروش_آنلاین");
                }

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
            //try
            //{
            //    Uri uri = new Uri(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    var likeMedia = await _instaApi.LikeMediaAsync(mediaId.Value);

            //    if (likeMedia.Succeeded)
            //        return new()
            //        {
            //            Status = true,
            //            SuccesMessage = "لایک کردن مدیا با موفقیت انجام شد.",
            //            Data = likeMedia.Value
            //        };
            //    else
            //        return new()
            //        {
            //            Status = false,
            //            ErrorMessage = "لایک کردن مدیا با موفقیت انجام نشد. لطفا دوباره امتحان کنید."
            //        };
            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        ErrorMessage = "خطایی در لایک کردن پیش آمده است.",
            //        Status = false
            //    };
            //}
            return null;
        }

        public async Task<ResultDto<bool>> UnLikeMediaAsync(string mediaLink)
        {
            //try
            //{
            //    Uri uri = new Uri(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    var unLikeMedia = await _instaApi.UnLikeMediaAsync(mediaId.Value);

            //    if (unLikeMedia.Succeeded)
            //        return new()
            //        {
            //            Status = true,
            //            SuccesMessage = "آن لایک کردن مدیا با موفقیت انجام شد.",
            //            Data = unLikeMedia.Value
            //        };
            //    else
            //        return new()
            //        {
            //            Status = false,
            //            ErrorMessage = "آن لایک کردن مدیا با موفقیت انجام نشد. لطفا دوباره امتحان کنید."
            //        };
            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        ErrorMessage = "خطایی در درج آن لایک کردن مدیا پیش آمده است.",
            //        Status = false
            //    };
            //}
            return null;
        }

        public async Task<ResultDto<InstaFriendshipStatus>> UsersWhoInsertCommentOnTheMediaAndFollowThemAsync(string mediaLink)
        {
            //try
            //{
            //    Uri uri = new(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    if (!mediaId.Succeeded)
            //        return new()
            //        {
            //            ErrorMessage = "مدیا یافت نشد لطفا لینک را به درستی وارد کنید و دوباره امتحان کنید.",
            //            Status = false
            //        };

            //    var comments = await _instaApi.GetMediaCommentsAsync(mediaId.Value, PaginationParameters.MaxPagesToLoad(2));
            //    if (comments.Value.Comments.Count == 0)
            //        return new()
            //        {
            //            ErrorMessage = "این پست کامنتی ندارد. لطفا لینک پستی را وارد کنید که حداقل یک کامنت داشته باشد.",
            //            Status = false                         
            //        };

            //    var usersId = comments.Value.Comments.Select(p => p.UserId).ToList();
            //    IResult<InstaFriendshipStatus> result = null;
            //    foreach (var userId in usersId)
            //    {
            //        var followUsers = await _instaApi.FollowUserAsync(userId);
            //        if (!followUsers.Succeeded)
            //        {
            //            result = followUsers;
            //            break;
            //        }
            //        Thread.Sleep(5000);
            //        result = followUsers;
            //    }

            //    if (result.Succeeded)
            //        return new()
            //        {
            //            Status = true,
            //            SuccesMessage = "کاربرانی که کامنت گذاشتن با موفقیت فالو شدند. تعداد این کاربران: " + usersId.Count() + ". توجه کنید که فقط پنجاه نفر از این افراد فالو میشوند. در صورتی که میخواهید دوباره این کار را تکرار کنید بعد از گذشت چند دقیقه انجام دهید تا برای پیجتون مشکلی پیش نیاد.",
            //            Data = result.Value
            //        };
            //    else
            //        return new()
            //        {
            //            Status = false,
            //            ErrorMessage = "در فالو کردن کاربران مشکلی پیش آمده است لطفا دوباره امتحان کنید.  متن خطا: " + result.Info
            //        };

            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        ErrorMessage = "در انجام عملیات مشکلی پیش آمده است. متن خطا: " + e.Message,
            //        Status = false
            //    };
            //}
            return null;
        }

        public async Task<ResultDto<InstaCommentList>> GetCommentsOnMediaAsync(string mediaLink)
        {
            //try
            //{
            //    Uri uri = new(mediaLink, UriKind.Absolute);
            //    var mediaId = await _instaApi.GetMediaIdFromUrlAsync(uri);
            //    var commentsList = await _instaApi.GetMediaCommentsAsync(mediaId.Value, PaginationParameters.MaxPagesToLoad(0));

            //    if (commentsList.Succeeded)
            //        return new()
            //        {
            //            Status = true,
            //            SuccesMessage = "کامنت های پست با موفقیت دریافت شد.",
            //            Data = commentsList.Value
            //        };
            //    else
            //        return new()
            //        {
            //            Status = false,
            //            ErrorMessage = " در گرفتن کامنت های پست مشکلی پیش آمده است. لطفا دوباره امتحان کنید."
            //        };
            //}
            //catch (Exception e)
            //{
            //    _logger.LogException(e);
            //    return new()
            //    {
            //        Status = false,
            //        ErrorMessage = "در گرفتن کامنت های محصول به مشکل خوردیم. پیام خطا: " + e.Message
            //    };
            //}
            return null;
        }

        public async Task<ResultDto<Object>> UploadPostToProduct(string imageUri)
        {
            try
            {
                var userName = await _instaApi.UserProcessor.GetCurrentUserAsync();

                var medias = await _instaApi.UserProcessor.GetUserMediaAsync(userName.Value.UserName, null);

                var media = medias.Value.FirstOrDefault(p => p.Images[0].Uri == imageUri);

                var date = ConverToShamsi.GetMonthAndYear(DateTime.Now);
                string folder = $@"wwwroot\Images\ProductImages\{date}\";
                var imagesName = new List<string>();
                foreach (var image in media.Images.Where(p => p.Width > 320))
                {
                    var name = DownloadRemoteImageFile(image.Uri, folder);
                    imagesName.Add(name);
                }
                string productName = "نام را مشخص کنید";
                string productDetail = media.Caption.Text;

                var mediaToAdd = new
                {
                    ProductName = productName,
                    ProductDetail = productDetail,
                    Images = imagesName,
                };

                if (media != null)
                {
                    return new()
                    {
                        Data = mediaToAdd,
                        Status = true,
                        SuccesMessage = "با موفقیت محصول یافت شد."
                    };
                }
                else
                {
                    return new()
                    {
                        Data = null,
                        ErrorMessage = "پست مورد نظر یافت نشد.",
                        Status = false
                    };
                }

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
        private static string DownloadRemoteImageFile(string uri, string fileName)
        {
            //var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            //if (!Directory.Exists(uploadsRootFolder))
            //{
            //    File.Create(uploadsRootFolder);
            //}
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //if ((response.StatusCode == HttpStatusCode.OK ||
            //    response.StatusCode == HttpStatusCode.Moved ||
            //    response.StatusCode == HttpStatusCode.Redirect) &&
            //    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            //{
            //    using Stream inputStream = response.GetResponseStream();
            //    using Stream outputStream = File.OpenWrite(uploadsRootFolder);
            //    byte[] buffer = new byte[inputStream.Length];
            //    int bytesRead;
            //    do
            //    {
            //        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
            //        outputStream.Write(buffer, 0, bytesRead);
            //    } while (bytesRead != 0);
            //    FileStream fileStream = inputStream as FileStream;
            //    return fileStream.Name;
            //}
            var todayDate = ConverToShamsi.GetMonthAndYear(DateTime.Now);
            var imageName = Guid.NewGuid() + "instagram";
            string folder = $@"wwwroot\Images\ProductImages\{todayDate}\{imageName}.png";
            var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            using (WebClient client = new())
            {
                client.DownloadFile(new Uri(uri), uploadsRootFolder);
            }
            return $@"{todayDate}/{imageName}.png";
        }
    }
}
