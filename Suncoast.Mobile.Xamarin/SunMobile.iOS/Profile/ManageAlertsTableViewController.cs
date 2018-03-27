using System;
using Foundation;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class ManageAlertsTableViewController : BaseTableViewController
	{
		StatusResponse<NotificationSettingsModel> _model;

		public ManageAlertsTableViewController(IntPtr handle) : base(handle)
		{			
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			switchAlerts.ValueChanged += (sender, e) =>	EnableAlerts(switchAlerts.On);
			switchSecurityAlerts.ValueChanged += (sender, e) => EnableSecurityAlerts(switchSecurityAlerts.On);

			GetPushNotificationSettings();
		}

		public override void SetCultureConfiguration()
		{
            Title = CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "60681413-42D0-45C0-8318-6EDB1BCB3678", "Manage Alert Settings");
			CultureTextProvider.SetMobileResourceText(lblManageAlertSettingsSecurityAlerts, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "C38700E3-CD85-49DF-ACD0-F66FBB788835", "Security Alerts");
			CultureTextProvider.SetMobileResourceText(lblManageAlertSettingsElectronicDocumentAlertPrefs, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "46AA19BF-64F6-4DF6-A5C9-4BD28DE68D67", "Electronic Document Alert Preferences");
			CultureTextProvider.SetMobileResourceText(lblManageAlertSettingsAccountSpecificAlerts, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "FACF6F3B-FB66-4971-9DF1-B566664CC41F", "Account Specific Alerts");
			tableViewMenu.GetHeaderView(1).TextLabel.Text = CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "3C885484-C145-46DD-BEFB-DE34A8C88665", "ACCOUNT SPECIFIC ALERTS");			
		}

		public async void EnableAlerts(bool enable)
		{
			ShowActivityIndicator();

			var methods = new MessagingMethods();
			await methods.UpdatePushNotificationAlertSettings(enable, View);

			HideActivityIndicator();

			if (_model?.Result != null)
			{
				_model.Result.Enabled = enable;
			}

			GetPushNotificationSettings();
		}

		public async void EnableSecurityAlerts(bool enable)
		{
			ShowActivityIndicator();

			var methods = new MessagingMethods();
			await methods.UpdateSecurityAlertSettings(enable, View);

			HideActivityIndicator();

			if (_model?.Result?.SecurityAlertsSetting != null)
			{
				_model.Result.SecurityAlertsSetting.Enabled = enable;
			}
		}

		private void Refresh()
		{
			_model = null;
			GetPushNotificationSettings();
		}

		private async void TurnOnSystemAlerts()
		{
			// Check to see if we are registered in iOS and turn on notifications if we are not.
			if (!UIApplication.SharedApplication.IsRegisteredForRemoteNotifications)
			{
				UIApplication.SharedApplication.RegisterForRemoteNotifications();

				var responseBody = CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "F0FEC15A-950F-4D96-8A26-9074801E6626", "Notifications have been turned on for SunMobile alerts.  To manage your iOS notifications, go to Settings.");
				var close = CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "8BC33FC5-2D7A-4C36-89F0-E4B9B2B794EF", "Close");
				var settings = CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "59A830A3-4073-4E07-85BF-52499143CCA3", "Settings");
				var response = await AlertMethods.Alert(View, "SunMobile", responseBody, close, settings);

				if (response == settings)
				{
					var settingsUrl = new NSUrl(UIApplication.OpenSettingsUrlString);
					UIApplication.SharedApplication.OpenUrl(settingsUrl);
				}
			}
		}

		private async void GetPushNotificationSettings()
		{
			if (_model == null)
			{
				ShowActivityIndicator();

				var methods = new MessagingMethods();
				_model = await methods.GetPushNotificationSettings(null, View);

				HideActivityIndicator();
			}

			if (_model?.Result?.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings != null)
			{				
				switchAlerts.On = _model.Result.Enabled;

				if (_model?.Result?.SecurityAlertsSetting != null)
				{
					switchSecurityAlerts.On = _model.Result.SecurityAlertsSetting.Enabled;
				}

				var tableViewSource = new ManageAlertsTableViewSource(_model.Result.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings, _model.Result.Enabled);

				tableViewSource.ItemSelected += (row) =>
				{
					if (row == 1)
					{
						var eStatementAlertOptionsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("EStatementAlertOptionsTableViewController") as EStatementAlertOptionsTableViewController;
						NavigationController.PushViewController(eStatementAlertOptionsTableViewController, true);
					}
				};

				tableViewSource.AccountSelected += item =>
				{
					var controller = AppDelegate.StoryBoard.InstantiateViewController("AlertSettingTableViewController") as AlertSettingTableViewController;
					controller.Model = (AccountSpecificAlertModel)item;

					controller.ItemChanged += accountSpecificAlertModel => 
					{						
						// I'm updating the model instead of doing a refresh for speed.
						int index = _model.Result.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings.FindIndex(x => x.AccountId == accountSpecificAlertModel.AccountId);

						if (index >= 0)
						{
							_model.Result.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings[index] = accountSpecificAlertModel;
						}

						GetPushNotificationSettings();
					};

					NavigationController.PushViewController(controller, true);
				};

				tableViewMenu.Source = tableViewSource;
				tableViewMenu.ReloadData();
			}

			if (switchAlerts.On)
			{
				TurnOnSystemAlerts();
			}
		}
	}
}