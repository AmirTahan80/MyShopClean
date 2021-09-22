using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities.TagHelper
{
    public static class  ConverStringDateToDateTime
    {
        public static DateTime ParsePersianDate(this string date)
        {
            var dateParts = date.Split('/');
            return new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), new PersianCalendar());
        }
    }
}
