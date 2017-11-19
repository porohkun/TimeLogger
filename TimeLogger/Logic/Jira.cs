using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLogger
{
    public static class Jira
    {
        public static double HoursInDay = 8;
        private const string _format = "{0} {1}:{2}";
        public static string ToJira(this TimeSpan source)
        {
            double hours = Math.Floor(source.TotalHours);
            double days = Math.Floor(hours / HoursInDay);
            hours = hours - days * HoursInDay;
            return string.Format(_format, TwoDigit(days), TwoDigit(hours), TwoDigit(source.Minutes));
        }

        private static string TwoDigit(double value)
        {
            return (value < 10 ? "0" : "") + value;
        }

        public static DateTime ParseDate(string text)
        {
            int year = int.Parse(text.Substring(0, 4));
            int month = int.Parse(text.Substring(5, 2));
            int day = int.Parse(text.Substring(8, 2));
            int hour = int.Parse(text.Substring(11, 2));
            int minute = int.Parse(text.Substring(14, 2));
            int second = int.Parse(text.Substring(17, 2));
            double belt = double.Parse(text.Substring(24, 4)) * (text[23] == '+' ? 1 : -1)/100;
            
            return new DateTime(year, month, day, hour, minute, second).AddHours(+2 - belt);
        }
    }
}
