using Application.InterFaces.User;
using Application.Utilities.TagHelper;
using Application.ViewModels;
using Application.ViewModels.User;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class CommentUserServices : ICommentUserServices
    {
        #region Injections
        private readonly ICommentRepository _commentRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentUserServices(ICommentRepository commentRepository,
            IProductRepository productRepository,
            UserManager<ApplicationUser> userManager)
        {
            _commentRepository = commentRepository;
            _productRepository = productRepository;
            _userManager = userManager;
        }
        #endregion

        public async Task<ResultDto> AddUserCommentAsync(CommentViewModel addComment, string userId)
        {
            try
            {
                var returnResult = new ResultDto();

                var product = await _productRepository.GetProductAsync(addComment.ProductId);
                if (product == null)
                {
                    returnResult.ErrorMessage = "مشکلی در افزودن نظر شما به وجود آمده است !!";
                    returnResult.Status = false;
                    return returnResult;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    returnResult.ErrorMessage = "مشکلی در افزودن نظر شما به وجود آمده است !!";
                    returnResult.Status = false;
                    return returnResult;
                }

                var parent = new Comment();

                if (addComment.ReplayId != 0)
                    parent = await _commentRepository.GetCommentAsync(addComment.ReplayId);
                else
                    parent = null;

                var goodNes = "";

                if(addComment.Goodness!=null)
                {
                    for (int i = 0; i < addComment.Goodness.Count(); i++)
                    {
                        if(i==0)
                        {
                            goodNes = addComment.Goodness[i];
                        }
                        else
                        {
                            goodNes += "," + addComment.Goodness[i];
                        }
                    }
                }


                var bads = "";

                if (addComment.Bads != null)
                {
                    for (int i = 0; i < addComment.Bads.Count(); i++)
                    {
                        if (i == 0)
                        {
                            bads = addComment.Bads[i];
                        }
                        else
                        {
                            bads += "," + addComment.Bads[i];
                        }
                    }
                }


                var commentcreate = new Comment()
                {
                    CommentTopic=addComment.Topic,
                    CommentText = addComment.Text,
                    IsShow = false,
                    User = user,
                    CommentInsertTime = ConverToShamsi.GetDate(DateTime.Now),
                    ReplayId = addComment.ReplayId!=0?addComment.ReplayId : null,
                    Product = product,
                    Parent = parent != null ? parent : null,
                    ProductId=product.Id,
                    UserId=user.Id,
                    Suggest=addComment.CustomerSuggestToBuyThisProduct,
                    ProductBads=bads,
                    ProductGoodNess=goodNes
                };

                await _commentRepository.AddCommentAsync(commentcreate);

                await _commentRepository.SaveAsync();

                returnResult.SuccesMessage = "نظر شما با موفقیت ثبت شد و تا 24 ساعت آینده نظر شما نمایش داده می شود .";
                returnResult.Status = true;
                return returnResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "نظر شما ثبت نشد لطفا دقایقی دیگر دوباره امتحان کنید  !!",
                    Status = false
                };
                return returnResult;
            }
        }

        public async Task<IEnumerable<CommentsViewModel>> GetProductComments(int id)
        {
            var comments = await _commentRepository.GetCommentsAsync();
            comments = comments.Where(p => p.ProductId == id);

            if (comments.Count() == 0 || comments == null)
                return null;

            var returnComment = comments.Select(p => new CommentsViewModel()
            {
                Id = p.CommentId,
                Text = p.CommentText,
                Topic = p.CommentTopic,
                UserName = p.User.UserName
            });

            return returnComment;
        }

        public async Task<ProfileViewModel> GetUserComments(string userId)
        {
            var comments = await _commentRepository.GetCommentsAsync();
            comments = comments.Where(p => p.UserId == userId);

            var user = await _userManager.FindByIdAsync(userId);

            var userCommentsResturn = comments.Select(p => new UserCommentsViewModel()
            {
                CommentId=p.CommentId,
                CommentText=p.CommentText,
                ProductId=p.ProductId,
                ProductImage=p.Product.ProductImages.FirstOrDefault().ImgFile+"/"+p.Product.ProductImages.FirstOrDefault().ImgSrc,
                ProductName=p.Product.Name,
                IsShow=p.IsShow
            });

            var returnProfile = new ProfileViewModel()
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Comments = userCommentsResturn
            };

            return returnProfile;
        }
    }
}
