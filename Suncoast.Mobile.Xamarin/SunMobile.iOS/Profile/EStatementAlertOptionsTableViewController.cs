using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.iOS.Common;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class EStatementAlertOptionsTableViewController : BaseTableViewController
	{
		private AlertSettings _alertSettings;
		private bool _enableChanged = false;

		public EStatementAlertOptionsTableViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var update = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "4f470296-f72c-47c9-86af-054ff8133cb7", "Update");
			var rightButton = new UIBarButtonItem(update, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => Update();

			// Hides the remaining rows.
			tableMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			switchEStatementAlerts.ValueChanged += (sender, e) =>
			{
				_enableChanged = true;
				Validate();
			};

			switchSendPushAlert.ValueChanged += (sender, e) =>
			{
				_enableChanged = true;
				Validate();
			};

			var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");
			var textMessage = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "49f62c66-802f-48d9-8220-b342f8116da7", "Text Message");

			var items = new List<string> { email, textMessage };

			CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtAlertMethod, items, (text) =>
			{
				var emailAddress = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "1fc6b9ff-64d5-40cc-b44c-6f3c5aafabc3", "Email Address");
				var phoneNumber = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "423794e8-1ed5-4c86-8778-5a60bf11740b", "Phone Number");
				lblAlertType.Text = text == email ? emailAddress : phoneNumber;
				txtAlertAddress.KeyboardType = text == AlertTypes.Email.ToString() ? UIKeyboardType.EmailAddress : UIKeyboardType.PhonePad;
				txtAlertAddress.Text = string.Empty;
				Validate();
			});

			txtAlertAddress.EditingChanged += (sender, e) =>
			{
				Validate();
			};

			txtAlertAddress.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;

				if (txtAlertMethod.Text == AlertTypes.Email.ToString())
				{
					return newLength <= 255;
				}
				else
				{
					return newLength <= 10 && (replacementString.IsNumeric() || replacementString == string.Empty);
				}
			};

			GetEDocumentAlertSettings();

			CheckEnabledStatus();			
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "178ee94e-7e62-4c82-9a54-2f5e7ebd0e5d", "Alert Settings");
			CultureTextProvider.SetMobileResourceText(lblEDocumentsAlertPrefNotify, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "d7a28196-c526-408d-9ca4-bb978be9f9ac", "Notify me when my eNotice or eStatement is available");
			CultureTextProvider.SetMobileResourceText(lblAlertType, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "1fc6b9ff-64d5-40cc-b44c-6f3c5aafabc3", "Email Address");
			CultureTextProvider.SetMobileResourceText(lblEDocumentsAlertPrefSendAnAlert, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "9ed58b25-1536-4df1-a1e9-1573ddf8d7f6", "Send an alert to my device");
		}

		private async void GetEDocumentAlertSettings()
		{
			ShowActivityIndicator();

			var methods = new DocumentMethods();
			var response = await methods.GetEDocumentAlertSettings(null, View);

			var messagingMethods = new MessagingMethods();
			var pushSettingsResponse = await messagingMethods.GetPushNotificationSettings(null, View);

			HideActivityIndicator();

			if (response != null && response.Success && response.Result != null)
			{
				_alertSettings = response.Result;
				switchEStatementAlerts.On = _alertSettings.AlertEnabled;
				txtAlertAddress.Text = _alertSettings.AlertEmail;
				var emailAddress = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "1fc6b9ff-64d5-40cc-b44c-6f3c5aafabc3", "Email Address");
				var phoneNumber = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "423794e8-1ed5-4c86-8778-5a60bf11740b", "Phone Number");
				var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");
				var textMessage = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "49f62c66-802f-48d9-8220-b342f8116da7", "Text Message");
				txtAlertMethod.Text = _alertSettings.AlertType == AlertTypes.Email.ToString() ? email : textMessage;
				lblAlertType.Text = _alertSettings.AlertType == AlertTypes.Email.ToString() ? emailAddress : phoneNumber;
				txtAlertAddress.KeyboardType = _alertSettings.AlertType == AlertTypes.Email.ToString() ? UIKeyboardType.EmailAddress : UIKeyboardType.PhonePad;
			}

			if (pushSettingsResponse?.Result?.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings != null)
			{
				if (pushSettingsResponse?.Result?.EDocumentAlertsSetting != null)
				{
					switchSendPushAlert.On = pushSettingsResponse.Result.EDocumentAlertsSetting.Enabled;
				}
			}

			Validate();
		}

		private void Validate()
		{
			bool validated = false;

			if (switchEStatementAlerts.On)
			{
				var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");

				if (txtAlertMethod.Text == email)
				{
					validated = StringUtilities.IsValidEmail(txtAlertAddress.Text);
				}
				else
				{
					validated = StringUtilities.IsValidPhone(txtAlertAddress.Text);
				}
			}
			else if (_enableChanged)
			{
				validated = true;
			}

			NavigationItem.RightBarButtonItem.Enabled = validated;

			CheckEnabledStatus();
		}

		private async void Update()
		{
			var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");

			var request = new EDocumentSetAlertRequest
			{
				AlertEnabled = switchEStatementAlerts.On,
				AlertType = txtAlertMethod.Text == email ? AlertTypes.Email.ToString() : AlertTypes.Sms.ToString(),
				EmailAddress = txtAlertAddress.Text
			};

			ShowActivityIndicator();

			var methods = new DocumentMethods();
			var response = await methods.SetEDocumentAlertSettings(request, View);
			var messagingMethods = new MessagingMethods();
			var messagingResponse = await messagingMethods.UpdateEDocumentAlertSettings(switchSendPushAlert.On, View);

			HideActivityIndicator();

			if (response != null && response.Success && messagingResponse != null && messagingResponse.Success)
			{
				NavigationController.PopViewController(true);
			}
			else
			{
				var errorText = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "09968389-0119-41f8-b411-ffeb05af2483", "Unable to update alert settings.");
				var ok = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "580eb21e-0104-44d2-ae36-ab6bf95a6cb8", "OK");
				await AlertMethods.Alert(View, "SunMobile", errorText, ok);
			}
		}

		private void CheckEnabledStatus()
		{
			txtAlertMethod.Enabled = txtAlertAddress.Enabled = switchEStatementAlerts.On;
		}
	}
}