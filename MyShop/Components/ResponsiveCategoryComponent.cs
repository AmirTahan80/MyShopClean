using Application.InterFaces.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyShop.Components
{
    public class ResponsiveCategoryComponent : ViewComponent
    {
        #region Injections
        private readonly ICategoryUserServices _categoryUserServices;
        public ResponsiveCategoryComponent(ICategoryUserServices categoryUserServices)
        {
            _categoryUserServices = categoryUserServices;
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryUserServices.GetResponsiveCategoriesAsync();
            return View("/Views/Components/ResponsiveCategoryComponent.cshtml", categories);
        }
    }
}
