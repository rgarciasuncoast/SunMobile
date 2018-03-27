using System;
using Foundation;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using UIKit;

namespace SunMobile.iOS.Cards
{
	public partial class CardsMenuTableViewController : BaseTableViewController, ICultureConfigurationProvider
	{
		public CardsMenuTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();			

			// Hides the remaining rows.
			tableViewMenu.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row)
			{
				case 0: // Travel Notifications
					var travelNotificationsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("TravelNotificationsTableViewController") as TravelNotificationsTableViewController;
					NavigationController.PushViewController(travelNotificationsTableViewController, true);
					break;
				case 1: // Tampa Bay Rays Card
                    var orderRaysCardViewController = AppDelegate.StoryBoard.InstantiateViewController("OrderRaysCardViewController") as OrderRaysCardViewController;
					NavigationController.PushViewController(orderRaysCardViewController, true);
					break;
			}
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "1e9837a3-f890-4b1e-94f0-2699e849674b", "Cards");
			lblTravelNotifications.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2e4602e6-9afa-43ac-8f44-e255236cc1e4", "Travel Notifications");
		}
	}
}