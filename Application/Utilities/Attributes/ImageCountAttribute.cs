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
    public class ImageCountAttribute: ValidationAttribute
    {
        int _maxCount;
        public ImageCountAttribute(int maxCount=5)
        {
            _maxCount = maxCount;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value!=null)
            {
                var files = value as IEnumerable<IFormFile>;
                if (files.Count() > _maxCount)
                {
                    return new ValidationResult($"تعداد عکس ها از حد ممکن بیشتر است ! نهایت تعداد عکسی که میتوانید آپلود کنید {_maxCount} است");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            return ValidationResult.Success;
        }
    }
}
