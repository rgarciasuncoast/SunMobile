using System;
using Foundation;
using SunMobile.iOS.Common;
using SunMobile.iOS.Locations;
using SunMobile.iOS.Messaging;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.About
{
	public partial class ContactUsTableViewController : BaseTableViewController
	{
		public ContactUsTableViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			try
			{
				lblVersion.Text = GeneralUtilities.GetAppVersion();
				lblContactInfo.Text = SessionSettings.Instance.GetStartupSettings["ContactInfo"].Replace("\\n", "\n");

				btnPhoneSpecialist.SetTitle(SessionSettings.Instance.GetStartupSettings["PhoneSpecialist"], UIControlState.Normal);
				btnPhoneSpecialist.TouchUpInside += CallNumber;

				btnPhoneCreditCards.SetTitle(SessionSettings.Instance.GetStartupSettings["PhoneCreditCards"], UIControlState.Normal);
				btnPhoneCreditCards.TouchUpInside += CallNumber;

				btnPhoneDebitCards.SetTitle(SessionSettings.Instance.GetStartupSettings["PhoneDebitCards"], UIControlState.Normal);
				btnPhoneDebitCards.TouchUpInside += CallNumber;

				btnPhoneDebitCardsAfterHours.SetTitle(SessionSettings.Instance.GetStartupSettings["PhoneDebitCardsAfterHours"], UIControlState.Normal);
				btnPhoneDebitCardsAfterHours.TouchUpInside += CallNumber;

				btnSendEmail.SetTitle(SessionSettings.Instance.GetStartupSettings["SupportEmail"], UIControlState.Normal);
				btnSendEmail.TouchUpInside += SendEmail;

				btnSendMessage.TouchUpInside += SendMessage;

				// Hides the remaining rows.
				tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Error in ContactUsTableViewController:ViewDidLoad");
			}			
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("55027AD1-817A-4206-8398-56BA3B6EBB6E", "D7C9182E-E4DF-4161-8967-F5B93E3BC1B2", "Contact Us");
			CultureTextProvider.SetMobileResourceText(lblInfo, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "8DD68B42-604C-46FC-B0CA-0922DA918E49", "Members Care Center\\n Mon - Fri 7am - 8pm Easter\\n Sat 8am-1pm Eastern");
			CultureTextProvider.SetMobileResourceText(lblSpeakToASpecialist, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "E2D88609-F679-4C7A-93AC-A189CFF6E6E4", "Speak to a Specialist");
			CultureTextProvider.SetMobileResourceText(lblReportLostAndStolenCards, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "EB9F49C5-68A9-4028-A737-8E9380DA094F", "Report Lost & Stolen Cards");
			CultureTextProvider.SetMobileResourceText(lblCreditCards, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "B3EF4DA7-E85A-4FD8-A426-BBE5947F97E4", "Credit Cards");
			CultureTextProvider.SetMobileResourceText(lblDebitATMCards, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "30E98B2B-7435-4C26-AD57-8D77935C129B", "Debit/ATM Cards");
			CultureTextProvider.SetMobileResourceText(lblDebitATMCardsAfterHours, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "271B49FD-7982-4A6C-976D-C0441268A5C1", "Debit/ATM Cards after hours");
			CultureTextProvider.SetMobileResourceText(lblDigitalBankingEmail, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "CC768CEE-B784-47ED-8C02-B93F3DE6ED06", "Digital Banking Email");
			CultureTextProvider.SetMobileResourceText(lblRoutingNumber, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "14AE6724-39AA-4964-BDB8-BDDD7008543B", "Routing Number");
			CultureTextProvider.SetMobileResourceText(lblFindABranchOrATM, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "6F7564E9-F36E-436E-A2DC-7D62AE284CEC", "Find a Branch or ATM");
			CultureTextProvider.SetMobileResourceText(lblFacebook, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "D93F6E03-B2FA-42DB-A24B-167C078EB6BD", "Suncoast Facebook Page");
			CultureTextProvider.SetMobileResourceText(lblTwitter, "55027AD1-817A-4206-8398-56BA3B6EBB6E", "C565E50A-0853-4FEE-A50D-196335DE9F69", "Suncoast Twitter Page");
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			try
			{
				switch (indexPath.Row)
				{
					case 4: // Locations
						var locationsViewController = AppDelegate.StoryBoard.InstantiateViewController("LocationsViewController") as LocationsViewController;
						NavigationController.PushViewController(locationsViewController, true);
						break;
					case 5: // Website
						var websiteViewController = AppDelegate.StoryBoard.InstantiateViewController("WebViewController") as WebViewController;
						websiteViewController.Url = SessionSettings.Instance.GetStartupSettings["SunnetUrl"];
						websiteViewController.Title = "Suncoast Credit Union";
						NavigationController.PushViewController(websiteViewController, true);
						break;
					case 6: // Facebook
						var faceBookViewController = AppDelegate.StoryBoard.InstantiateViewController("WebViewController") as WebViewController;
						faceBookViewController.Url = SessionSettings.Instance.GetStartupSettings["FacebookUrl"];
						faceBookViewController.Title = "Suncoast Facebook";
						NavigationController.PushViewController(faceBookViewController, true);
						break;
					case 7: // Twitter
						var twitterViewController = AppDelegate.StoryBoard.InstantiateViewController("WebViewController") as WebViewController;
						twitterViewController.Url = SessionSettings.Instance.GetStartupSettings["TwitterUrl"];
						twitterViewController.Title = "Suncoast Twitter";
						NavigationController.PushViewController(twitterViewController, true);
						break;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Error in ContactUsTableViewController:RowSelected");
			}
		}

		private void SendEmail(object sender, EventArgs e)
		{
			try
			{
				GeneralUtilities.SendEmail(this, btnSendEmail.Title(UIControlState.Normal), "SunMobile", string.Empty, true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Error in ContactUsTableViewController:SendEmail");
			}
		}

		private void SendMessage(object sender, EventArgs e)
		{
			try
			{
				var messageViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageCenterTableViewController") as MessageCenterTableViewController;
				AppDelegate.MenuNavigationController.PushViewController(messageViewController, true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Error in ContactUsTableViewController:SendMessage");
			}
		}

		private void CallNumber(object sender, EventArgs e)
		{
			try
			{
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				{
					string phoneNumber = string.Empty;

					if (sender is UIButton)
					{
						phoneNumber = ((UIButton)sender).Title(UIControlState.Normal).Replace("(", string.Empty).Replace(")", string.Empty).Replace(" ", "-");
					}

					var url = new NSUrl(string.Format(@"telprompt://{0}", phoneNumber));
					UIApplication.SharedApplication.OpenUrl(url);
				}			
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Error in ContactUsTableViewController:PhoneCall");
			}
		}
	}
}