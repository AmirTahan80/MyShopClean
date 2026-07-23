using Application.InterFaces.Admin;
using Application.ViewModels.Admin;
using Domain.InterFaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class SiteSettingService : ISiteSettingService
    {
        private const string CacheKey = "site-settings";
        private readonly ISiteSettingRepository _repository;
        private readonly IMemoryCache _cache;

        public SiteSettingService(ISiteSettingRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<SiteSettingViewModel> GetAsync()
        {
            if (_cache.TryGetValue(CacheKey, out SiteSettingViewModel cached))
            {
                return cached;
            }

            var setting = await _repository.GetAsync() ?? CreateDefault();
            var model = Map(setting);
            _cache.Set(CacheKey, model, TimeSpan.FromMinutes(10));
            return model;
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
            setting.TorobAccessToken = model.TorobAccessToken?.Trim();
            setting.EmallsEnabled = model.EmallsEnabled;
            setting.EmallsAccessToken = model.EmallsAccessToken?.Trim();

            await _repository.SaveAsync(setting);
            _cache.Remove(CacheKey);
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
            TorobAccessToken = setting.TorobAccessToken,
            EmallsEnabled = setting.EmallsEnabled,
            EmallsAccessToken = setting.EmallsAccessToken
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
}
