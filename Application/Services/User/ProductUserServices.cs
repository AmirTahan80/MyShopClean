﻿using Application.InterFaces.User;
using Application.ViewModels.User;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class ProductUserServices : IProductUserServices
    {
        #region Injections
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductUserServices(IProductRepository productRepository, UserManager<ApplicationUser> userManager, ICartRepository cartRepository,
            ICommentRepository commentRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _commentRepository = commentRepository;
            _categoryRepository = categoryRepository;
        }
        #endregion
        //Implements

        public async Task<ProductIndexViewModel> GetProductsListAsync(ICollection<int> categoriesId)
        {
            var allProducts = await _productRepository.GetAllProductsAsync();
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            var products = new List<Product>();
            if (categoriesId.Count() > 0)
            {
                if (!categoriesId.Any(p => p == 0))
                {
                    products = allProducts.Where(p => categoriesId.Any(t => p.Categories.Any(e=>e.CategoryId ==t))).ToList();
                }
                else
                {
                    categoriesId.Clear();
                    categoriesId.Add(0);
                    products = allProducts.ToList();
                }
            }
            else
            {
                categoriesId.Clear();
                categoriesId.Add(0);
                products = allProducts.ToList();
            }

            var returnCorrentProduct = products.Select(p => new GetListOfProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Count = p.Count,
                ImageSrc = (p.ProductImages.FirstOrDefault().ImgFile == ""?"": p.ProductImages.FirstOrDefault().ImgFile + "/") + p.ProductImages.FirstOrDefault().ImgSrc,
                InstagramPost= p.ProductImages.FirstOrDefault().ImgFile == "" ? true:false,
                Price = p.Price
            });
            var retrunCategoriesTreeView = GetCategoriesTreeView(categories, allProducts);

            var retrunProductsAndCategories = new ProductIndexViewModel()
            {
                Products = returnCorrentProduct,
                CategoriesTreeView = retrunCategoriesTreeView,
                CategoriesId = categoriesId
            };

            return retrunProductsAndCategories;
        }

        public async Task<GetProductDescriptionViewModel> GetProductDescriptionAsync(int productId, string userId = "")
        {
            var product = await _productRepository.GetProductAsync(productId);
            if (product == null)
                return null;

            var questionsTreeView = new List<QuestionViewModel>();

            if (product.Questions != null)
            {
                var parentQuestion = product.Questions.Where(p => p.ReplayOn == null).OrderByDescending(p => p.Id).ToList();

                foreach (var parent in parentQuestion)
                {
                    var item = new QuestionViewModel()
                    {
                        Email = parent.User.UserName,
                        Id = parent.Id,
                        Text = parent.QuestionText,
                        ReplayId = 0,
                        Counter = 0
                    };

                    questionsTreeView.Add(item);

                    if (parent.Replais != null)
                        GetQuesTionsReplay(parent, 1);

                }

                void GetQuesTionsReplay(Question question, int counter)
                {

                    foreach (var replay in question.Replais)
                    {

                        var item = new QuestionViewModel()
                        {
                            Id = replay.Id,
                            Email = replay.User.UserName,
                            ReplayId = replay.ReplayOn.Id,
                            Text = replay.QuestionText,
                            ProductId = replay.Product.Id,
                            Counter = counter * 2
                        };

                        questionsTreeView.Add(item);

                        if (replay.Replais != null)
                        {
                            counter++;
                            GetQuesTionsReplay(replay, counter);
                            counter--;
                        }

                    }

                }
            }

            var productReturn = new GetProductDescriptionViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Count = product.Count,
                Price = product.Price.ToString("#,0"),
                Images = product.ProductImages.Select(p => new ProductImageViewModel()
                {
                    ImgSrc = (p.ImgFile ==""? "" : p.ImgFile + "/")+ p.ImgSrc,
                }),
                CategoryId = product.Categories.Select(p => p.CategoryId).LastOrDefault(),
                CategoryName = product.Categories.Select(p => p.Category.Name).LastOrDefault(),
                InstagramPost = product.ProductImages.FirstOrDefault().ImgFile == "" ? true : false,
                Description = product.Detail,
                IsProductHaveAttributes = product.IsProductHaveAttributes,
                Properties = product.Properties != null ? product.Properties.Select(p => new PropertiesViewModel()
                {
                    PropertyName = p.ValueName,
                    PropertyValue = p.ValueType
                }):null,
                Attributes = product.IsProductHaveAttributes == true ? product.ProductAttributes.OrderBy(p=>p.AttributeId)
                .Select(p => new ProductAttributeViewModel()
                {
                    AttributesName = p.AttributeName,
                    AttributesValue = p.AttributeValues.Select(p => new SelectListItem()
                    {
                        Value = p.ValueId.ToString(),
                        Text = p.ValueName
                    })
                }).ToList() : null,
                Templates = product.AttributeTemplates
                .Select(p => new ProductAttributesTemplate()
                {
                    Id = p.AttributeTemplateId,
                    Template = p.Template,
                    Count = p.AttrinbuteTemplateCount,
                    Price = p.AttrinbuteTemplatePrice
                }),
                Comments = product.Comments.Where(p => p.IsShow)
                .Select(p => new ProductCommentViewModel()
                {
                    Id = p.CommentId,
                    Text = p.CommentText,
                    Topic = p.CommentTopic,
                    UserName = p.User.UserName,
                    CustomerSuggestToBuy = p.Suggest,
                    InsertTime = p.CommentInsertTime,
                    Bads = p.ProductBads != null ? p.ProductBads.Split(",") : null,
                    GoodNess = p.ProductGoodNess != null ? p.ProductGoodNess.Split(",") : null
                }).OrderByDescending(p => p.Id),
                Questions = questionsTreeView
            };

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var favorite = await _cartRepository.GetFavoriteAsync(user.Id);
                    if (favorite != null)
                    {
                        var favoriteDetail = favorite.UserFavoritesDetails.SingleOrDefault(p => p.ProductId == product.Id);
                        if (favoriteDetail != null)
                        {
                            productReturn.IsInUserFavorite = true;
                        }
                        else
                        {
                            productReturn.IsInUserFavorite = false;
                        }
                    }
                }
            }

            return productReturn;
        }


        #region PrivateMethodes
        private IEnumerable<GetCategoriesTreeViewViewModel> GetCategoriesTreeView(IEnumerable<Category> categories,IEnumerable<Product> products)
        {
            var parents = categories.Where(p => p.Parent == null).ToList();


            var categoriesTreeView = new List<GetCategoriesTreeViewViewModel>();

            if (parents != null)
            {
                foreach (var parent in parents)
                {
                    var productCount = products.Where(p => p.Categories.Any(t=>t.CategoryId == parent.Id)).Count();
                    var item = new GetCategoriesTreeViewViewModel()
                    {
                        Id = parent.Id,
                        Name = parent.Name,
                        Count = 0,
                        ProductCount= productCount
                    };

                    categoriesTreeView.Add(item);

                    if (parent.Children != null)
                        GetCategoriesChild(parent, 2);

                }

                void GetCategoriesChild(Category category, int counter)
                {

                    foreach (var replay in category.Children)
                    {
                        var productCount = products.Where(p => p.Categories.Any(t => t.CategoryId == replay.Id)).Count();

                        var item = new GetCategoriesTreeViewViewModel()
                        {
                            Id = replay.Id,
                            Name = replay.Name,
                            Count = counter,
                            ProductCount= productCount
                        };

                        categoriesTreeView.Add(item);

                        if (replay.Children != null)
                        {
                            counter++;
                            GetCategoriesChild(replay, counter);
                            counter--;
                        }

                    }

                }
            }

            return categoriesTreeView;
        }

        #endregion
    }
}
