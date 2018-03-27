using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Host.Pathways;
using SunBlock.DataTransferObjects.Messages.SecuredMessaging;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunBlock.DataTransferObjects.Mobile.Model.SecuredMessaging;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;
using SunMobile.Shared.Views;

namespace SunMobile.Shared.Methods
{
	public class MessagingMethods : SunBlockServiceBase
	{
        private static readonly string cultureViewId = "AA39BE92-CF96-4ABC-9DC7-FA559CF2E95E";

		public Task<StatusResponse<List<MessageThread>>> RetrieveMessageThreadByMember(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/RetrieveMessageThreadByMember";
			var response = PostToSunBlock<StatusResponse<List<MessageThread>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> InsertOrUpdateMessageThread(MessageThread request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/InsertOrUpdateMessageThread";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> SendMessage(ComposeThreadRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SendMessage";

			if (request != null && !string.IsNullOrEmpty(request.MessageCategory) && request.MessageCategory.Contains("Document / Image Request"))
			{
				var documentType = request.MessageCategory.Substring(27);
				request.MessageCategory = "Document / Image Request";
				request.MessageBody = $"[{documentType}]" + Environment.NewLine + request.MessageBody;
			}

			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> ReplyToThread(ReplyToThreadRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/ReplyToThread";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<CrmMessageResponse> GetUnreadMessageCounts(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetUnreadMessageCounts";
			var response = PostToSunBlock<CrmMessageResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<GetMessageSubjectsResponse> GetMessageSubjects(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetMessageSubjects";
			var response = PostToSunBlock<GetMessageSubjectsResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<NotificationSettingsModel>> GetPushNotificationSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetPushNotificationSettings";
			var response = PostToSunBlock<StatusResponse<NotificationSettingsModel>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}	

		public Task<StatusResponse> UpdatePushNotificationAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdatePushNotificationAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> UpdateSecurityAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdateSecurityAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> UpdateEDocumentAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdateEDocumentAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> UpdateAvailableBalanceAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdateAvailableBalanceAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> UpdateNsfAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdateNsfAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> UpdateDirectDepositAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdateDirectDepositAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> UpdatePaymentReminderAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UpdatePaymentReminderAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public async Task<GetMessageSubjectsResponse> GetMessageSubjectsAndDocumentTypes(object request, object view)
		{
			var response = await GetMessageSubjects(request, view);

			if (response?.Subjects != null)
			{
				response.Subjects.Remove("Document / Image Request");

				if (response?.DocumentTypes != null)
				{
					foreach (var documentType in response.DocumentTypes)
					{
						response.Subjects.Add($"Document / Image Request - {documentType}");
					}
				}
			}

			return response;
		}

		public async Task<List<MessageViewModel>> LoadAllSecuredMessages(MessageTypes messageType, object View)
		{
			var returnValue = new List<MessageViewModel>();

			try 
			{
				var methods = new MessagingMethods();
				var response = await methods.RetrieveMessageThreadByMember(null, View);

				foreach (var messageThread in response.Result)
				{
					foreach (var message in messageThread.Messages)
					{
						var messageViewModel = new MessageViewModel();

						bool addToModel = false;

						switch (messageType)
						{
							case MessageTypes.SecuredMessagingInbox:
								if (!message.SentByMember && messageThread.Category == "General")
								{
									addToModel = true;
								}
								break;
							case MessageTypes.SecuredMessagingNotifications:
								if (!message.SentByMember && messageThread.Category != "General")
								{
									addToModel = true;
								}
								break;
							case MessageTypes.SecuredMessagingSent:
								if (message.SentByMember)
								{
									addToModel = true;
								}
								break;
						}

						if (addToModel)
						{
							messageViewModel.Id = message.Id.ToString();
							messageViewModel.MessageType = MessageTypes.SecuredMessagingInbox;
							messageViewModel.Subject = messageThread.Subject;
							messageViewModel.Body = message.Body;
							messageViewModel.BodyStrippedOfHtml = StringUtilities.StringUtilities.StripHtmlTags(message.Body);
							messageViewModel.DateReceived = message.EntryDateUtc.UtcToEastern();
							messageViewModel.FriendlyDate = message.EntryDateUtc.GetFriendlyDate();
							messageViewModel.IsRead = message.ReadByMember || messageType == MessageTypes.SecuredMessagingSent;
							messageViewModel.Thread = messageThread;
							returnValue.Add(messageViewModel);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Logging.Log(ex, "MessagingMethods:LoadAllSecuredMessages");
			}

			return returnValue;
		}

		public List<MessageViewModel> LoadAlerts()
		{
			var returnValue = new List<MessageViewModel>();

			var alerts = RetainedSettings.Instance.Alerts;

			try
			{
				foreach (var alert in alerts)
				{
					var messageViewModel = new MessageViewModel();
					messageViewModel.Id = alert.Id;
					messageViewModel.MessageType = MessageTypes.AlertsInbox;
					messageViewModel.Subject = "Suncoast Credit Union Alert";
					messageViewModel.Body = alert.Message;
					messageViewModel.BodyStrippedOfHtml = alert.Message;
					messageViewModel.DateReceived = alert.ReceivedDate;
					messageViewModel.FriendlyDate = alert.ReceivedDate.GetFriendlyDate();
					messageViewModel.IsRead = alert.IsRead;
					returnValue.Add(messageViewModel);
				}
			}
			catch(Exception ex)
			{
				Logging.Logging.Log(ex, "MessagingMethods:LoadAlerts");
			}

			return returnValue;
		}

		public async Task MarkMessageAsRead(MessageViewModel messageViewModel, object View)
		{
			switch (messageViewModel.MessageType)
			{
				case MessageTypes.SecuredMessagingInbox:
				case MessageTypes.SecuredMessagingNotifications:
				case MessageTypes.SecuredMessagingSent:
					var methods = new MessagingMethods();
					var message = messageViewModel.Thread.Messages.Single(x => x.Id.ToString() == messageViewModel.Id);
					var messageIndex = messageViewModel.Thread.Messages.IndexOf(message);
					message.ReadByMember = true;
					messageViewModel.Thread.Messages[messageIndex] = message;
					await methods.InsertOrUpdateMessageThread(messageViewModel.Thread, View);
					break;
				case MessageTypes.AlertsInbox:
					RetainedSettings.Instance.MarkAlertAsRead(messageViewModel.Id);
					break;
			}
		}		

		public async Task<bool> TrashMessage(MessageViewModel messageViewModel, bool displayConfirmation, object view)
		{			
			var returnValue = false;

			#if __IOS__
			var nativeView = (UIKit.UIView)view;
			#endif

			#if __ANDROID__
			var nativeView = (Android.Content.Context)view;
			#endif

			string response = "Yes";

			if (displayConfirmation)
			{
                var confirmation = CultureTextProvider.GetMobileResourceText(cultureViewId, "F12AC20B-0923-4AFB-AB31-0A9D218A774B", "Confirmation");
                var body = CultureTextProvider.GetMobileResourceText(cultureViewId, "8C45B515-E6CC-4AC6-AADA-841E2E95ECC4", "Are you sure you want to delete this message?");
                response = await AlertMethods.Alert(nativeView, confirmation, body, CultureTextProvider.YES(), CultureTextProvider.NO());			
			}			

			if (response == "Yes")
			{				
				switch (messageViewModel.MessageType)
				{
					case MessageTypes.SecuredMessagingInbox:
					case MessageTypes.SecuredMessagingNotifications:
					case MessageTypes.SecuredMessagingSent:
						var methods = new MessagingMethods();
						var message = messageViewModel.Thread.Messages.Single(x => x.Id.ToString() == messageViewModel.Id);
						var messageIndex = messageViewModel.Thread.Messages.IndexOf(message);
						message.DeletedByMember = true;
						messageViewModel.Thread.Messages[messageIndex] = message;
						await methods.InsertOrUpdateMessageThread(messageViewModel.Thread, view);
						break;
					case MessageTypes.AlertsInbox:
						RetainedSettings.Instance.DeleteAlert(messageViewModel.Id);
						break;
				}

				returnValue = true;
			}

			return returnValue;
		}

		public List<ListViewItem> LoadAlertSettingsFromModel(AccountSpecificAlertModel model)
		{
			var listViewItems = new List<ListViewItem>();
			ListViewItem listViewItem;
			AlertSettingModel alertSettingModel;

			if (model.AvailableBalaceThresholdAlertSettings != null)
			{
				var availableBalanceThresholdAlertModel = (AvailableBalanceThresholdAlertModel)model.AvailableBalaceThresholdAlertSettings;
				listViewItem = new ListViewItem();
				listViewItem.Item1Text = availableBalanceThresholdAlertModel.DisplayText;
				listViewItem.Item2Text = availableBalanceThresholdAlertModel.Enabled ? "On" : "Off";
				listViewItem.Item3Text = "AvailableBalaceThresholdAlertSettings";
				listViewItem.IsChecked = availableBalanceThresholdAlertModel.Enabled;
				listViewItem.MoreIconVisible = true;
				listViewItem.Data = availableBalanceThresholdAlertModel;
				listViewItems.Add(listViewItem);
			}

			if (model.DirectDepositAlertSettings != null)
			{
				alertSettingModel = (AlertSettingModel)model.DirectDepositAlertSettings;
				listViewItem = new ListViewItem();
				listViewItem.Item1Text = alertSettingModel.DisplayText;
				listViewItem.Item2Text = alertSettingModel.Enabled ? "On" : "Off";
				listViewItem.Item3Text = "DirectDepositAlertSettings";
				listViewItem.IsChecked = alertSettingModel.Enabled;
				listViewItem.MoreIconVisible = false;
				listViewItem.Data = model.DirectDepositAlertSettings;
				listViewItems.Add(listViewItem);
			}

			if (model.NsfAlertSettings != null)
			{
				alertSettingModel = (AlertSettingModel)model.NsfAlertSettings;
				listViewItem = new ListViewItem();
				listViewItem.Item1Text = alertSettingModel.DisplayText;
				listViewItem.Item2Text = alertSettingModel.Enabled ? "On" : "Off";
				listViewItem.Item3Text = "NsfAlertSettings";
				listViewItem.IsChecked = alertSettingModel.Enabled;
				listViewItem.MoreIconVisible = false;
				listViewItem.Data = model.NsfAlertSettings;
				listViewItems.Add(listViewItem);
			}

			if (model.PaymentReminderAlertSettings != null)
			{
				alertSettingModel = (AlertSettingModel)model.PaymentReminderAlertSettings;
				listViewItem = new ListViewItem();
				listViewItem.Item1Text = alertSettingModel.DisplayText;
				listViewItem.Item2Text = alertSettingModel.Enabled ? "On" : "Off";
				listViewItem.Item3Text = "PaymentReminderAlertSettings";
				listViewItem.IsChecked = alertSettingModel.Enabled;
				listViewItem.MoreIconVisible = false;
				listViewItem.Data = model.PaymentReminderAlertSettings;
				listViewItems.Add(listViewItem);
			}

			return listViewItems;
		}

		public async Task SaveAlertSettings(List<AlertSetting> itemsChanged, AccountSpecificAlertModel model, object view, bool updateHost = true)
		{
			var methods = new MessagingMethods();

			foreach (var alertSetting in itemsChanged)
			{
				if (alertSetting.Description == "AvailableBalaceThresholdAlertSettings")
				{
					var request = new AvailableBalanceThresholdSettingsUpdateRequest 
					{
						Suffix = model.AccountId,
						AccountSettingType = model.AccountSettingType,
						Value = alertSetting.Value,
						ThresholdAmount = alertSetting.Amount
					};

					model.AvailableBalaceThresholdAlertSettings.Enabled = alertSetting.Value;

					if (updateHost) 
					{
						await methods.UpdateAvailableBalanceAlertSettings(request, view);
					}
				}

				if (alertSetting.Description == "DirectDepositAlertSettings")
				{
					var request = new AccountSpecificAlertSettingsUpdateRequest 
					{
						Suffix = model.AccountId,
						AccountSettingType = model.AccountSettingType,
						Value = alertSetting.Value
					};

					model.DirectDepositAlertSettings.Enabled = alertSetting.Value;

					if (updateHost) 
					{
						await methods.UpdateDirectDepositAlertSettings(request, view);
					}
				}

				if (alertSetting.Description == "NsfAlertSettings")
				{
					var request = new AccountSpecificAlertSettingsUpdateRequest 
					{
						Suffix = model.AccountId,
						AccountSettingType = model.AccountSettingType,
						Value = alertSetting.Value
					};

					model.NsfAlertSettings.Enabled = alertSetting.Value;

					if (updateHost) 
					{
						await methods.UpdateNsfAlertSettings(request, view);
					}
				}

				if (alertSetting.Description == "PaymentReminderAlertSettings")
				{
					var request = new AccountSpecificAlertSettingsUpdateRequest 
					{
						Suffix = model.AccountId,
						AccountSettingType = model.AccountSettingType,
						Value = alertSetting.Value
					};

					model.PaymentReminderAlertSettings.Enabled = alertSetting.Value;

					if (updateHost) 
					{
						await methods.UpdatePaymentReminderAlertSettings(request, view);
					}
				}
			}
		}
	}
}