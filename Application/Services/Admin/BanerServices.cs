using Application.InterFaces.Admin;
using Application.Utilities.TagHelper;
using Application.ViewModels;
using Application.ViewModels.Admin;
using Domain.InterFaces;
using Domain.Models.IndexFolder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class BanerServices : IBanerServices
    {
        #region Injections
        private readonly IBanerRepository _banerRepository;
        public BanerServices(IBanerRepository banerRepository)
        {
            _banerRepository = banerRepository;
        }
        #endregion
        public async Task<IList<BanerViewModel>> GetBanersAsync()
        {
            var baners = await _banerRepository.GetBanersAsync();
            var banersOrder = baners.OrderByDescending(p => p.Id);

            var returnBaners = banersOrder.Select(p => new BanerViewModel()
            {
                Id = p.Id,
                Text = p.Text,
                ImagePath = p.Image,
                Link = p.Link,
                ImageLocation = p.BanerPlace
            }).ToList();

            return returnBaners;
        }

        public async Task<BanerViewModel> GetBanerAsync(int id)
        {
            var baners = await _banerRepository.GetBanersAsync();
            var banerFind = baners.SingleOrDefault(p => p.Id == id);

            var returnBaner = new BanerViewModel()
            {
                Id = banerFind.Id,
                ImagePath = banerFind.Image,
                Link = banerFind.Link,
                Text = banerFind.Text
            };

            return returnBaner;
        }

        public async Task<ResultDto> AddBanerAsync(BanerViewModel createBaner)
        {
            try
            {
                var returnResut = new ResultDto();

                var imageUploadPath = uploadImage(createBaner.Image);
                if (string.IsNullOrWhiteSpace(imageUploadPath))
                {
                    returnResut.ErrorMessage = "در آپلود عکس شکلی پیش آمده است !!! لطفا دوباره تلاش کنید...";
                    returnResut.Status = false;

                    return returnResut;
                }

                var createBanerModel = new Baner()
                {
                    Link = createBaner.Link,
                    Text = createBaner.Text,
                    Image = imageUploadPath,
                    BanerPlace = createBaner.ImageLocation
                };

                await _banerRepository.AddBanerAsync(createBanerModel);

                await _banerRepository.SaveAsync();

                returnResut.SuccesMessage = "بنر با موفقیت آپلود و ذخیره شد ...";
                returnResut.Status = true;

                return returnResut;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResut = new ResultDto()
                {
                    ErrorMessage = "بنر با موفیت افزوده نشد لطفا دقایقی دیگر دوباره امتحان کنید !!!",
                    Status = false
                };
                return returnResut;
            }
        }

        public async Task<ResultDto> EditBanerAsync(BanerViewModel editBaner)
        {
            try
            {
                var retrunResult = new ResultDto();

                var baners = await _banerRepository.GetBanersAsync();
                var banerFind = baners.SingleOrDefault(p => p.Id == editBaner.Id);

                banerFind.Text = editBaner.Text;
                banerFind.Link = editBaner.Link;
                banerFind.BanerPlace = editBaner.ImageLocation;

                if (editBaner.Image != null)
                {
                    var result = DeletePhoto(banerFind.Image);
                    if (!result)
                    {
                        retrunResult.ErrorMessage = "در ویرایش بنر مشکلی به وجود آمده است ... لطفا دوباره تلاش کنید !!!";
                        retrunResult.Status = false;
                        return retrunResult;
                    }
                    var imagePath = uploadImage(editBaner.Image);

                    banerFind.Image = imagePath;
                }

                await _banerRepository.SaveAsync();

                retrunResult.SuccesMessage = "ویرایش بنر با موفقیت انجام شد .";
                retrunResult.Status = true;

                return retrunResult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var retrunResult = new ResultDto()
                {
                    ErrorMessage = "در ویرایش بنر مشکلی به وجود آمده است ... لطفا دوباره تلاش کنید !!!",
                    Status = false
                };
                return retrunResult;
            }
        }

        public async Task<ResultDto> DeleteBanersAsync(IList<BanerViewModel> deleteBaners)
        {
            try
            {

                var returnResult = new ResultDto();

                var banersIdForDelete = deleteBaners.Where(p => p.IsSelected).Select(p => p.Id);
                var allBaners = await _banerRepository.GetBanersAsync();
                var baners = new List<Baner>();

                foreach (var banerId in banersIdForDelete)
                {
                    baners.Add(allBaners.Where(p => p.Id == banerId).SingleOrDefault());
                }

                foreach (var baner in baners)
                {
                    var result = DeletePhoto(baner.Image);
                    if (!result)
                    {
                        returnResult.ErrorMessage = "در حذف بنر از فایل مشکلی به وجود آمد لطفا دوباره امتحان کنید !!!";
                        returnResult.Status = false;

                        return returnResult;
                    }

                }   

                _banerRepository.DeleteBaners(baners);

                await _banerRepository.SaveAsync();

                returnResult.SuccesMessage = "بنر ها با موفقیت حذف شد ...";
                returnResult.Status = true;

                return returnResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "در حذف بنر مشکلی پیش آمده است !!! دقایقی دیگر دوباره امتحان کنید !!!",
                    Status = false
                };
                return returnResult;
            }
        }


        #region PrivateMethode TagHelper

        private string uploadImage(IFormFile file)
        {
            if (file == null)
                return ("");

            var todayDate = ConverToShamsi.GetMonthAndYear(DateTime.Now);
            string folder = $@"wwwroot\Images\Baners\{todayDate}";
            var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }
            if (file.Length != 0)
            {

                string fileName = DateTime.Now.Ticks.ToString() + "-" + file.FileName;
                string filePath = Path.Combine(uploadsRootFolder, fileName);
                using (var FileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(FileStream);
                }
                return (todayDate + "/" + fileName);
            }
            else
                return ("");
        }

        private bool DeletePhoto(string imagePath)
        {
            if (imagePath == "")
                return false;

            string folder = $@"wwwroot\Images\Baners\{imagePath}";
            var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (File.Exists(uploadsRootFolder))
            {
                File.Delete(uploadsRootFolder);
                return true;
            }
            else
                return false;
        }


        #endregion
    }
}
