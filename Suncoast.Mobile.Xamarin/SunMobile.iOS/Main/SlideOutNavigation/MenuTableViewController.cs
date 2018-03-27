using System;
using Foundation;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunMobile.iOS.About;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.BillPay;
using SunMobile.iOS.Cards;
using SunMobile.iOS.Common;
using SunMobile.iOS.Deposits;
using SunMobile.iOS.Documents;
using SunMobile.iOS.ExternalServices;
using SunMobile.iOS.LoanCenter;
using SunMobile.iOS.Locations;
using SunMobile.iOS.Messaging;
using SunMobile.iOS.Profile;
using SunMobile.iOS.Transfers;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Main
{
	public partial class MenuTableViewController : UITableViewController
	{
		public MenuTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            SetCultureConfiguration();

			View.TintColor = AppStyles.TintColor;
			EdgesForExtendedLayout = UIRectEdge.None;

			// Hides the remaining rows.
			tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, AppBecameActive);

			tableView.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellHeder.BackgroundColor = AppStyles.BarTintColor;
			cellAccounts.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellCards.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellTransfers.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellDeposits.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellBillPay.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellLoanCenter.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellSunMoney.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellLocations.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellMessageCenter.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellDocumentCenter.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellProfile.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellAbout.BackgroundColor = AppStyles.MenuBackgroundColor;
			cellSignOut.BackgroundColor = AppStyles.MenuBackgroundColor;

			labelAccounts.TextColor = AppStyles.MenuTextColor;
			labelCards.TextColor = AppStyles.MenuTextColor;
			labelTransfers.TextColor = AppStyles.MenuTextColor;
			labelDeposits.TextColor = AppStyles.MenuTextColor;
			labelBillPay.TextColor = AppStyles.MenuTextColor;
			labelLoanCenter.TextColor = AppStyles.MenuTextColor;
			labelSunMoney.TextColor = AppStyles.MenuTextColor;
			labelLocations.TextColor = AppStyles.MenuTextColor;
			labelMessageCenter.TextColor = AppStyles.MenuTextColor;
			labelDocumentCenter.TextColor = AppStyles.MenuTextColor;
			labelProfile.TextColor = AppStyles.MenuTextColor;
			labelAbout.TextColor = AppStyles.MenuTextColor;
			labelSignOut.TextColor = AppStyles.MenuTextColor;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				CommonMethods.AddBottomToolbar(this);
			}
		}

		public void SetCultureConfiguration()
		{
            labelAccounts.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "956E2062-2EA2-44BD-884F-A92EFDECDF9F", "Accounts");
            labelCards.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "2481D95B-D465-4B87-AA3A-3CC420B1C296", "Cards");
            labelTransfers.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "294EB33E-925F-42A6-8560-F7C3676530D3", "Transfer Funds");
            labelDeposits.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "99446298-1B9F-41BA-894A-875772D7CF4E", "Deposit Funds");
            labelBillPay.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "FC274687-8339-4618-8960-5C202AC14881", "Bill Pay");
            labelLoanCenter.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "72B7A071-14D7-40BB-B1F7-F6C5F94BF188", "Loan Center");
            labelSunMoney.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "B888D315-F938-4283-BE4A-8624454218FB", "SunMoney");
            labelLocations.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "2896CB59-E999-49A9-9059-4E12786FA1B5", "Find ATM/Branch");
            labelMessageCenter.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "26C013AC-ED93-406F-8DE9-DBB33579F8B5", "Message Center");
            labelDocumentCenter.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "9F23B326-B342-47F2-A4BC-D4578D17F9E2", "Documents");
            labelProfile.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "91E519FD-02C1-479B-A770-AC20CE256EB2", "My Profile");
            labelAbout.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "9EC061A4-3B17-412A-87C7-FE119ECF96D5", "Contact Us");
            labelSignOut.Text = CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "7096369F-20F4-4124-97DC-CC9F72F8BFFF", "Sign Out");					
		}

        public void SetAccessabilityLabels()
        {
            labelAccounts.AccessibilityLabel = "Accounts";
            labelCards.AccessibilityLabel = "Cards";
            labelTransfers.AccessibilityLabel = "Transfer Funds";
            labelDeposits.AccessibilityLabel = "Deposit Funds";
            labelBillPay.AccessibilityLabel = "Bill Pay";
            labelLoanCenter.AccessibilityLabel = "Loan Center";
            labelSunMoney.AccessibilityLabel = "Sun Money";
            labelLocations.AccessibilityLabel = "Find ATMs and Branches";
            labelMessageCenter.AccessibilityLabel = "Message Center";
            labelDocumentCenter.AccessibilityLabel = "Documents";
            labelProfile.AccessibilityLabel = "My Profile";
            labelAbout.AccessibilityLabel = "Contact Us";
            labelSignOut.AccessibilityLabel = "Sign Out";

            labelAccounts.AccessibilityElementsHidden = false;
            labelCards.AccessibilityElementsHidden = false;
            labelTransfers.AccessibilityElementsHidden = false;
            labelDeposits.AccessibilityElementsHidden = false;
            labelBillPay.AccessibilityElementsHidden = false;
            labelLoanCenter.AccessibilityElementsHidden = false;
            labelSunMoney.AccessibilityElementsHidden = false;
            labelLocations.AccessibilityElementsHidden = false;
            labelMessageCenter.AccessibilityElementsHidden = false;
            labelDocumentCenter.AccessibilityElementsHidden = false;
            labelProfile.AccessibilityElementsHidden = false;
            labelAbout.AccessibilityElementsHidden = false;
            labelSignOut.AccessibilityElementsHidden = false;
        }

        public void RemoveAccessabilityLabels()
        {
            labelAccounts.AccessibilityElementsHidden = true;
            labelCards.AccessibilityElementsHidden = true;
            labelTransfers.AccessibilityElementsHidden = true;
            labelDeposits.AccessibilityElementsHidden = true;
            labelBillPay.AccessibilityElementsHidden = true;
            labelLoanCenter.AccessibilityElementsHidden = true;
            labelSunMoney.AccessibilityElementsHidden = true;
            labelLocations.AccessibilityElementsHidden = true;
            labelMessageCenter.AccessibilityElementsHidden = true;
            labelDocumentCenter.AccessibilityElementsHidden = true;
            labelProfile.AccessibilityElementsHidden = true;
            labelAbout.AccessibilityElementsHidden = true;
            labelSignOut.AccessibilityElementsHidden = true;
        }

		private async void AppBecameActive(NSNotification notification)
		{
			if (SessionSettings.Instance.IsAuthenticated)
			{
				var methods = new AuthenticationMethods();

				var request = new AnalyzeRequest
				{
					UserId = SessionSettings.Instance.UserId
				};

				await methods.SessionIsActive(request, null);
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row)
			{
				case 1:
					var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(accountsViewController, true);
					break;
				case 2:
					var cardsMenuTableViewController = AppDelegate.StoryBoard.InstantiateViewController("CardsMenuTableViewController") as CardsMenuTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(cardsMenuTableViewController, true);
					break;
				case 3:
					var transfersTableViewController = AppDelegate.StoryBoard.InstantiateViewController("TransfersTableViewController") as TransfersTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(transfersTableViewController, true);
					break;
				case 4:
					var depositsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("DepositsTableViewController") as DepositsTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(depositsTableViewController, true);
					break;
				case 5:
					var billPayViewController = AppDelegate.StoryBoard.InstantiateViewController("BillPayMenuTableViewController") as BillPayMenuTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(billPayViewController, true);
					break;
				case 6:
                    var loanCenterMenuTableViewController = AppDelegate.StoryBoard.InstantiateViewController("LoanCenterMenuTableViewController") as LoanCenterMenuTableViewController;
					CommonMethods.PopToRootIfOniPad();
                    AppDelegate.MenuNavigationController.PushViewController(loanCenterMenuTableViewController, true);
					break;
				case 7:
					var sunMoneyViewController = AppDelegate.StoryBoard.InstantiateViewController("SunMoneyViewController") as SunMoneyViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(sunMoneyViewController, true);
					break;
				case 8:
					var locationsViewController = AppDelegate.StoryBoard.InstantiateViewController("LocationsViewController") as LocationsViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(locationsViewController, true);
					break;
				case 9:
					var messageCenterTableViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageCenterTableViewController") as MessageCenterTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(messageCenterTableViewController, true);
					break;    
				case 10:
					var documentsMenuTableViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentsMenuTableViewController") as DocumentsMenuTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(documentsMenuTableViewController, true);
					break;
				case 11:
					var profileViewController = AppDelegate.StoryBoard.InstantiateViewController("ProfileTableViewController") as ProfileTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(profileViewController, true);
					break;
				case 12:
					var contactUsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("ContactUsTableViewController") as ContactUsTableViewController;
					CommonMethods.PopToRootIfOniPad();
					AppDelegate.MenuNavigationController.PushViewController(contactUsTableViewController, true);
					break;
				case 13:
					AppDelegate.MenuNavigationController.SignOut();
					break;
			}
		}
	}
}