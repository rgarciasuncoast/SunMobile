using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Host.Pathways;
using SunBlock.DataTransferObjects.Messages.SecuredMessaging;
using SunBlock.DataTransferObjects.Mobile.Model.SecuredMessaging;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid.Messaging
{
	public class MessageComposeFragment : BaseFragment
	{
		public MessageThread Thread { get; set; }
		private ImageView btnSend;
		private Spinner spinnerSubject;
		private EditText txtBody;
		private GetMessageSubjectsResponse _messageSubjects;
		private TextView lblSubject;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.MessageComposeView, null);

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			string json;

			if (Thread != null)
			{
				json = JsonConvert.SerializeObject(Thread);
				outState.PutString("Thread", json);
			}

			json = JsonConvert.SerializeObject(_messageSubjects);
			outState.PutString("MessageSubjects", json);
			outState.PutInt("SpinnerSelection", spinnerSubject.SelectedItemPosition);
			outState.PutString("Body", txtBody.Text);

			base.OnSaveInstanceState (outState);
		}

		public override void SetCultureConfiguration()
		{
			txtBody.Hint = CultureTextProvider.GetMobileResourceText("6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "39937220-F47F-4721-A9DC-0DDA932B4BF8", "Compose Message");
			CultureTextProvider.SetMobileResourceText(lblSubject, "6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "C51146A3-FF92-4E75-847D-665B7CE41E7D", "Subject");
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			btnSend = Activity.FindViewById<ImageView>(Resource.Id.btnSend);
			btnSend.Click += (sender, e) => SendMessage();
			btnSend.Enabled = false;
			spinnerSubject = Activity.FindViewById<Spinner>(Resource.Id.spnrSubject);
			spinnerSubject.ItemSelected += (sender, e) => Validate();
			txtBody = Activity.FindViewById<EditText>(Resource.Id.txtMessageBody);
			lblSubject = Activity.FindViewById<TextView>(Resource.Id.lblSubject);

			var windowManager = Context.GetSystemService(Android.Content.Context.WindowService).JavaCast<IWindowManager>();
			var display = windowManager.DefaultDisplay;
			var size = new Point();
			display.GetSize(size);
			var width = Math.Min((int)(size.X * .9), 800);
			spinnerSubject.DropDownWidth = width;

			ClearAll();

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("Thread");

				if (json != "" && json != "[]")
				{
					Thread = JsonConvert.DeserializeObject<MessageThread>(json);
				}

				txtBody.Text = savedInstanceState.GetString("Body");
				json = savedInstanceState.GetString("MessageSubjects");
				_messageSubjects = JsonConvert.DeserializeObject<GetMessageSubjectsResponse>(json);
			}

			if (Thread == null)
			{
				((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "4FF56702-17BD-4EEE-80B8-23D03EC3F3A7", "New Message"));

				if (savedInstanceState != null)
				{
					var selection = savedInstanceState.GetInt("SpinnerSelection", 0);
					GetMessageSubjects(selection);
				}
				else
				{
					GetMessageSubjects(0);
				}
			}
			else
			{
				((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "8F119C5F-BC6E-4E5F-8BE6-50C5E021BBC3", "Reply"));
				var subjects = new List<string>();
				subjects.Add(Thread.Category);
				var adapter = new ArrayAdapter<string>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, subjects);
				spinnerSubject.Adapter = adapter;
				txtBody.TextChanged += (sender, e) => Validate();
			}			
		}

		private void ClearAll()
		{
			txtBody.Text = string.Empty;
		}

		private async void GetMessageSubjects(int selection)
		{
			ShowActivityIndicator();

			if (_messageSubjects == null)
			{
				var methods = new MessagingMethods();
				_messageSubjects = await methods.GetMessageSubjectsAndDocumentTypes(null, Activity);
			}

			HideActivityIndicator();

			if (_messageSubjects?.Subjects != null)
			{
				var adapter = new ArrayAdapter<string>(Activity, Resource.Layout.spinner_item, _messageSubjects.Subjects);
				spinnerSubject.Adapter = adapter;
				spinnerSubject.SetSelection(selection);
				txtBody.TextChanged += (sender, e) => Validate();
			}
		}

		private void Validate()
		{
			try
			{
				btnSend.Enabled = !string.IsNullOrEmpty(txtBody.Text) && !string.IsNullOrEmpty(spinnerSubject.SelectedItem.ToString());
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MessageComposeFragment:Validate");
			}
		}

		private async void SendMessage()
		{
			var methods = new MessagingMethods();
			StatusResponse response;

			ShowActivityIndicator();

			try
			{
				if (Thread == null)
				{
					var composeThreadRequest = new ComposeThreadRequest();
					composeThreadRequest.MessageCategory = spinnerSubject.SelectedItem.ToString();
					composeThreadRequest.MessageBody = txtBody.Text;
					response = await methods.SendMessage(composeThreadRequest, Activity);	 	
				}
				else
				{
					var replyToThreadRequest = new ReplyToThreadRequest();
					replyToThreadRequest.MessageTypeId = Thread.MessageTypeId;
					replyToThreadRequest.MessageBody = txtBody.Text;
					response = await methods.ReplyToThread(replyToThreadRequest, Activity);
				}

				var ok = CultureTextProvider.GetMobileResourceText("6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "6AEED6E5-45F0-4B01-B9CA-12C5FD02DF07", "OK");

				if (response != null && response.Success)
				{
					await AlertMethods.Alert(Activity, "SunMobile", CultureTextProvider.GetMobileResourceText("6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "4E17BC02-D68D-4C82-85F3-AB76529EDFE1", "Your message was successfully sent."), ok);

					Activity.SupportFragmentManager.PopBackStack();
				}
				else if (response != null && !response.Success && !string.IsNullOrEmpty(response.FailureMessage))
				{
					await AlertMethods.Alert(Activity, "SunMobile", response.FailureMessage, ok);
				}
				else
				{
					await AlertMethods.Alert(Activity, "SunMobile", CultureTextProvider.GetMobileResourceText("6A5F17AC-7894-4947-B7D7-AEF5B1C4224E", "FE504B74-F5D9-4F19-A229-9415A594E364", "Error sending message."), ok);
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "MessageComposeFragment:SendMessage");
			}

			HideActivityIndicator();
		}
	}
}