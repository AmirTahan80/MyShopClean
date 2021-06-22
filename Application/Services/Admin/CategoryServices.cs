using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class CategoryServices : ICategoryServices
    {
        #region Injections
        private readonly ICategoryRepository _categoryRepository;
        public CategoryServices(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        #endregion
        public async Task<IList<GetCategoryViewModel>> GetAllCategoriesAsync()
        {

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoryList = categories.Select(p => new GetCategoryViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                ParentId = p.ParentId,
                HasChild = p.Children.Count > 0 ? true : false,
                Parent = p.Parent != null ? new CategoryParentViewModel()
                {
                    Name = p.Parent.Name,
                    ParentId = p.ParentId,
                } : null
            }).ToList();
            return categoryList;
        }
        public async Task<GetCategoryViewModel> GetCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryAsync(categoryId);
            if (category == null)
                return null;

            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoryReturn = new GetCategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                HasChild = category.Children.Count != 0 ? true : false,
                Categories = CategoriesListTreeView(categoryId, categories),
                Parent = category.Parent != null ? new CategoryParentViewModel()
                {
                    Name = category.Parent.Name,
                    ParentId = category.Parent.Id
                } : null,
                Children = category.Children.Count != 0 ? GetChildrenCategory(category.Children, category.Id) : null,
                ParentId = category.ParentId,
                Detail = category.Detail
            };

            return categoryReturn;
        }
        public async Task<AddCategoryViewModel> GetCategoriesTreeForAdd()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoriesTree = CategoriesListTreeView(null, categories);
            var returnTreeView = new AddCategoryViewModel()
            {
                CategoriesTreeView = categoriesTree,
            };
            return returnTreeView;
        }
        public async Task<bool> AddCategoryAsync(AddCategoryViewModel categoryForAdd)
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                var addCategory = new Category()
                {
                    Name = categoryForAdd.Name,
                    Parent = GetParent(categoryForAdd.ParentId, categories),
                    Detail = categoryForAdd.Detail
                };

                await _categoryRepository.AddCategoryAsync(addCategory);
                await _categoryRepository.SaveAsync();
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }
        public async Task<bool> EditCategoryAsync(GetCategoryViewModel categoryForEdit)
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                var category = await _categoryRepository.GetCategoryAsync(categoryForEdit.Id);

                category.Name = categoryForEdit.Name;
                category.ParentId = categoryForEdit.ParentId!=0? categoryForEdit.ParentId : null;
                category.Parent = categoryForEdit.ParentId != 0 ? GetParent(categoryForEdit.ParentId, categories) : null ;

                _categoryRepository.UpdateCategory(category);
                await _categoryRepository.SaveAsync();

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }
        public async Task<bool> DeleteCategoryAsync(IEnumerable<GetCategoryViewModel> categoriesForDelete)
        {
            try
            {
                var selecCategoriesForDelete = categoriesForDelete.Where(p => p.IsSelected).Select(p => p.Id);

                var deleteCategory = new List<GetCategoryViewModel>();
                foreach (var category in selecCategoriesForDelete)
                {
                    deleteCategory.Add(await GetCategoryAsync(category));
                }

                bool result = false;
                foreach (var category in deleteCategory)
                {
                    result = await DeleteCategoriesAndChild(category);
                    if (!result)
                        break;
                    else
                        continue;
                }

                if (!result)
                    return false;
                else
                    await _categoryRepository.SaveAsync();

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        #region Private
        private Category GetParent(int? ParentId, IEnumerable<Category> categories)
        {
            var parent = categories.SingleOrDefault(p => p.Id == ParentId);
            return parent;
        }
        private List<Category> CategoriesList(IEnumerable<Category> categories = null,
            IEnumerable<Category> childcategories = null)
        {
            List<Category> items = new List<Category>();
            List<Category> subItems = new List<Category>();
            if (categories != null)
            {
                var parentCategoriesModel = categories.Where(p => p.ParentId == null).ToList();
                GetCategoriesParent(parentCategoriesModel);
                return items;
            }
            else if (childcategories != null)
            {
                GetChilderFromParent(childcategories);
                return subItems;
            }
            else
                return null;

            void GetCategoriesParent(IEnumerable<Category> parentCategories)
            {
                foreach (var parent in parentCategories)
                {
                    var category = parentCategories.SingleOrDefault(p => p.Id == parent.Id);
                    if (category.Children.Count > 0)
                    {
                        GetChilderFromParent(category.Children);
                        var item = new Category()
                        {
                            Id = parent.Id,
                            Name = parent.Name,
                            Children = subItems.Select(p => new Category()
                            {
                                Name = p.Name,
                                Id = p.Id,
                                ParentId = p.ParentId,
                                Parent = p.Parent,
                                Children = p.Children,
                            }).ToList()
                        };
                        items.Add(item);
                    }
                    else
                    {
                        var item = new Category()
                        {
                            Id = parent.Id,
                            Name = parent.Name,
                        };
                        items.Add(item);
                    }
                }
                subItems.Clear();
            }

            void GetChilderFromParent(IEnumerable<Category> childrenCategories, int catId = 0)
            {
                foreach (var child in childrenCategories)
                {
                    if (catId == 0)
                    {
                        var subItem = new Category()
                        {
                            Id = child.Id,
                            Name = child.Name,
                            ParentId = child.ParentId,
                            Parent = child.Parent
                        };
                        subItems.Add(subItem);
                    }
                    else
                    {
                        var subItem = subItems.SingleOrDefault(p => p.Id == catId);
                        var subItemChider = new Category()
                        {
                            Id = child.Id,
                            Name = child.Name,
                            ParentId = child.ParentId,
                            Parent = child.Parent
                        };
                        subItem.Children.Add(subItemChider);
                    }
                    var chidCategory = childrenCategories.SingleOrDefault(p => p.Id == child.Id);
                    if (chidCategory.Children.Any())
                    {
                        GetChilderFromParent(chidCategory.Children, chidCategory.Id);
                    }
                }
            }
        }
        private List<SelectListItem> CategoriesListTreeView(int? categoryId, IEnumerable<Category> categories = null,
           IEnumerable<Category> childcategories = null)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> subItems = new List<SelectListItem>();

            var index = new SelectListItem()
            {
                Value = "0",
                Text = "دسته پدر است"
            };
            items.Add(index);

            if (categories != null)
            {
                var parentCategoriesModel = categories.Where(p => p.ParentId == null && p.Id != categoryId)
                    .ToList();
                GetCategoriesParent(parentCategoriesModel);
                return items;
            }
            else if (childcategories != null)
            {
                GetChilderFromParent(childcategories);
                return subItems;
            }
            else
                return null;

            void GetCategoriesParent(IEnumerable<Category> parentCategories)
            {
                int count = 0;
                foreach (var parent in parentCategories)
                {
                    var item = new SelectListItem()
                    {
                        Value = parent.Id.ToString(),
                        Text = parent.Name,
                    };
                    items.Add(item);
                    var category = parentCategories.SingleOrDefault(p => p.Id == parent.Id);
                    if (category.Children.Count > 0)
                    {
                        count++;
                        GetChilderFromParent(category.Children, count);
                    }
                }
            }

            void GetChilderFromParent(IEnumerable<Category> childrenCategories,
                int countNumber = 0)
            {
                foreach (var child in childrenCategories)
                {
                    if (child.Id != categoryId)
                    {
                        var item = new SelectListItem()
                        {
                            Value = child.Id.ToString(),
                            Text = new string('-', countNumber * 2) + child.Name
                        };
                        items.Add(item);
                        var chidCategory = categories.SingleOrDefault(p => p.Id == child.Id);
                        if (chidCategory.Children.Count > 0)
                        {
                            countNumber++;
                            GetChilderFromParent(chidCategory.Children, countNumber);
                        }
                    }
                }
            }
        }
        private ICollection<GetChildrenCategoryViewModel> GetChildrenCategory(IEnumerable<Category> categories, int categoryId)
        {
            var returnChild = categories.Where(p => p.ParentId == categoryId).Select(p => new GetChildrenCategoryViewModel()
            {
                ChildId = p.Id,
                ChildName = p.Name,
                ParentId = categoryId,
                Children = p.Children.Count != 0 ? GetChildrenCategory(p.Children, p.Id) : null
            }).ToList();
            return returnChild;
        }
        private async Task<bool> DeleteCategoriesAndChild(GetCategoryViewModel categoryForDelete)
        {
            if (categoryForDelete == null)
                return false;

            if (categoryForDelete.Children != null)
                await DeleteCategoryChild(categoryForDelete.Children);

            async Task DeleteCategoryChild(IEnumerable<GetChildrenCategoryViewModel> children)
            {
                foreach (var child in children)
                {
                    if (child.Children != null)
                        await DeleteCategoryChild(child.Children);

                    await _categoryRepository.DeleteCategoryByIdAsync(child.ChildId);
                }
            }

            await _categoryRepository.DeleteCategoryByIdAsync(categoryForDelete.Id);

            return true;
        }
        #endregion
    }
}
