using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunMobile.iOS.Common;
using SunMobile.iOS.Documents;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class AccountSettingsTableViewController : BaseTableViewController
	{
		public AccountSettingsTableViewController(IntPtr handle) : base(handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hide the remaining rows
            tableViewMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            LoadAccountPreferences();
        }

        private async void LoadAccountPreferences()
        {
            var methods = new AccountMethods();
            var request = new AccountListRequest();

            ShowActivityIndicator();

            var response = await methods.AccountList(request, View);

            HideActivityIndicator();

            if (response?.ClientViewState != null && response.ClientViewState == "AccountList")
            {
                var tableViewSource = new AccountSettingsTableViewSource(response);

                tableViewSource.ShowDetails += (obj) =>
                {
                    var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
                    documentViewerViewController.DocumentType = Shared.Data.DocumentViewerTypes.Url;
                    documentViewerViewController.Urls = new List<string> { "https://sunblockstorage.blob.core.windows.net/documents/eStatement-Disclosure.pdf" };
                    NavigationController.PushViewController(documentViewerViewController, true);
                };

                tableViewSource.AccountSelected += item =>
                {
                    var controller = AppDelegate.StoryBoard.InstantiateViewController("AccountSpecificSettingsTableViewController") as AccountSpecificSettingsTableViewController;
                    controller.SelectedAccount = item;
                    NavigationController.PushViewController(controller, true);
                };

                tableViewMain.Source = tableViewSource;
                tableViewMain.ReloadData();
            }
        }
	}
}