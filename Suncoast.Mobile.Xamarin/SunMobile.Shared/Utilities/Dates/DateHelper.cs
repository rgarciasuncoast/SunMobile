using System;
using System.Collections.Generic;
using System.Globalization;

namespace SunMobile.Shared.Utilities.Dates
{
    public static class DateHelper
    {
        public static string GetFriendlyDate(this DateTime date)
        {
            string returnValue = string.Format("{0:MM/dd/yyyy}", date);

            // If today, show the time (4:20 PM)
            if (date.Date == DateTime.Today.Date)
            {
                returnValue = string.Format("{0:t}", date);
            }

            if (date.Date == DateTime.Today.Date.AddDays(-1))
            {
                returnValue = "Yesterday";
            }

            // If within the past week, show the day
            if (date.Date > DateTime.Today.AddDays(-7) && date.Date < DateTime.Today.Date.AddDays(-1))
            {
                returnValue = date.DayOfWeek.ToString();
            }

            return returnValue;
        }

        public static string GetFiendlyFullDate(this DateTime date)
        {
            string returnValue = string.Format("{0:dddd, MMMM d, yyyy}", date);

            return returnValue;
        }

        public static string GetFriendlyDateTime(this DateTime date)
        {
            string returnValue = string.Format("{0:MM/dd/yyyy} at {0:t}", date);

            if (date.Date == DateTime.Today.Date)
            {
                returnValue = string.Format("Today at {0:t}", date);
            }

            if (date.Date == DateTime.Today.Date.AddDays(-1))
            {
                returnValue = string.Format("Yesterday at {0:t}", date);
            }

            if (date.Date > DateTime.Today.AddDays(-7) && date.Date < DateTime.Today.Date.AddDays(-1))
            {
                string.Format("{0:dddd} at {0:t}", date);
            }

            return returnValue;
        }

        public static string[] GetMonthsArray()
        {
            string[] months = { "", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(1), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(2), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(3), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(4), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(5), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(6), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(7), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(8), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(9), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(10), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(11), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(12) };

            return months;
        }

        public static bool IsDateMinValOrJsonMinVal(this DateTime date)
        {
            return (date == DateTime.MinValue || date.Date == DateTime.MinValue.Date.ToUniversalTime().Date);
        }

        public static bool IsSqlMinVal(this DateTime date)
        {
            var minDate = DateTime.MinValue.ToSqlMinValue();
            return minDate == date || minDate.Ticks == date.Ticks || minDate.Ticks == date.ToUniversalTime().Ticks || minDate.ToShortDateString() == date.ToShortDateString();
        }

        public static bool IsAnyKnownMinValue(this DateTime date)
        {
            return date.IsSqlMinVal() || date.IsDateMinValOrJsonMinVal();
        }

        public static DateTime ToSqlMinValue(this DateTime value)
        {
            return Convert.ToDateTime("1/1/1753");
        }

        public static DateTime UtcToLocal(this DateTime valueToConvert)
        {
            if (valueToConvert.IsAnyKnownMinValue())
            {
                return DateTime.SpecifyKind(valueToConvert, DateTimeKind.Utc);
            }

            var returnValue = valueToConvert.ToLocalTime();

            return returnValue;
        }

        public static DateTime LocalToUtc(this DateTime valueToConvert)
        {
            if (valueToConvert.IsAnyKnownMinValue())
            {
                return DateTime.SpecifyKind(valueToConvert, DateTimeKind.Utc);
            }

            var returnValue = valueToConvert.ToUniversalTime();

            return returnValue;
        }

        public static DateTime UtcToEastern(this DateTime valueToConvert)
        {
            var returnValue = valueToConvert;

            try
            {
                if (valueToConvert.IsAnyKnownMinValue())
                {
                    return DateTime.SpecifyKind(valueToConvert, DateTimeKind.Utc);
                }

                if (valueToConvert.Kind == DateTimeKind.Utc)
                {
                    var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                    returnValue = TimeZoneInfo.ConvertTimeFromUtc(valueToConvert, easternTimeZone);
                }
            }
            catch (ArgumentException ex)
            {
                Logging.Logging.Log(ex, "DateHelper:UtcToEastern", $"ValueToConvert is {valueToConvert}.");
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "DateHelper:UtcToEastern", $"ValueToConvert is {valueToConvert}.");
            }

            return returnValue;
        }

        public static DateTime LocalToEasternToUtc(this DateTime valueToConvert)
        {
            var returnValue = valueToConvert;

            try
            {
                if (valueToConvert.IsAnyKnownMinValue())
                {
                    return DateTime.SpecifyKind(valueToConvert, DateTimeKind.Utc);
                }

                DateTime eastern;

                if (valueToConvert.Kind == DateTimeKind.Local)
                {
                    var easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                    eastern = TimeZoneInfo.ConvertTime(valueToConvert, TimeZoneInfo.Local, easternTimeZone);
                }
                else
                {
                    eastern = valueToConvert;
                }

                returnValue = eastern.ToUniversalTime();
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "DateHelper:LocalToEasternToUtc", $"ValueToConvert is {valueToConvert}.");
            }

            return returnValue;
        }

#if __IOS__
        public static DateTime OSDateToDateTime(Foundation.NSDate osDate)
        {
            return (DateTime)osDate;
        }

        public static Foundation.NSDate DateTimeToOSDate(DateTime dateTime, bool useLocalTime = false)
        {
            DateTime reference;

            if (useLocalTime)
            {
                reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
            }
            else
            {
                reference = new DateTime(2001, 1, 1, 0, 0, 0);
            }

            return Foundation.NSDate.FromTimeIntervalSinceReferenceDate((dateTime - reference).TotalSeconds);
        }

        public static Foundation.NSMutableArray DateTimeListToOSArray(List<DateTime> dateList)
        {
            var osArray = new Foundation.NSMutableArray();

            foreach (var d in dateList)
            {
                osArray.Add(DateTimeToOSDate(d, true));
            }

            return osArray;
        }
#endif

#if __ANDROID__
        public static DateTime OSDateToDateTime(Java.Util.GregorianCalendar calendar)
        {
            var dateTime = new DateTime(calendar.Get(Java.Util.CalendarField.Year), calendar.Get(Java.Util.CalendarField.Month) + 1, calendar.Get(Java.Util.CalendarField.DayOfMonth));
            return dateTime;
        }

        public static Java.Util.Calendar DateTimeToOSDate(DateTime dateTime)
        {
            var calendar = Java.Util.Calendar.Instance;
            calendar.Set(dateTime.Year, dateTime.Month - 1, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

            return calendar;
        }

        public static Java.Util.Date DateTimeToJavaDate(DateTime dateTime)
        {
            var javaDate = new Java.Util.Date(dateTime.Year, dateTime.Month - 1, dateTime.Day);

            return javaDate;
        }

        public static Android.Runtime.JavaList<Java.Util.Date> DateTimeListToOSArray(List<DateTime> dateList)
        {
            var osArray = new Android.Runtime.JavaList<Java.Util.Date>();

            foreach (var d in dateList)
            {
                osArray.Add(DateTimeToJavaDate(d));
            }

            return osArray;
        }
#endif
    }
}