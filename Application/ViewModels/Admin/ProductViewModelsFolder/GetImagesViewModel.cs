using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class GetImagesViewModel
    {
        public string ImgSrc { get; set; }
        public int ImgId { get; set; }
        public bool IsSelected { get; set; }
    }
}
