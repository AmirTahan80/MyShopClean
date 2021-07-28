using Application.InterFaces.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.Components
{
    public class CategoryComponent:ViewComponent
    {
        #region Injections
        private readonly ICategoryUserServices _categoryUserServices;
        public CategoryComponent(ICategoryUserServices categoryUserServices)
        {
            _categoryUserServices = categoryUserServices;
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryUserServices.GetCategoriesTreeViewAsync();
            return View("/Views/Components/CategoryComponent.cshtml", categories);
        }

    }
}
