using Application.InterFaces.User;
using Application.ViewModels.User;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class CategoryUserServices : ICategoryUserServices
    {
        #region Injection
        private readonly ICategoryRepository _categoryRepository;
        public CategoryUserServices(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        #endregion

        public async Task<IEnumerable<GetCategoriesTreeViewViewModel>> GetCategoriesTreeViewAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var parentCategories = categories.Where(p => p.Parent == null);
            var categoriesRetuen = parentCategories.Select(p => new GetCategoriesTreeViewViewModel()
            {
                Id=p.Id,
                Name=p.Name,
                IsCategoryHasChild=p.Children.Count>0?true:false,
                Children=p.Children==null?null: GetChildrenCategory(categories,p.Id)
            });

            return categoriesRetuen;
        }


        #region Private Methode
        private List<Category> CategoriesList(IEnumerable<Category> categories)
        {
            List<Category> Item = new List<Category>();
            List<Category> SubItem = new List<Category>();
            List<Category> SubChildItem = new List<Category>();

            var ParentCategories = categories.Where(c => c.ParentId == null).ToList();

            int count = 0;

            ParentCategoryScroller(ParentCategories);

            void ParentCategoryScroller(IEnumerable<Category> Categories)
            {
                foreach (var Parent in Categories)
                {
                    Item.Add(new Category()
                    {
                        Id = Parent.Id,
                        Name = Parent.Name
                    });

                    var category = Categories.SingleOrDefault(p => p.Id == Parent.Id);
                    if (category.Children.Any())
                    {
                        ChildrenCategoryScroller(category.Children);
                        var singleItem = Item.SingleOrDefault(p => p.Id == category.Id);
                        foreach (var child in SubItem)
                        {
                            singleItem.Children.Add(new Category()
                            {
                                Id = child.Id,
                                Name = child.Name,
                                Children = child.Children
                            });
                        }
                        SubItem.Clear();
                    }
                    count = 0;
                }
            }
            void ChildrenCategoryScroller(IEnumerable<Category> Children)
            {
                foreach (var Child in Children)
                {
                    SubItem.Add(new Category()
                    {
                        Id = Child.Id,
                        Name = Child.Name
                    });

                    var category = categories.SingleOrDefault(p => p.Id == Child.Id);
                    if (category.Children.Any())
                    {
                        SubChildrenCategoryScroller(category.Children, count);
                        var singleItem = SubItem.SingleOrDefault(p => p.Id == category.Id);
                        foreach (var child in SubChildItem)
                        {
                            singleItem.Children.Add(new Category()
                            {
                                Id = child.Id,
                                Name = child.Name,
                                Children = child.Children
                            });
                        }
                        SubChildItem.Clear();
                    }
                }
            }
            void SubChildrenCategoryScroller(IEnumerable<Category> Children, int counter)
            {
                foreach (var Child in Children)
                {

                    SubChildItem.Add(new Category()
                    {
                        Id = Child.Id,
                        Name = new string('^', counter * 2) + $"{Child.Name}"
                    });
                    var category = categories.SingleOrDefault(p => p.Id == Child.Id);
                    if (category.Children.Any())
                    {
                        ++counter;
                        SubChildrenCategoryScroller(category.Children, counter);
                        --counter;
                    }
                }
            }

            return Item;
        }
       
        private ICollection<GetCategoriesTreeViewViewModel> GetChildrenCategory(IEnumerable<Category> categories, int categoryId)
        {
            var returnChild = categories.Where(p => p.ParentId == categoryId).Select(p => new GetCategoriesTreeViewViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                ParentId = categoryId,
                IsCategoryHasChild=p.Children.Count>0?true:false,
                Children = p.Children.Count != 0 ? GetChildrenCategory(p.Children, p.Id) : null
            }).ToList();
            return returnChild;
        }

        private List<GetCategoriesTreeViewViewModel> GetCategoriesLikeTreeView(IEnumerable<Category> categories)
        {
            var parents = categories.Where(p => p.Parent == null);
            if (parents != null)
            {
                var categoriesReturn = new List<GetCategoriesTreeViewViewModel>();
                categoriesReturn=parents.Select(p=>new GetCategoriesTreeViewViewModel()
                {
                    Id=p.Id,
                    Name=p.Name,
                    Children=p.Children==null?null: GetChildrent(p.Children)
                }).ToList();

                return categoriesReturn;
            }
            else
            {
                return null;
            }

            List<GetCategoriesTreeViewViewModel> GetChildrent(IEnumerable<Category> children)
            {
                if (children != null)
                {
                    var categoriesChildren = new List<GetCategoriesTreeViewViewModel>();
                    categoriesChildren = children.Select(p => new GetCategoriesTreeViewViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ParentId = p.ParentId,
                        Parent = new GetParentCategoryViewModel()
                        {
                            Id=p.Parent.Id,
                            Name=p.Name
                        },
                        Children = p.Children == null ? null : GetChildrent(p.Children)
                    }).ToList();

                    return categoriesChildren;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
    }
}
