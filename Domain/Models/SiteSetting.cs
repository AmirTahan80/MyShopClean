using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string SiteName { get; set; }

        [MaxLength(250)]
        public string SiteTagline { get; set; }

        [Required, MaxLength(7)]
        public string PrimaryColor { get; set; }

        [Required, MaxLength(7)]
        public string SecondaryColor { get; set; }

        [Required, MaxLength(7)]
        public string AccentColor { get; set; }

        [MaxLength(250)]
        public string FooterTitle { get; set; }

        [MaxLength(2000)]
        public string FooterDescription { get; set; } = "توسعه داده شده توسط AmirTahan.";

        [MaxLength(500)]
        public string FooterCopyright { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [EmailAddress, MaxLength(250)]
        public string SupportEmail { get; set; }

        [MaxLength(500)]
        public string PublicBaseUrl { get; set; }

        public bool TorobEnabled { get; set; }

        [MaxLength(200)]
        public string TorobAccessToken { get; set; }

        public bool EmallsEnabled { get; set; }

        [MaxLength(200)]
        public string EmallsAccessToken { get; set; }

        [MaxLength(1000)]
        public string LogoUrl { get; set; }

        [MaxLength(250)]
        public string Slogan { get; set; }
    }
}
