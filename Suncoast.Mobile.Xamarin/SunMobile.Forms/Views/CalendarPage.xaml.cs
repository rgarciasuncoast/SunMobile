using System;
using System.Collections.Generic;
using Syncfusion.SfCalendar.XForms;
using Xamarin.Forms;

namespace SunMobile.Forms.Views
{
    public partial class CalendarPage : ContentPage
    {
        List<DateTime> black_dates;

        public CalendarPage()
        {
            InitializeComponent();
            this.setBlackOutDates();
            this.sampleSettings();
        }

        void sampleSettings()
        {
            this.Padding = new Thickness(-10);
            MonthViewSettings monthSettings = new MonthViewSettings();
            monthSettings.DayHeight = 50;
            monthSettings.HeaderBackgroundColor = Color.White;
            monthSettings.InlineBackgroundColor = Color.FromHex("#EEEEEE");
            monthSettings.DateSelectionColor = Color.FromHex("#EEEEEE");
            monthSettings.TodayTextColor = Color.FromHex("#2196F3");
            monthSettings.SelectedDayTextColor = Color.Black;
            calendar.MonthViewSettings = monthSettings;

            if (Device.OS == TargetPlatform.Android)
            {
                calendar.HeaderHeight = 50;
                //sampleLayout.Padding = new Thickness(10, 10, 10, 10);

            }

            if (Device.OS == TargetPlatform.iOS)
            {
                calendar.HeaderHeight = 40;
                //sampleLayout.Padding = new Thickness(10, 10, 10, 0);
            }
        }

        void setBlackOutDates()
        {
            black_dates = new List<DateTime>();

            for (int i = 0; i < 5; i++)
            {
                DateTime date = DateTime.Now.Date.AddDays(i + 7);
                black_dates.Add(date);
            }

            for (int i = 0; i < 5; i++)
            {
                DateTime date = DateTime.Now.Date.AddDays(i - 7);
                black_dates.Add(date);
            }
            calendar.BlackoutDates = black_dates;
        }

        public View getContent()
        {
            return this.Content;
        }
    }
}