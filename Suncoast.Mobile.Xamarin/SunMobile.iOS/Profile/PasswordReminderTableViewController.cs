using System;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class PasswordReminderTableViewController : BaseTableViewController
	{
		public PasswordReminderTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Hides the remaining rows.
			tableViewMenu.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
		}

		public override async void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			try
			{
				switch (indexPath.Row)
				{
					case 1:
						var updatePasswordViewController = AppDelegate.StoryBoard.InstantiateViewController("UpdatePasswordViewController") as UpdatePasswordViewController;
						updatePasswordViewController.Completed += (obj) =>
						{
							NavigationController.PopViewController(true);
						};

						NavigationController.PushViewController(updatePasswordViewController, true);
						break;
					case 2:
						var request = new DelayPasswordNotificationRequest();

						ShowActivityIndicator();
						var methods = new AuthenticationMethods();
						await methods.DelayPasswordNotification(request, View);
						HideActivityIndicator();

						SessionSettings.Instance.ShowPasswordReminder = false;

						NavigationController.PopViewController(true);
						break;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Error in AboutTableViewController:RowSelected");			
			}
		}
	}
}