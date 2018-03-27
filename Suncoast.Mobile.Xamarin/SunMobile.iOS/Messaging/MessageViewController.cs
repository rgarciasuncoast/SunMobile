using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.iOS.Common;
using SunMobile.iOS.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.Dates;
using UIKit;

namespace SunMobile.iOS.Messaging
{
	partial class MessageViewController : BaseViewController
	{
		public event Action<bool> MessagesChanged = delegate{};
		public MessageViewModel Message { get; set; }
		private bool _refreshMessageList;

        private static readonly string cultureViewId = "EB8FA72C-AFF4-440A-83F7-B2E676B53C0A";

		public MessageViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			btnCompose.Clicked += (sender, e) => ComposeMessage();
			btnReply.Clicked += (sender, e) =>	ReplyToMessage();				
			btnTrash.Clicked += (sender, e) => Trash();
			btnDocumentCenter.Clicked += (sender, e) => GotoDocumentCenter();

			if (Message.Thread.Messages.FindAll(x => x.DocumentUploadAssociated).Count > 0)
			{
				if (Message.Body.ToLower().Contains("documents have uploaded successfully") || Message.Body.ToLower().Contains("documents have been accepted"))
				{
					CultureTextProvider.SetMobileResourceText(btnDocumentCenter, cultureViewId, "873D46C9-C07C-42A1-96A7-A46CC16E3A88", "View Documents");
				}
				else
				{
					CultureTextProvider.SetMobileResourceText(btnDocumentCenter, cultureViewId, "D870915A-383E-4990-8E1E-DDEB9C1C9B42", "Manage Documents");
				}
			}
			else if (Message.Thread.Messages.FindAll(x => x.DocumentDownloadAssociated).Count > 0)
			{
				CultureTextProvider.SetMobileResourceText(btnDocumentCenter, cultureViewId, "873D46C9-C07C-42A1-96A7-A46CC16E3A88", "View Documents");
			}

			lblSubject.Text = Message.Subject;
			lblDateReceived.Text = Message.DateReceived.GetFriendlyDateTime();

			// Set default font.
			var htmlString = string.Format("<span style=\"font-family: HelveticaNeue; font-size: 14\">{0}</span>", Message.Body);
			webViewMessage.LoadHtmlString(htmlString, null);
		
			MarkMessageAsRead();

			SetToolbarItems();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (IsMovingFromParentViewController || IsBeingDismissed) 
			{
				if (_refreshMessageList)
				{
					MessagesChanged(true);
				}
			}
		}

		private void SetToolbarItems()
		{
			var toolbarButtons = toolbarMain.Items;

			UIBarButtonItem[] newtoolbarButtons;
			UIBarButtonItem composeButton = null;
			UIBarButtonItem fixedSpace = null;
			UIBarButtonItem fixedSpace2 = null;
			UIBarButtonItem replyButton = null;
			UIBarButtonItem trashButton = null;
			UIBarButtonItem documentCenterButton = null;
			UIBarButtonItem flexibleSpace = null;

			foreach (var button in toolbarButtons)
			{
				if (button.Tag == 1000)
				{
					composeButton = button;
				}

				if (button.Tag == 2000)
				{
					fixedSpace = button;
				}

				if (button.Tag == 3000)
				{
					replyButton = button;
				}

				if (button.Tag == 4000)
				{
					fixedSpace2 = button;
				}

				if (button.Tag == 5000)
				{
					trashButton = button;
				}

				if (button.Tag == 7000)
				{
					documentCenterButton = button;
				}

				if (button.Tag == 6000)
				{
					flexibleSpace = button;
				}
			}

			// Remove the reply button for alerts and notifications.
			if (Message.MessageType == MessageTypes.AlertsInbox || Message.MessageType == MessageTypes.SecuredMessagingNotifications)
			{
				if (Message.Thread.Messages.FindAll(x => x.DocumentUploadAssociated).Count > 0 || Message.Thread.Messages.FindAll(x => x.DocumentDownloadAssociated).Count > 0)
				{
					newtoolbarButtons = new[] { documentCenterButton, flexibleSpace, trashButton, fixedSpace, composeButton };
				}
				else
				{
					newtoolbarButtons = new[] { flexibleSpace, trashButton, fixedSpace, composeButton };
				}

				toolbarMain.SetItems(newtoolbarButtons, false);
			}
			else if (Message.Thread.Messages.FindAll(x => x.DocumentUploadAssociated).Count <= 0 && Message.Thread.Messages.FindAll(x => x.DocumentDownloadAssociated).Count <= 0)
			{
				newtoolbarButtons = new[] { flexibleSpace, trashButton, fixedSpace, replyButton, fixedSpace2, composeButton };
				toolbarMain.SetItems(newtoolbarButtons, false);
			}
		}

		private async void MarkMessageAsRead()
		{
			if (!Message.IsRead)
			{				
				_refreshMessageList = true;
				var methods = new MessagingMethods();
				await methods.MarkMessageAsRead(Message, View);
			}
		}

		private void ComposeMessage()
		{
			var messageComposeViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageComposeViewController") as MessageComposeViewController;
			messageComposeViewController.Thread = null;
			messageComposeViewController.MessagesChanged += obj =>
			{
				if (Message.MessageType == MessageTypes.SecuredMessagingSent)
				{
					_refreshMessageList = true;
				}
			};

			NavigationController.PushViewController(messageComposeViewController, true);
		}

		private void ReplyToMessage()
		{
			var messageComposeViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageComposeViewController") as MessageComposeViewController;
			messageComposeViewController.Thread = Message.Thread;
			messageComposeViewController.MessagesChanged += obj =>
			{
				if (Message.MessageType == MessageTypes.SecuredMessagingSent)
				{
					_refreshMessageList = true;
				}
			};

			NavigationController.PushViewController(messageComposeViewController, true);
		}

		private async void Trash()
		{	
			var methods = new MessagingMethods();
			var response = await methods.TrashMessage(Message, true, View);

			if (response)
			{				
				_refreshMessageList = true;
				NavigationController.PopViewController(true);
			}
		}

		private async void GotoDocumentCenter()
		{
            var viewDocuments = CultureTextProvider.GetMobileResourceText(cultureViewId, "873D46C9-C07C-42A1-96A7-A46CC16E3A88", "View Documents");
			if (btnDocumentCenter.Title == viewDocuments)
			{
				var request = new DocumentDownloadQueryRequest
				{
					CaseId = Message.Thread.ConversationNo
				};

				var methods = new DocumentMethods();

				ShowActivityIndicator();

				var response = await methods.QueryDownloadDocuments(request, View);

				HideActivityIndicator();

				var documentCenterViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentCenterViewController") as DocumentCenterViewController;
				documentCenterViewController.ShowDownloads = Message.Thread.Messages.FindAll(x => x.DocumentDownloadAssociated).Count > 0;
				NavigationService.NavigatePush(NavigationController, documentCenterViewController, true, true);

				if (response != null && response.Success)
				{
					var documents = new List<DocumentCenterFile>();

					if (response.Result.Count > 0)
					{
						foreach (var document in response.Result)
						{
							documents.Add(document.File);
						}
					}
					else
					{
						var documentCenterFile = new DocumentCenterFile();
						documentCenterFile.FileId = "0"; // Bogus file id.
						documents.Add(documentCenterFile);
					}

					var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
					documentViewerViewController.Files = documents;
					NavigationController.PushViewController(documentViewerViewController, true);
				}
			}
			else
			{
				var documentCenterViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentCenterViewController") as DocumentCenterViewController;
				NavigationService.NavigatePush(NavigationController, documentCenterViewController, true, true);
			}
		}
	}
}