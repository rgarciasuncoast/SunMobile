using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Messaging
{
	public class MessageCenterFragment : BaseFragment
	{
		private TableRow tableRowInbox;
		private TableRow tableRowNotifications;
		private TableRow tableRowSent;
		private TableRow tableRowComposeMessage;
		private TextView lblInboxCount;
		private TextView lblNotificationsCount;
		private TextView lblInbox;
		private TextView lblNotifications;
		private TextView lblSent;
		private TextView lblComposeMessage;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.MessageCenterView, null);
			RetainInstance = true;

			return view;
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("40981c0e-b85e-4af0-ad28-cb45c03dc63e", "d039c0f3-8567-45db-a3cd-553fb21be06c", "Message Center"));
                CultureTextProvider.SetMobileResourceText(lblInbox, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "93087ed5-e3b7-4bd3-b9a3-96db09c353df", "Inbox");
                CultureTextProvider.SetMobileResourceText(lblNotifications, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "960985bb-d6c0-4d71-8726-4b517533d5b5", "Notifications");
                CultureTextProvider.SetMobileResourceText(lblSent, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "af3aa5c3-efc2-423e-bc3c-3483cf8c7e44", "Sent");
                CultureTextProvider.SetMobileResourceText(lblComposeMessage, "40981c0e-b85e-4af0-ad28-cb45c03dc63e", "cd011a01-6956-4f6f-be2f-e84ef84a196d", "Compose Message");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "MessageCenterFragment:SetCultureConfiguration");
			}
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			tableRowInbox = Activity.FindViewById<TableRow>(Resource.Id.rowInbox);
			tableRowInbox.Click += (sender, e) => ListItemClicked(0);
			tableRowNotifications = Activity.FindViewById<TableRow>(Resource.Id.rowNotifications);
			tableRowNotifications.Click += (sender, e) => ListItemClicked(1);
			tableRowSent = Activity.FindViewById<TableRow>(Resource.Id.rowSent);
			tableRowSent.Click += (sender, e) => ListItemClicked(2);
			tableRowComposeMessage = Activity.FindViewById<TableRow>(Resource.Id.rowComposeMessage);
			tableRowComposeMessage.Click += (sender, e) => ListItemClicked(3);

			lblInbox = Activity.FindViewById<TextView>(Resource.Id.lblMessageCenterInbox);
			lblSent = Activity.FindViewById<TextView>(Resource.Id.lblMessageCenterSent);
			lblNotifications = Activity.FindViewById<TextView>(Resource.Id.lblMessageCenterNotifications);
			lblComposeMessage = Activity.FindViewById<TextView>(Resource.Id.lblMessageCenterComposeMessage);

			lblInboxCount = Activity.FindViewById<TextView>(Resource.Id.lblInboxCount);
			lblNotificationsCount = Activity.FindViewById<TextView>(Resource.Id.lblNotificationsCount);

			ClearAll();

			GetUnreadCounts();			
		}

		public void ClearAll()
		{
			lblInboxCount.Text = string.Empty;
			lblNotificationsCount.Text = string.Empty;
		}

		private async void GetUnreadCounts()
		{
			var methods = new MessagingMethods();
			var response = await methods.GetUnreadMessageCounts(null, Activity);

			if (response != null && response.Success)
			{
				lblInboxCount.Text = response.NewSecureMessagesCount.ToString();
				lblNotificationsCount.Text = response.NewEnotificationsCount.ToString();
			}
		}

		public void ListItemClicked(int position)
		{
			Android.Support.V4.App.Fragment fragment = null;

			switch (position)
			{
				case 0:
					fragment = new MessagesFragment();
					((MessagesFragment)fragment).MessageViewTypes = MessageTypes.SecuredMessagingInbox;
					break;
				case 1:
					fragment = new MessagesFragment();
					((MessagesFragment)fragment).MessageViewTypes = MessageTypes.SecuredMessagingNotifications;
					break;
				case 2:
					fragment = new MessagesFragment();
					((MessagesFragment)fragment).MessageViewTypes = MessageTypes.SecuredMessagingSent;
					break;
				case 3:
					fragment = new MessageComposeFragment();
				break;
			}

			if (fragment != null)
			{
				NavigationService.NavigatePush(fragment, true, false);
			}
		}
	}
}