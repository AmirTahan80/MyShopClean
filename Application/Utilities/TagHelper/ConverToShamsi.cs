using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities.TagHelper
{
    public static class ConverToShamsi
    {
        /// <summary>
        /// Return Year - Month (1400 - 3)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetMonthAndYear(DateTime time)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            var getMonthAndYear = persianCalendar.GetYear(time) + "-" + persianCalendar.GetMonth(time);
            return getMonthAndYear;
        }
        /// <summary>
        /// Return Year , Month , Day , Hour , Minute , Second , MiliSecond
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDate(DateTime time)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            var curentDate = persianCalendar.ToDateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
            return curentDate;
        }
        /// <summary>
        /// Return Year , Month , Day 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDateYeadAndMonthAndDay(DateTime time)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            var curentDate =Convert.ToDateTime(persianCalendar.GetYear(time) + "/" + persianCalendar.GetMonth(time) + "/" + persianCalendar.GetDayOfMonth(time));
            return curentDate;
        }
    }
}
