using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Products;
using SunMobile.Droid.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Sharing;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Accounts
{
    public class PayoffQuoteFragment : BaseFragment
    {
        private TableRow rowPayoffDate;
        private TextView txtDate;
        private WebView webView;
        private ImageView btnShare;
        private ImageView btnPrint;
        private string _tempFullFileName;

        public Account Account { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {          
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.PayoffQuoteView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            ((MainActivity)Activity).SetActionBarTitle("Payoff Quote");

            rowPayoffDate = Activity.FindViewById<TableRow>(Resource.Id.rowPayoffSelectDate);
            rowPayoffDate.Click += (sender, e) => SelectDate();
            txtDate = Activity.FindViewById<TextView>(Resource.Id.lblDate);
            webView = Activity.FindViewById<WebView>(Resource.Id.webView);
            btnShare = Activity.FindViewById<ImageView>(Resource.Id.btnShare);
            btnShare.Click += (sender, e) => Share();
            btnPrint = Activity.FindViewById<ImageView>(Resource.Id.btnPrint);
            btnPrint.Click += (sender, e) => Print();

            ClearAll();
        }

        private void ClearAll()
        {
            txtDate.Text = string.Empty;
        }

        private void SelectDate()
        {
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                var intent = new Intent(Activity, typeof(SelectDateActivity));

                var dateList = new List<DateTime>();

                for (int i = 0; i < Account.PayoffEligibility.ValidDatesForPayoff.Count; i++)
                {
                    dateList.Add(DateTime.ParseExact(Account.PayoffEligibility.ValidDatesForPayoff[i], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                }

                if (dateList.Count > 0)
                {
                    intent.PutExtra("SelectDate", dateList[0].ToString());
                }

                var json = JsonConvert.SerializeObject(dateList);
                intent.PutExtra("ValidDates", json);
                intent.PutExtra("DisableHolidays", false);
                intent.PutExtra("DisableWeekends", false);
                StartActivityForResult(intent, 100); 
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "PayoffQuoteFragment:SelectDate");
            }
        }

        private async Task GetPayoffInfo(DateTime payoffDate)
        {
            try
            {
                var methods = new AccountMethods();
                var request = new PayoffQuoteRequest
                {
                    LoanSuffix = Account.Suffix,
                    DateOfPayoff = payoffDate.ToString("yyyyMMdd")
                };

                ShowActivityIndicator();

                var response = await methods.GetPayoffQuote(request, Activity);

                HideActivityIndicator();

                if (response != null && !string.IsNullOrEmpty(response.ViewTemplate))
                {
                    webView.LoadData(response.ViewTemplate, "text/html", "UTF-8");
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "PayoffTableViewFragment:GetPayoffInfo");
            }
        }

        private void Share()
        {
            try
            {
                var bitmap = Images.ConvertWebViewToBitmap(webView);

                if (bitmap != null)
                {
                    var fileBytes = Images.ConvertBitmapToByteArray(bitmap);

                    if (fileBytes != null)
                    {
                        _tempFullFileName = Sharing.Share(Activity, "payoffquote.jpg", fileBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "PayoffTableViewFragment:Share");
            }
        }

        private void Print()
        {
            Sharing.Print(Activity, webView);
        }

        public override void OnStop()
        {
            IsolatedStorage.DeleteFile(_tempFullFileName);

            base.OnStop();
        }

        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 100 && resultCode == (int)Result.Ok)
            {
                var json = data.GetStringExtra("SelectedDate");
                var selectedDate = JsonConvert.DeserializeObject<DateTime>(json);
                txtDate.Text = selectedDate.Date.ToString("d");
                await GetPayoffInfo(selectedDate);
            }
        }
    }
}