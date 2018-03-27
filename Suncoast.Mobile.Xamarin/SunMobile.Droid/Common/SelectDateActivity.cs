using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Com.Syncfusion.Calendar;
using Newtonsoft.Json;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Utilities.SyncFusion;

namespace SunMobile.Droid.Common
{
    [Activity(Label = "SelectDateActivity", Theme = "@style/CustomHoloLightTheme")]
    public class SelectDateActivity : BaseActivity
    {
        private LinearLayout mainLinearLayout;
        private ImageButton btnCloseWindow;
        private Button btnSelect;
        private TextView txtTitle;
        private SfCalendar calendar;

        private bool _disableHolidays;
        private bool _disableWeekends;
        private string _selectDateString;
        private string _startDateString;
        private string _endDateString;
        private DateTime _selectDate;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<DateTime> _holidays;
        private List<DateTime> _validDateList;

        private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SelectDateView);

            _disableHolidays = Intent.GetBooleanExtra("DisableHolidays", false);
            _disableWeekends = Intent.GetBooleanExtra("DisableWeekends", false);
            _selectDateString = Intent.GetStringExtra("SelectDate");
            _startDateString = Intent.GetStringExtra("StartDate");
            _endDateString = Intent.GetStringExtra("EndDate");
            var json = Intent.GetStringExtra("ValidDates");

            if (json != null)
            {
                _validDateList = JsonConvert.DeserializeObject<List<DateTime>>(json);
            }

            if (savedInstanceState != null)
            {
                _disableHolidays = savedInstanceState.GetBoolean("DHolidays");
                _disableWeekends = savedInstanceState.GetBoolean("DWeekends");
                json = savedInstanceState.GetString("SelectDate");
                _selectDate = JsonConvert.DeserializeObject<DateTime>(json);
                json = savedInstanceState.GetString("StartDate");
                _startDate = JsonConvert.DeserializeObject<DateTime>(json);
                json = savedInstanceState.GetString("EndDate");
                _endDate = JsonConvert.DeserializeObject<DateTime>(json);
                json = savedInstanceState.GetString("Holidays");
                _holidays = JsonConvert.DeserializeObject<List<DateTime>>(json);
                json = savedInstanceState.GetString("ValidDates");
                _validDateList = JsonConvert.DeserializeObject<List<DateTime>>(json);
            }

            mainLinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

            btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
            btnCloseWindow.Click += (sender, e) => Finish();

            btnSelect = FindViewById<Button>(Resource.Id.btnSelect);
            btnSelect.Click += (sender, e) => SelectDate();

            if (!string.IsNullOrEmpty(_startDateString))
            {
                var _strippedStartDateString = _startDateString.Split(' ')[0] + " 00:00:00 AM";
                _startDate = DateTime.Parse(_strippedStartDateString);
            }
            else
            {
                _startDate = DateTime.MinValue;
            }

            if (!string.IsNullOrEmpty(_endDateString))
            {
                var _strippedEndDateString = _endDateString.Split(' ')[0] + " 00:00:00 AM";
                _endDate = DateTime.Parse(_strippedEndDateString);
            }
            else
            {
                _endDate = DateTime.MinValue;
            }

            if (!string.IsNullOrEmpty(_selectDateString))
            {
                var _strippedSelectDateString = _selectDateString.Split(' ')[0] + " 00:00:00 AM";
                _selectDate = DateTime.Parse(_strippedSelectDateString);
            }
            else
            {
                _selectDate = DateTime.MinValue;
            }

            CreateCalendar();
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(txtTitle, cultureViewId, "F98BE3D8-0BB5-455D-A411-80D4D8517978");
                CultureTextProvider.SetMobileResourceText(btnSelect, cultureViewId, "AB1A9E32-12DD-43C5-8F42-500E98DE7AB8");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SelectDateActivity:SetCultureConfiguration");
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            var json = JsonConvert.SerializeObject(_startDate);
            outState.PutString("StartDate", json);
            json = JsonConvert.SerializeObject(_holidays);
            outState.PutString("Holidays", json);
            json = JsonConvert.SerializeObject(_validDateList);
            outState.PutString("ValidDates", json);
            outState.PutBoolean("DHolidays", _disableHolidays);
            outState.PutBoolean("DWeekends", _disableWeekends);

            base.OnSaveInstanceState(outState);
        }

        private async void CreateCalendar()
        {
            ShowActivityIndicator();

            var helper = new SyncFusionHelper();
            calendar = await helper.SycFusionCalendarCreate(this, mainLinearLayout, _selectDate, _startDate, _endDate, _validDateList);

            HideActivityIndicator();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            ShowActivityIndicator();

            var handler = new Handler();
            handler.PostDelayed(async () =>
            {
                ShowActivityIndicator();
                var helper = new SyncFusionHelper();
                await helper.SycFusionCalendarDisableDates(calendar, this, _selectDate, _startDate, _endDate, _validDateList, _disableHolidays, _disableWeekends);
                HideActivityIndicator();
            }, 100);
        }

        private void SelectDate()
        {
            var intent = new Intent();
            intent.PutExtra("ClassName", "SelectDateActivity");
            var json = JsonConvert.SerializeObject(DateHelper.OSDateToDateTime((Java.Util.GregorianCalendar)calendar.SelectedDate));
            intent.PutExtra("SelectedDate", json);
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}