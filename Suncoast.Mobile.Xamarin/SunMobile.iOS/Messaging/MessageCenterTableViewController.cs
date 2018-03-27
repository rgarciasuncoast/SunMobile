using System;
using Foundation;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Messaging
{
	public partial class MessageCenterTableViewController : BaseTableViewController
	{
		public MessageCenterTableViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Hides the remaining rows.
			tableViewMenu.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);				

			CommonMethods.AddBottomToolbar(this);

			ClearAll();

			GetUnreadCounts();			
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("40981c0e-b85e-4af0-ad28-cb45c03dc63e", "d039c0f3-8567-45db-a3cd-553fb21be06c", "Message Center");
			CultureTextProvider.SetMobileResourceText(lblMessageCenterInbox, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "93087ed5-e3b7-4bd3-b9a3-96db09c353df", "Inbox");
			CultureTextProvider.SetMobileResourceText(lblMessageCenterNotifications, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "960985bb-d6c0-4d71-8726-4b517533d5b5", "Notifications");
			CultureTextProvider.SetMobileResourceText(lblMessageCenterSent, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "af3aa5c3-efc2-423e-bc3c-3483cf8c7e44", "Sent");
			CultureTextProvider.SetMobileResourceText(lblMessageCenterComposeMessage, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "cd011a01-6956-4f6f-be2f-e84ef84a196d", "Compose Message");
		}

		private void ClearAll()
		{
			lblSecuredInboxUnreadCount.Text = string.Empty;
			lblSecuredNotificationsUnreadCount.Text = string.Empty;
		}

		private async void GetUnreadCounts()
		{	
			var methods = new MessagingMethods();
			var response = await methods.GetUnreadMessageCounts(null, View);

			if (response != null && response.Success)
			{
				lblSecuredInboxUnreadCount.Text = response.NewSecureMessagesCount.ToString();
				lblSecuredNotificationsUnreadCount.Text = response.NewEnotificationsCount.ToString();
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Section)
			{
				case 0:
					switch (indexPath.Row)
					{
						case 0: // Secured Messaging Inbox
							var securedMessagingInboxController = AppDelegate.StoryBoard.InstantiateViewController("MessagesViewController") as MessagesViewController;
							securedMessagingInboxController.MessageViewTypes = MessageTypes.SecuredMessagingInbox;
							securedMessagingInboxController.MessageCountsChanged += obj => GetUnreadCounts();
							NavigationController.PushViewController(securedMessagingInboxController, true);
							break;
						case 1: // Secured Messaging Notifications
							var securedMessagingNotificationsController = AppDelegate.StoryBoard.InstantiateViewController("MessagesViewController") as MessagesViewController;
							securedMessagingNotificationsController.MessageViewTypes = MessageTypes.SecuredMessagingNotifications;
							securedMessagingNotificationsController.MessageCountsChanged += obj => GetUnreadCounts();
							NavigationController.PushViewController(securedMessagingNotificationsController, true);
							break;
						case 2:  // Secured Mesasging Sent
							var securedMessagingSentController = AppDelegate.StoryBoard.InstantiateViewController("MessagesViewController") as MessagesViewController;
							securedMessagingSentController.MessageViewTypes = MessageTypes.SecuredMessagingSent;
							securedMessagingSentController.MessageCountsChanged += obj => GetUnreadCounts();
							NavigationController.PushViewController(securedMessagingSentController, true);
							break;		
						case 3:  // Secured Messaging Compose
							var messageComposeViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageComposeViewController") as MessageComposeViewController;
							messageComposeViewController.Thread = null;
							NavigationController.PushViewController(messageComposeViewController, true);
							break;
					}
					break;
			}
		}
	}
}