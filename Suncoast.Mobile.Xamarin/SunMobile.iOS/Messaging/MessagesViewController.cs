using System;
using System.Collections.Generic;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Messaging
{
	partial class MessagesViewController : BaseViewController
	{
		public MessageTypes MessageViewTypes { get; set; }
		public event Action<bool> MessageCountsChanged = delegate{};
		private UIRefreshControl _refreshControl;
		private List<MessageViewModel> _viewModel;
		private bool _refreshMessageCounts;

        private static readonly string cultureViewId = "85EC5976-F0EE-4E27-A6E3-8238A00C1417";

		public MessagesViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_refreshControl = new UIRefreshControl();
			tableViewMain.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += (sender, e) => Refresh();
			btnCompose.Clicked += (sender, e) => ComposeMessage();

			LoadMessages();

			CommonMethods.AddBottomToolbar(this);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (IsMovingFromParentViewController || IsBeingDismissed) 
			{
				if (_refreshMessageCounts)
				{
					MessageCountsChanged(true);
				}
			}
		}

		private void ComposeMessage()
		{	
			var messageComposeViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageComposeViewController") as MessageComposeViewController;
			messageComposeViewController.Thread = null;
			messageComposeViewController.MessagesChanged += obj =>
			{
				if (MessageViewTypes == MessageTypes.SecuredMessagingSent)
				{
					Refresh();
				}
			};

			NavigationController.PushViewController(messageComposeViewController, true);
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

				Title = typeString;
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MessagesViewController:SetTitle");
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
						_viewModel = await methods.LoadAllSecuredMessages(MessageTypes.SecuredMessagingInbox, View);
						break;
					case MessageTypes.SecuredMessagingNotifications:
						_viewModel = await methods.LoadAllSecuredMessages(MessageTypes.SecuredMessagingNotifications, View);
						break;
					case MessageTypes.SecuredMessagingSent:
						_viewModel = await methods.LoadAllSecuredMessages(MessageTypes.SecuredMessagingSent, View);
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
					_refreshControl.EndRefreshing();
				}
			}

			if (_viewModel != null && _viewModel.Count > 0)
			{
				var tableViewSource = new MessagesTableViewSource(_viewModel);

				tableViewSource.ItemSelected += item =>
				{
					var controller = AppDelegate.StoryBoard.InstantiateViewController("MessageViewController") as MessageViewController;
					item.MessageType = MessageViewTypes;
					controller.Message = item;

					controller.MessagesChanged += obj =>
					{
						Refresh();
						_refreshMessageCounts = true;
					};

					NavigationController.PushViewController(controller, true);
				};

				tableViewSource.ItemDeleted += Trash;

				tableViewMain.Source = tableViewSource;
				tableViewMain.ReloadData();
			}

            SetTitle(MessageViewTypes);
		}

		private async void Trash(MessageViewModel item)
		{
			var methods = new MessagingMethods();
			await methods.TrashMessage(item, false, View);
			_refreshMessageCounts = true;
		}
	}
}