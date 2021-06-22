using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageSizeAttribute : ValidationAttribute
    {
        public ImageSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }
        private readonly int _maxFileSize;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as IEnumerable<IFormFile>;
            int success = 0;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        if (file.Length > _maxFileSize)
                        {
                            success = 1;
                        }
                        else
                            success = 0;
                    }
                    else
                        success = 2;
                }

                if (success == 0)
                {
                    return ValidationResult.Success;
                }
                else if (success == 1)
                    return new ValidationResult("سایز فایل بزرگ تر از حد تایین شده است" + "سایز فایل باید" + _maxFileSize + "باشد!");
                else if (success == 2)
                    return new ValidationResult("عکسی آپلود نشده حداقل باید یک عکس آپلود کنید!");
                else
                    return new ValidationResult("خطایی غیره منتظره پیش آمده است !");
            }
            else
            {
                return new ValidationResult("برای افزودن محصول حداقل یک عکس مورد نیاز است !!!");
            }
        }
    }
}
