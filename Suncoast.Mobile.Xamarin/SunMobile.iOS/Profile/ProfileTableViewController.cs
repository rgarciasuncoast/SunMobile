using System;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using UIKit;

namespace SunMobile.iOS.Profile
{
    public partial class ProfileTableViewController : BaseTableViewController
	{
        private static readonly string cultureViewId = "9EA5D29A-8CD2-40E4-861B-290FB3C4A864";

		public ProfileTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Hides the remaining rows.
			tableViewMenu.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			CommonMethods.AddBottomToolbar(this);
		}

		public override void SetCultureConfiguration()
		{
			try
			{
				var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "92FE93E5-E3E1-43E5-9390-483DB48654E6");

				if (!string.IsNullOrEmpty(viewText))
				{
					Title = viewText;
				}

                CultureTextProvider.SetMobileResourceText(lblProfileContactInfo, cultureViewId, "06B488C2-82FB-4029-BE4E-FF4D2AD397A7");
                CultureTextProvider.SetMobileResourceText(lblProfileElectronicDocumentEnrollment, cultureViewId, "ADE900A8-F1A4-4D00-8BD3-134D91BB5BF4");
                CultureTextProvider.SetMobileResourceText(lblProfileManageAlertSettings, cultureViewId, "15B4185A-2541-462B-8250-06F58164A0DC");
                CultureTextProvider.SetMobileResourceText(lblProfileUpdatePassword, cultureViewId, "1F5DC964-15B6-4A56-93EF-73EF21BDEE3C");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactInfoViewController:SetCultureConfiguration");
			}
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			try
			{
				switch (indexPath.Row)
				{
					case 0:
						var contactInfoViewController = AppDelegate.StoryBoard.InstantiateViewController("ContactInfoViewController") as ContactInfoViewController;
                        contactInfoViewController.EditMode = true;
						NavigationController.PushViewController(contactInfoViewController, true);
						break;
					case 1:
						var updatePasswordViewController = AppDelegate.StoryBoard.InstantiateViewController("UpdatePasswordViewController") as UpdatePasswordViewController;
						NavigationController.PushViewController(updatePasswordViewController, true);
						break;
					case 2:
                        var accountSettingsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountSettingsTableViewController") as AccountSettingsTableViewController;
                        NavigationController.PushViewController(accountSettingsTableViewController, true);
						break;
					case 3:
						var manageAlertsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("ManageAlertsTableViewController") as ManageAlertsTableViewController;
						NavigationController.PushViewController(manageAlertsTableViewController, true);
						break;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ProfileTableViewController:RowSelected");
			}
		}
	}
}