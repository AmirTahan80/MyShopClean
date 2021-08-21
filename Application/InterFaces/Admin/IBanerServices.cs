using Application.ViewModels;
using Application.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface IBanerServices
    {
        /// <summary>
        /// گرفتن لیستی از بنر ها
        /// </summary>
        /// <returns>IList<BanerViewModel></returns>
        Task<IList<BanerViewModel>> GetBanersAsync();
        /// <summary>
        /// پیدا کردن بنر با آیدی
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BanerViewModel</returns>
        Task<BanerViewModel> GetBanerAsync(int id);
        /// <summary>
        /// افزودن بنر
        /// </summary>
        /// <param name="createBaner"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> AddBanerAsync(BanerViewModel createBaner);
        /// <summary>
        /// ویرایش بنر
        /// </summary>
        /// <param name="editBaner"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> EditBanerAsync(BanerViewModel editBaner);
        /// <summary>
        /// حذف بنر
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> DeleteBanerAsync(int id);

    }
}
