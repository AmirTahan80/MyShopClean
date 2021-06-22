using Microsoft.AspNetCore.Http;
using Application.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Application.InterFaces.Admin
{
    public interface IProudctServices
    {
        Task<IEnumerable<GetProductsAndImageSrcViewModel>> GetAllProductsAsync();
        Task<GetProductViewModel> GetProductAsync(int productId);
        Task<AddProductViewModel> GetCategoriesTreeViewForAdd();
        Task<bool> DeleteListOfProducts(IEnumerable<GetProductsAndImageSrcViewModel> deleteListOfProducts);

        Task<bool> EditProductAsync(GetProductViewModel editProduct);
        JsonResult UploadFileEditor(IFormFile file);
        Task<bool> DeletePhotoAsync(int imageId);
        
        Task<bool> AddProductAsync(AddProductViewModel addProduct);
        //Task<bool> DeleteProductAsync(int productId);
    }
}
