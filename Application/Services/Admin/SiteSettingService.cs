using Application.InterFaces.Admin;
using Application.Utilities;
using Application.Utilities.TagHelper;
using Application.ViewModels.Admin;
using Domain.InterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly ISiteSettingRepository _repository;
        private SiteSettingViewModel _requestCache;

        public SiteSettingService(ISiteSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<SiteSettingViewModel> GetAsync()
        {
            if (_requestCache != null)
            {
                return _requestCache;
            }

            var setting = await _repository.GetAsync() ?? CreateDefault();
            _requestCache = Map(setting);
            return _requestCache;
        }

        public async Task UpdateAsync(SiteSettingViewModel model)
        {
            var setting = await _repository.GetAsync() ?? new SiteSetting();
            setting.SiteName = model.SiteName.Trim();
            setting.SiteTagline = model.SiteTagline?.Trim();
            setting.PrimaryColor = model.PrimaryColor.ToUpperInvariant();
            setting.SecondaryColor = model.SecondaryColor.ToUpperInvariant();
            setting.AccentColor = model.AccentColor.ToUpperInvariant();
            setting.FooterTitle = model.FooterTitle?.Trim();
            setting.FooterDescription = model.FooterDescription?.Trim();
            setting.FooterCopyright = model.FooterCopyright?.Trim();
            setting.Address = model.Address?.Trim();
            setting.Phone = model.Phone?.Trim();
            setting.SupportEmail = model.SupportEmail?.Trim();
            setting.PublicBaseUrl = model.PublicBaseUrl?.Trim().TrimEnd('/');
            setting.TorobEnabled = model.TorobEnabled;

            if (model.LogoUrl != setting.LogoUrl)
            {
                DeletePhoto(setting.LogoUrl);
                var logoUrl = uploadImage(model.Logo);

                setting.LogoUrl = model.LogoUrl;
            }

            if (!string.IsNullOrWhiteSpace(model.TorobAccessToken))
            {
                setting.TorobAccessToken = AccessTokenHasher.Hash(model.TorobAccessToken);
            }
            setting.EmallsEnabled = model.EmallsEnabled;
            if (!string.IsNullOrWhiteSpace(model.EmallsAccessToken))
            {
                setting.EmallsAccessToken = AccessTokenHasher.Hash(model.EmallsAccessToken);
            }

            await _repository.SaveAsync(setting);
            _requestCache = null;
        }

        public async Task<bool> IsTorobTokenValidAsync(string token)
        {
            var setting = await _repository.GetAsync();
            return setting != null && AccessTokenHasher.Verify(setting.TorobAccessToken, token);
        }

        public async Task<bool> IsEmallsTokenValidAsync(string token)
        {
            var setting = await _repository.GetAsync();
            return setting != null && AccessTokenHasher.Verify(setting.EmallsAccessToken, token);
        }

        private static SiteSettingViewModel Map(SiteSetting setting) => new SiteSettingViewModel
        {
            Id = setting.Id,
            SiteName = setting.SiteName,
            SiteTagline = setting.SiteTagline,
            PrimaryColor = setting.PrimaryColor,
            SecondaryColor = setting.SecondaryColor,
            AccentColor = setting.AccentColor,
            FooterTitle = setting.FooterTitle,
            FooterDescription = setting.FooterDescription,
            FooterCopyright = setting.FooterCopyright,
            Address = setting.Address,
            Phone = setting.Phone,
            SupportEmail = setting.SupportEmail,
            PublicBaseUrl = setting.PublicBaseUrl,
            TorobEnabled = setting.TorobEnabled,
            TorobAccessToken = null,
            EmallsEnabled = setting.EmallsEnabled,
            EmallsAccessToken = null
        };

        private static SiteSetting CreateDefault() => new SiteSetting
        {
            SiteName = "MyShop",
            SiteTagline = "فروشگاه اینترنتی",
            PrimaryColor = "#EF394E",
            SecondaryColor = "#0EA5E9",
            AccentColor = "#F59E0B",
            FooterTitle = "فروشگاه اینترنتی MyShop",
            FooterDescription = "خرید آنلاین ساده، امن و سریع.",
            FooterCopyright = "تمامی حقوق محفوظ است.",
            PublicBaseUrl = "http://localhost:8080"
        };
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
            return SecureImageUpload.TrySave(file, uploadsRootFolder, out var fileName, out _)
                ? todayDate + "/" + fileName
                : string.Empty;
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
