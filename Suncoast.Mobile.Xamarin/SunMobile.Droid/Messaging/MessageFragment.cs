using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.Droid.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.Dates;

namespace SunMobile.Droid.Messaging
{
	public class MessageFragment : BaseFragment
	{
		public MessageViewModel Message { get; set;}
		private TextView lblFrom;
		private TextView lblDateRecieved;
		private WebView webViewMessage;
		private ImageView btnCompose;
		private ImageView btnReply;
		private ImageView btnTrash;
		private Button btnDocumentCenter;

        private static readonly string cultureViewId = "EB8FA72C-AFF4-440A-83F7-B2E676B53C0A";

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.MessageDetailView, null);

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = JsonConvert.SerializeObject(Message);
			outState.PutString("Message", json);

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("Message");
				Message = JsonConvert.DeserializeObject<MessageViewModel>(json);
			}

			lblFrom = Activity.FindViewById<TextView>(Resource.Id.lblFrom);
			webViewMessage = Activity.FindViewById<WebView>(Resource.Id.webViewMessageBody);
			lblDateRecieved = Activity.FindViewById<TextView>(Resource.Id.lblReceivedTime);

			btnCompose = Activity.FindViewById<ImageView>(Resource.Id.btnCompose);
			btnCompose.Click += (sender, e) => ComposeMessage();
			btnReply = Activity.FindViewById<ImageView>(Resource.Id.btnReply);
			btnReply.Click += (sender, e) => ReplyMessage();
			btnTrash = Activity.FindViewById<ImageView>(Resource.Id.btnTrash);
			btnTrash.Click += (sender, e) => TrashMessage();
			btnDocumentCenter = Activity.FindViewById<Button>(Resource.Id.btnDocumentCenter);
			btnDocumentCenter.Click += (sender, e) => GotoDocumentCenter();

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
			else
			{
				btnDocumentCenter.Visibility = ViewStates.Gone;
			}

			// Don't let them reply to notifications.
			if (Message.MessageType == MessageTypes.SecuredMessagingNotifications)
			{
				btnReply.Visibility = ViewStates.Gone;
			}

			if (Message.MessageType == MessageTypes.AlertsInbox)
			{
				var layoutParams = (RelativeLayout.LayoutParams)btnTrash.LayoutParameters;
				layoutParams.AddRule(LayoutRules.AlignParentRight);
				layoutParams.RightMargin = 20;
				btnTrash.LayoutParameters = layoutParams;

				btnCompose.Visibility = ViewStates.Gone;
				btnReply.Visibility = ViewStates.Gone;
			}

			lblFrom.Text = Message.Subject;
			lblDateRecieved.Text = Message.DateReceived.GetFriendlyDateTime();

			// Set default font.
			var htmlString = string.Format("<span style=\"font-family: HelveticaNeue; font-size: 14\">{0}</span>", Message.Body.Replace("“", "\"").Replace("”", "\""));
			webViewMessage.LoadData(htmlString, "text/html", "UTF-8");

			MarkMessageAsRead();
		}

		private async void MarkMessageAsRead()
		{
			if (!Message.IsRead)
			{
				try
				{
					var methods = new MessagingMethods();
					await methods.MarkMessageAsRead(Message, Activity);
					((MessagesFragment)Activity.SupportFragmentManager.FindFragmentByTag("MessagesFragment")).RefreshMessageList = true;
				}
				catch (Exception ex)
				{
					Logging.Log(ex, "MessageFragment:MarkMessageAsRead");
				}
			}
		}

		private void ComposeMessage()
		{
			var composeFragment = new MessageComposeFragment();

			NavigationService.NavigatePush(composeFragment, true, false);
		}

		private void ReplyMessage()
		{
			var composeFragment = new MessageComposeFragment();
			composeFragment.Thread = Message.Thread;

			NavigationService.NavigatePush(composeFragment, true, false);
		}

		private async void TrashMessage()
		{
			try
			{
				var methods = new MessagingMethods();
				var response = await methods.TrashMessage(Message, true, Activity);

				if (response)
				{
					var messagesFragment = ((MessagesFragment)Activity.SupportFragmentManager.FindFragmentByTag("MessagesFragment"));

					if (messagesFragment != null)
					{
						messagesFragment.RefreshMessageList = true;
					}

					Activity.SupportFragmentManager.PopBackStack();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MessageFragment:TrashMesage");
			}
		}

		private async void GotoDocumentCenter()
		{
            var viewDocuments = CultureTextProvider.GetMobileResourceText(cultureViewId, "873D46C9-C07C-42A1-96A7-A46CC16E3A88", "View Documents");
            if (btnDocumentCenter.Text == viewDocuments)
			{
				var request = new DocumentDownloadQueryRequest
				{
					CaseId = Message.Thread.ConversationNo
				};

				var methods = new DocumentMethods();

                ShowActivityIndicator();

				var response = await methods.QueryDownloadDocuments(request, Activity);

				HideActivityIndicator();

				var documentCenterFragment = new DocumentCenterFragment();
				documentCenterFragment.ShowDownloads = true;

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

					documentCenterFragment.Documents = documents;
					NavigationService.NavigatePush(documentCenterFragment, false, true);
				}
			}
			else
			{
				var documentCenterFragment = new DocumentCenterFragment();
				NavigationService.NavigatePush(documentCenterFragment, false, true);
			}

		}
	}
}