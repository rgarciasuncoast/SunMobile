using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.BillPay;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Dates;

#if __IOS__
using Syncfusion.SfCalendar.iOS;
using UIKit;
#endif

#if __ANDROID__
using Com.Syncfusion.Calendar;
using Android.App;
#endif

namespace SunMobile.Shared.Utilities.SyncFusion
{
    public class SyncFusionHelper
    {
#if __IOS__
        public Task<SFCalendar> SycFusionCalendarCreate(UIView view, DateTime selectDate, DateTime startDate, DateTime endDate, List<DateTime> validDateList)
#endif
#if __ANDROID__
        public Task<SfCalendar> SycFusionCalendarCreate(Activity view, Android.Widget.LinearLayout layout, DateTime selectDate, DateTime startDate, DateTime endDate, List<DateTime> validDateList)
#endif
        {
#if __IOS__
            var tcs = new TaskCompletionSource<SFCalendar>();
            var calendar = new SFCalendar();
#endif

#if __ANDROID__
            var tcs = new TaskCompletionSource<SfCalendar>();
            var calendar = new SfCalendar(view);
#endif

            try
            {
                if (selectDate == DateTime.MinValue)
                {
                    if (startDate == DateTime.MinValue)
                    {
                        calendar.SelectedDate = DateHelper.DateTimeToOSDate(DateTime.Today.Date);
                    }
                    else
                    {
                        calendar.SelectedDate = DateHelper.DateTimeToOSDate(startDate);
                    }
                }
                else
                {
                    calendar.SelectedDate = DateHelper.DateTimeToOSDate(selectDate);
                }

                if (startDate == DateTime.MinValue)
                {
                    if (validDateList != null && validDateList.Count > 0)
                    {
                        startDate = validDateList[0];
                    }
                    else
                    {
                        startDate = DateTime.Today.AddMonths(-6);
                    }
                }

                if (endDate == DateTime.MinValue)
                {
                    if (validDateList != null && validDateList.Count > 0)
                    {
                        endDate = validDateList[validDateList.Count - 1];
                    }
                    else
                    {
                        endDate = DateTime.Today.AddYears(1);
                    }
                }

                calendar.NavigateToMonthOnInActiveDatesSelection = false;
                calendar.MinDate = DateHelper.DateTimeToOSDate(startDate);
                calendar.MaxDate = DateHelper.DateTimeToOSDate(endDate);
                calendar.ShowYearView = true;

#if __IOS__
                calendar.HeaderHeight = 40;
                calendar.DisplayDate = DateHelper.DateTimeToOSDate(DateTime.Today);
                calendar.FirstDayofWeek = 0;
                calendar.EnableInLine = false;

                var customLabelList = new Foundation.NSMutableArray<Foundation.NSString>();
                customLabelList.Add((Foundation.NSString)"Sun");
                customLabelList.Add((Foundation.NSString)"Mon");
                customLabelList.Add((Foundation.NSString)"Tue");
                customLabelList.Add((Foundation.NSString)"Wed");
                customLabelList.Add((Foundation.NSString)"Thu");
                customLabelList.Add((Foundation.NSString)"Fri");
                customLabelList.Add((Foundation.NSString)"Sat");
                calendar.CustomDayLabels = customLabelList;
                calendar.ViewMode = SFCalendarViewMode.SFCalendarViewModeMonth;

                var monthSettings = new SFMonthViewSettings
                {
                    HeaderTextColor = UIColor.White,
                    HeaderBackgroundColor = UIColor.FromRGB(61, 61, 61),
                    DayLabelTextColor = UIColor.White,
                    DayLabelBackgroundColor = UIColor.FromRGB(61, 61, 61),
                    TodayTextColor = UIColor.Red,
                    WeekEndTextColor = UIColor.FromRGB(9, 144, 233),
                    DateSelectionColor = UIColor.Green,
                    CurrentMonthTextColor = UIColor.Black,
                    CurrentMonthBackgroundColor = UIColor.White,
                    PreviousMonthTextColor = UIColor.Black,
                    PreviousMonthBackgroundColor = UIColor.FromRGB(255, 255, 240),
                    DisabledTextColor = UIColor.Gray,
                    DisabledBackgroundColor = UIColor.LightGray
                };

                calendar.MonthViewSettings = monthSettings;

                calendar.Frame = new CoreGraphics.CGRect(0, 0,                                           
                                                         (iOS.AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Width - 
                                                         (iOS.AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Width < iOS.AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Height ? 0 : iOS.AppDelegate.MenuNavigationController.View.Frame.Width)),
                                                         iOS.AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Height);

                view.AddSubview(calendar);
#endif

#if __ANDROID__
                calendar.HeaderHeight = 95;
                calendar.HeaderTextSize = 13;
                calendar.HeaderFontAttributes = Android.Graphics.Typeface.DefaultFromStyle(Android.Graphics.TypefaceStyle.Normal);
                calendar.HeaderTextColor = Android.Graphics.Color.White;
                calendar.HeaderBackgroundColor = Android.Graphics.Color.ParseColor("#3D3D3D");
                calendar.FirstDayOfWeek = 1;
                calendar.ViewMode = Com.Syncfusion.Calendar.Enums.ViewMode.MonthView;

                var monthViewSettings = new MonthViewSettings
                {
                    TodayTextColor = Android.Graphics.Color.Red,
                    WeekEndTextColor = Android.Graphics.Color.ParseColor("#0990E9"),
                    DateSelectionColor = Android.Graphics.Color.Green,
                    CurrentMonthTextColor = Android.Graphics.Color.Black,
                    CurrentMonthBackgroundColor = Android.Graphics.Color.White,
                    PreviousMonthTextColor = Android.Graphics.Color.Black,
                    PreviousMonthBackgroundColor = Android.Graphics.Color.ParseColor("#fffff0"),
                    DisabledTextColor = Android.Graphics.Color.Gray,
                    DisabledBackgroundColor = Android.Graphics.Color.LightGray,
                };

                calendar.MonthViewSettings = monthViewSettings;
                calendar.UpdateCalendar();

                layout.AddView(calendar);
#endif

                tcs.SetResult(calendar);

            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "SyncFusionHelper:SycFusionCalendarCreate");
            }

