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
        /// <summary>
        /// گرفتن تمامی محصولات از دیتابیس و نمایش آنها به صورت لیست
        /// Get All Products From data Base And Show them In List
        /// </summary>
        /// <returns>IEnumerable<GetProductsAndImageSrcViewModel></returns>
        Task<IEnumerable<GetProductsAndImageSrcViewModel>> GetAllProductsAsync();
        /// <summary>
        /// گرفتن یک محصول و یا یافتن آن از طریق ای دی محصول برای نمایش و ویرایش
        /// Get Or Find Product From Id For Show Or Edit
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>GetProductViewModel</returns>
        Task<GetProductViewModel> GetProductAsync(int productId);
        /// <summary>
        /// گرفتن درختی دسته ها برای افزودن محصول
        /// Get Categories Like Tree View For Add Product
        /// </summary>
        /// <returns>AddProductViewModel</returns>
        Task<AddProductViewModel> GetCategoriesTreeViewForAdd();
        /// <summary>
        /// حذف محصول (ها) از داخل لیست و دیتابیس 
        /// </summary>
        /// <param name="deleteListOfProducts"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> DeleteListOfProducts(IEnumerable<GetProductsAndImageSrcViewModel> deleteListOfProducts);
        /// <summary>
        /// ویرایش محصول 
        /// Edit Product
        /// </summary>
        /// <param name="editProduct"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> EditProductAsync(GetProductViewModel editProduct);
        /// <summary>
        /// آپلود عکس ها از CkEditr 5 داخل پوشه و نمایش آن
        /// Upload File From CkEdit5 to Folders And Show Them
        /// </summary>
        /// <param name="file"></param>
        /// <returns>JsonResult</returns>
        JsonResult UploadFileEditor(IFormFile file);
        /// <summary>
        /// حذف عکس محصول به صورت تکی از دیتابیس
        /// Delete a photo from database
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> DeletePhotoAsync(int imageId);
        /// <summary>
        /// افزودن محصول
        /// Add Product
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns>bool : True or False</returns>
        Task<bool> AddProductAsync(AddProductViewModel addProduct);
    }
}
