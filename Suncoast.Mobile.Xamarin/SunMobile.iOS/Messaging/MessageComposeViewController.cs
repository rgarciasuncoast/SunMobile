using System;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Messages.SecuredMessaging;
using SunBlock.DataTransferObjects.Mobile.Model.SecuredMessaging;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Messaging
{
	public partial class MessageComposeViewController : BaseViewController
	{
		public MessageThread Thread { get; set; }
		public event Action<bool> MessagesChanged = delegate{};

		public MessageComposeViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var newMessage = CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "4ff56702-17bd-4eee-80b8-23d03ec3f3a7", "New Message");
			var reply = CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "8f119c5f-bc6e-4e5f-8be6-50c5e021bbc3", "Reply");
			Title = Thread == null ? newMessage : reply;

			var send = CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "81e46c0c-8725-4056-8618-6b62d7774111", "Send");
			var rightButton = new UIBarButtonItem(send, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => SendMessage();

			txtSubject.Enabled = Thread == null;
			txtBody.Changed += (sender, e) => Validate();

			ClearAll();

			CommonMethods.CreateTextViewWithPlaceHolder(txtBody, CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "39937220-f47f-4721-a9dc-0dda932b4bf8", "Compose Message"));

			GetMessageSubjects();			
		}

		public override void SetCultureConfiguration()
		{
			CultureTextProvider.SetMobileResourceText(lblComposeMessageSubject, "6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "c51146a3-ff92-4e75-847d-665b7ce41e7d", "Subject");
		}

		private void ClearAll()
		{
			txtSubject.Text = string.Empty;
			txtBody.Text = string.Empty;
		}

		private async void GetMessageSubjects()
		{
			ShowActivityIndicator();

			var methods = new MessagingMethods();
			var response = await methods.GetMessageSubjectsAndDocumentTypes(null, View);

			HideActivityIndicator();

			if (response?.Subjects != null)
			{
				CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtSubject, response.Subjects, (obj) =>
				{
					Validate();
				}, 14f);
			}

			if (Thread?.Subject != null)
			{
				txtSubject.Text = Thread.Subject;
			}
		}

		private void Validate()
		{
			NavigationItem.RightBarButtonItem.Enabled = !string.IsNullOrEmpty(txtSubject.Text) && !string.IsNullOrEmpty(txtBody.Text) && txtBody.Text != "Compose message";
		}

		private async void SendMessage()
		{
			var methods = new MessagingMethods();
			StatusResponse response;

			ShowActivityIndicator();

			if (Thread == null)
			{
				var composeThreadRequest = new ComposeThreadRequest();
				composeThreadRequest.MessageCategory = txtSubject.Text;
				composeThreadRequest.MessageBody = txtBody.Text;
				response = await methods.SendMessage(composeThreadRequest, View);	 	
			}
			else
			{
				var replyToThreadRequest = new ReplyToThreadRequest();
				replyToThreadRequest.MessageTypeId = Thread.MessageTypeId;
				replyToThreadRequest.MessageBody = txtBody.Text;
				response = await methods.ReplyToThread(replyToThreadRequest, View);
			}

			HideActivityIndicator();

			var ok = CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "6aeed6e5-45f0-4b01-b9ca-12c5fd02df07", "OK");


			if (response != null && response.Success)
			{
				await AlertMethods.Alert(View, "SunMobile", CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "4e17bc02-d68d-4c82-85f3-ab76529edfe1", "Your message was successfully sent."), ok);

				if (MessagesChanged != null)
				{
					MessagesChanged(true);
				}

				NavigationController.PopViewController(true);
			}
			else if (response != null && !response.Success && !string.IsNullOrEmpty(response.FailureMessage))
			{
				await AlertMethods.Alert(View, "SunMobile", response.FailureMessage, ok);
			}
			else
			{
				await AlertMethods.Alert(View, "SunMobile", CultureTextProvider.GetMobileResourceText("6a5f17ac-7894-4947-b7d7-aef5b1c4224e", "fe504b74-f5d9-4f19-a229-9415a594e364", "Error sending message."), ok);
			}
		}
	}
}