            return tcs.Task;
        }

        #if __IOS__
        public async Task SycFusionCalendarDisableDates(SFCalendar calendar, UIView view, DateTime selectDate, DateTime startDate, DateTime endDate, List<DateTime> validDateList, bool disableHolidays, bool disableWeekends)
        #endif
        #if __ANDROID__
        public async Task SycFusionCalendarDisableDates(SfCalendar calendar, Activity view, DateTime selectDate, DateTime startDate, DateTime endDate, List<DateTime> validDateList, bool disableHolidays, bool disableWeekends)
        #endif
        {
            try
            {
                if (selectDate == DateTime.MinValue)
                {
                    if (startDate == DateTime.MinValue)
                    {
                        calendar.SelectedDate = DateHelper.DateTimeToOSDate(DateTime.Today.Date);
                    }
                    else
                    {
                        calendar.SelectedDate = DateHelper.DateTimeToOSDate(startDate);
                    }
                }
                else
                {
                    calendar.SelectedDate = DateHelper.DateTimeToOSDate(selectDate);
                }

                if (startDate == DateTime.MinValue)
                {
                    if (validDateList != null && validDateList.Count > 0)
                    {
                        startDate = validDateList[0];
                    }
                    else
                    {
                        startDate = DateTime.Today.AddMonths(-6);
                    }
                }

                if (endDate == DateTime.MinValue)
                {
                    if (validDateList != null && validDateList.Count > 0)
                    {
                        endDate = validDateList[validDateList.Count - 1];
                    }
                    else
                    {
                        endDate = DateTime.Today.AddYears(1);
                    }
                }

                var blackOutDays = new List<DateTime>();
                var holidays = new List<DateTime>();

                if (disableHolidays)
                {
                    var methods = new BillPayMethods();
                    var request = new GetHolidaysRequest();

                    var response = await methods.GetHolidaysAsList(request, view);

                    if (response != null)
                    {
                        holidays = response;
                        blackOutDays.AddRange(holidays);
                    }
                }

                var date = startDate;

                while (date <= endDate)
                {
                    if (disableWeekends && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                    {
                        blackOutDays.Add(date);
                    }

                    if (validDateList != null && !validDateList.Contains(date))
                    {
                        blackOutDays.Add(date);
                    }

                    date = date.AddDays(1);
                }

                calendar.BlackoutDates = DateHelper.DateTimeListToOSArray(blackOutDays);
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "SyncFusionHelper:CreateSycFusionCalendar");
            }
        }
    }
}