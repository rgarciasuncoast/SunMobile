using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.iOS.Common;
using SunMobile.iOS.Documents;
using UIKit;

namespace SunMobile.iOS.Profile
{
    public partial class AccountSpecificSettingsTableViewController : BaseTableViewController
	{
        public Account SelectedAccount { get; set; }

		public AccountSpecificSettingsTableViewController(IntPtr handle) : base(handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = SelectedAccount.Description;

            // Hide the remaining rows
            tableViewMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            LoadSettings();
        }

        private void LoadSettings()
        {
            var tableViewSource = new AccountSpecificSettingsTableViewSource(null);

            tableViewSource.ShowDetails += (obj) =>
            {
                var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
                documentViewerViewController.DocumentType = Shared.Data.DocumentViewerTypes.Url;
                documentViewerViewController.Urls = new List<string> { "https://www.suncoastcreditunion.com/~/media/files/fees/smart_checking_fees%20pdf.ashx" };
                NavigationController.PushViewController(documentViewerViewController, true);
            };

            tableViewMain.Source = tableViewSource;
            tableViewMain.ReloadData();
        }
	}
}