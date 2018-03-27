using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Utilities.SyncFusion;
using Syncfusion.SfCalendar.iOS;
using UIKit;

namespace SunMobile.iOS.Common
{
    public partial class DatePickerViewController : BaseViewController
    {
        public event Action<DateTime> ItemSelected = delegate { };
        public DateTime SelectDate { get; set; }
        public bool DisableHolidays { get; set; }
        public bool DisableWeekends { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DateTime> ValidDateList { get; set; }
        private SFCalendar _calendar;

        public DatePickerViewController(IntPtr handle) : base(handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "F98BE3D8-0BB5-455D-A411-80D4D8517978", "Select a Date");

            var buttonText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "AB1A9E32-12DD-43C5-8F42-500E98DE7AB8", "Select");
            var rightButton = new UIBarButtonItem(buttonText, UIBarButtonItemStyle.Plain, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            rightButton.Clicked += (sender, e) =>
            {
                ItemSelected(DateHelper.OSDateToDateTime(_calendar.SelectedDate));
                NavigationController.PopViewController(false);
            };

            await CreateCalendar();
        }

        private async Task CreateCalendar()
        {
            ShowActivityIndicator();

            var helper = new SyncFusionHelper();
            _calendar = await helper.SycFusionCalendarCreate(View, SelectDate, StartDate, EndDate, ValidDateList);
            await helper.SycFusionCalendarDisableDates(_calendar, View, SelectDate, StartDate, EndDate, ValidDateList, DisableHolidays, DisableWeekends);

            HideActivityIndicator();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            _calendar.Frame = new CoreGraphics.CGRect(0, 0,
                                                         (AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Width -
                                                         (AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Width < AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Height ? 0 : AppDelegate.MenuNavigationController.View.Frame.Width)),
                                                         AppDelegate.MenuNavigationController.CurrentViewController.View.Frame.Height);
        }
    }
}