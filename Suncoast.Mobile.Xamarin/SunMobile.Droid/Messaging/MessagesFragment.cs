using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Messaging
{
	public class MessagesFragment : BaseListFragment
	{
		public MessageTypes MessageViewTypes { get; set; }
		public bool RefreshMessageList { get; set; }
		private SwipeRefreshLayout _refreshControl;
		private MessagesListAdapter _messagesListAdapter;
		private List<MessageViewModel> _viewModel;

		private ImageView btnCompose;
		private RelativeLayout actionBarLayout;

        private static readonly string cultureViewId = "85EC5976-F0EE-4E27-A6E3-8238A00C1417";

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.MessageListView, null);		

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = JsonConvert.SerializeObject(MessageViewTypes);
			outState.PutString("MessageViewTypes", json);
			outState.PutBoolean("RefreshMessageList", RefreshMessageList);
			json = JsonConvert.SerializeObject(_viewModel);
			outState.PutString("ViewModel", json);

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			_refreshControl = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.alertRefresher);
			_refreshControl.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			_refreshControl.Refresh += (sender, e) => Refresh();

			actionBarLayout = Activity.FindViewById<RelativeLayout>(Resource.Id.actionBar);

			btnCompose = Activity.FindViewById<ImageView>(Resource.Id.btnCompose);
			btnCompose.Click += (sender, e) => ComposeMessage();

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("MessageViewTypes");
				MessageViewTypes = JsonConvert.DeserializeObject<MessageTypes>(json);
				RefreshMessageList = savedInstanceState.GetBoolean("RefreshMessageList");
				json = savedInstanceState.GetString("ViewModel");
				_viewModel = JsonConvert.DeserializeObject<List<MessageViewModel>>(json);
			}

			if (MessageViewTypes == MessageTypes.AlertsInbox)
			{
				actionBarLayout.Visibility = ViewStates.Gone;
			}

			ListViewMain.ItemClick += (sender, e) =>
			{
				var messageFragment = new MessageFragment();
				var message = _messagesListAdapter.GetListViewItem(e.Position);
				message.MessageType = MessageViewTypes;
				messageFragment.Message = message;

				NavigationService.NavigatePush(messageFragment, true, false);		
			};

			LoadMessages();
		}

		public override void OnResume()
		{
			base.OnResume();

			if (_viewModel != null && RefreshMessageList)
			{
				_viewModel = null;
				RefreshMessageList = false;
				LoadMessages();
			}
		}

		private void ComposeMessage()
		{
			var composeFragment = new MessageComposeFragment();
			NavigationService.NavigatePop(false);
			NavigationService.NavigatePush(composeFragment, true, false);
		}

		private void Refresh()
		{
			_viewModel = null;

			LoadMessages();
		}

		private void SetTitle(MessageTypes messageType)
		{
            try
            {
                var typeString = "";
                switch (messageType)
                {
                    case MessageTypes.SecuredMessagingInbox:
                        typeString = CultureTextProvider.GetMobileResourceText(cultureViewId, "C5C0C914-D96D-4034-B0BF-E6E94AF41D67", "Inbox");
                        break;
                    case MessageTypes.SecuredMessagingNotifications:
                        typeString = CultureTextProvider.GetMobileResourceText(cultureViewId, "F5A70C19-4AE8-4C0C-9E04-05EA3734F417", "Notifications");
                        break;
                    case MessageTypes.SecuredMessagingSent:
                        typeString = CultureTextProvider.GetMobileResourceText(cultureViewId, "DA87760B-FC5D-4985-AA40-0E9E5F09E7F8", "Sent");
                        break;
                    case MessageTypes.AlertsInbox:
                        typeString = CultureTextProvider.GetMobileResourceText(cultureViewId, "C5C0C914-D96D-4034-B0BF-E6E94AF41D67", "Inbox");
                        break;
                }

                ((MainActivity)Activity).SetActionBarTitle(typeString);
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "MessagesFragment:SetTitle");
            }
		}

		private async void LoadMessages()
		{
			if (_viewModel == null) 
			{
				if (!_refreshControl.Refreshing)
				{
                    ShowActivityIndicator();
				}

				var methods = new MessagingMethods();

				switch (MessageViewTypes)
				{
					case MessageTypes.SecuredMessagingInbox:
						_viewModel = await methods.LoadAllSecuredMessages(MessageTypes.SecuredMessagingInbox, Activity);
						break;
					case MessageTypes.SecuredMessagingNotifications:
						_viewModel = await methods.LoadAllSecuredMessages(MessageTypes.SecuredMessagingNotifications, Activity);
						break;
					case MessageTypes.SecuredMessagingSent:
						_viewModel = await methods.LoadAllSecuredMessages(MessageTypes.SecuredMessagingSent, Activity);
						break;
					case MessageTypes.AlertsInbox:
						_viewModel = methods.LoadAlerts();
						break;
				}

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.Refreshing = false;
				}
			}

			if (_viewModel != null && _viewModel.Count > 0)
			{				
				_messagesListAdapter = new MessagesListAdapter(Activity, Resource.Layout.MessageListViewItem, _viewModel, Resource.Id.lblTimeStamp, Resource.Id.lblMessageBody, Resource.Id.lblMessageSubject);
				ListAdapter = _messagesListAdapter;
			}

			SetTitle(MessageViewTypes);
		}
	}
}