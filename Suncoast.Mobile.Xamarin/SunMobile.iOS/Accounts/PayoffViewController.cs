using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Products;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Sharing;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Images;
using UIKit;

namespace SunMobile.iOS.Accounts
{
    public partial class PayoffViewController : BaseViewController
    {
        public Account Account { get; set; }
        private DateTime _payoffDate;
        private PayoffTableViewSource _payoffTableViewSource;

        public PayoffViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var rightButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            rightButton.Clicked += (sender, e) => Done();

            btnPrint.Clicked += (sender, e) => Print();
            btnShare.Clicked += (sender, e) => Share();

            _payoffTableViewSource = new PayoffTableViewSource();
            _payoffTableViewSource.ItemSelected += () =>
            {
                SelectDate();
            };

            tablePayoffDate.Source = _payoffTableViewSource;
            tablePayoffDate.ReloadData();
        }

        private void SelectDate()
        {
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                var datePickerViewController = AppDelegate.StoryBoard.InstantiateViewController("DatePickerViewController") as DatePickerViewController;

                var dateList = new List<DateTime>();

                for (int i = 0; i < Account.PayoffEligibility.ValidDatesForPayoff.Count; i++)
                {
                    dateList.Add(DateTime.ParseExact(Account.PayoffEligibility.ValidDatesForPayoff[i], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
                }

                datePickerViewController.ValidDateList = dateList;

                if (dateList.Count > 0)
                {
                    datePickerViewController.SelectDate = dateList[0];
                }

                datePickerViewController.ItemSelected += async date =>
                {
                    _payoffDate = date;
                    _payoffTableViewSource.SetPayoffDate(date);
                    tablePayoffDate.ReloadData();
                    await GetPayoffInfo();
                };

                NavigationController.PushViewController(datePickerViewController, true);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "PayoffTableViewController:SelectDate");
            }
        }

        private async Task GetPayoffInfo()
        {
            try
            {
                var methods = new AccountMethods();
                var request = new PayoffQuoteRequest
                {
                    LoanSuffix = Account.Suffix,
                    DateOfPayoff = _payoffDate.ToString("yyyyMMdd")
                };

                ShowActivityIndicator();

                var response = await methods.GetPayoffQuote(request, View);

                HideActivityIndicator();

                if (response != null && !string.IsNullOrEmpty(response.ViewTemplate))
                {
                    webView.LoadHtmlString(response.ViewTemplate, null);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "PayoffTableViewController:GetPayoffInfo");
            }
        }

        private void Print()
        {
            Sharing.Print(webView);
        }

        private void Share()
        {
            try
            {
                var image = Images.ConvertViewToImage(webView);

                if (image != null)
                {
                    var fileBytes = Images.ConvertUIImageToBytes(image);

                    if (fileBytes != null)
                    {
                        Sharing.Share(this, fileBytes, btnShare);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "PayoffTableViewController:Share");
            }
        }

        private void Done()
        {
            NavigationController.PopViewController(true);
        }
    }
}