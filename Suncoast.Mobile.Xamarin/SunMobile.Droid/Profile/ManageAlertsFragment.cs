using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Common.Utilities.Serialization;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Culture;

namespace SunMobile.Droid.Profile
{
	public class ManageAlertsFragment : BaseListFragment
	{
		private StatusResponse<NotificationSettingsModel> _model;
		private Switch switchAlerts;
		private Switch switchSecurityAlerts;
		private TableRow tableRowEStatementAlertOptions;
		private TextView lblSecurityAlerts;
		private TextView lblElectronicDocumentAlertPreferences;
		private TextView lblAccountSpecificAlerts;
		private TextView lblAccountSpecificAlertsHeader;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.ManageAlertSettingsView, null);

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = Json.Serialize(_model);
			outState.PutString("Model", json);

			if (_model?.Result != null)
			{
				json = Json.Serialize(_model.Result);
				outState.PutString("Model.Result", json);
			}

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Manage Alert Settings");

			switchAlerts = Activity.FindViewById<Switch>(Resource.Id.switchAlerts);
			switchSecurityAlerts = Activity.FindViewById<Switch>(Resource.Id.switchSecurityAlerts);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("Model");
				_model = Json.Deserialize<StatusResponse<NotificationSettingsModel>>(json);

				// TODO: Sometimes this is null
				try
				{
					json = savedInstanceState.GetString("Model.Result");

					if (!string.IsNullOrEmpty(json))
					{
						_model.Result = Json.Deserialize<NotificationSettingsModel>(json);
					}

					switchAlerts.Checked = _model.Result.Enabled;
					switchSecurityAlerts.Checked = _model.Result.Enabled;
				}
				catch { }
			}

			switchAlerts.CheckedChange += EnableAlerts;
			switchSecurityAlerts.CheckedChange += EnableSecurityAlerts;

			tableRowEStatementAlertOptions = Activity.FindViewById<TableRow>(Resource.Id.rowAlertNotificationAvailable);
			tableRowEStatementAlertOptions.Click += (sender, e) =>
			{
				var eStatementAertOptionsFragment = new EStatementAlertOptionsFragment();
				NavigationService.NavigatePush(eStatementAertOptionsFragment, true, false);
			};

			GetPushNotificationSettings();

			lblSecurityAlerts = Activity.FindViewById<TextView>(Resource.Id.lblSecurityAlerts);
			lblElectronicDocumentAlertPreferences = Activity.FindViewById<TextView>(Resource.Id.lblNotificationAvailable);
			lblAccountSpecificAlerts = Activity.FindViewById<TextView>(Resource.Id.lblAccountSpecificAlerts);
			lblAccountSpecificAlertsHeader = Activity.FindViewById<TextView>(Resource.Id.rowAccountSpecificAlertsHeader);			
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "60681413-42D0-45C0-8318-6EDB1BCB3678", "Manage Alert Settings"));
                CultureTextProvider.SetMobileResourceText(lblSecurityAlerts, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "C38700E3-CD85-49DF-ACD0-F66FBB788835", "Security Alerts");
                CultureTextProvider.SetMobileResourceText(lblElectronicDocumentAlertPreferences, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "46AA19BF-64F6-4DF6-A5C9-4BD28DE68D67", "Electronic Document Alert Preferences");
                CultureTextProvider.SetMobileResourceText(lblAccountSpecificAlerts, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "FACF6F3B-FB66-4971-9DF1-B566664CC41F", "Account Specific Alerts");
                CultureTextProvider.SetMobileResourceText(lblAccountSpecificAlertsHeader, "A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "3C885484-C145-46DD-BEFB-DE34A8C88665", "ACCOUNT SPECIFIC ALERTS");
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "ManageAlertsFragment:SetCultureConfiguration");
            }
		}

		public async void EnableAlerts(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			var enable = switchAlerts.Checked;

			ShowActivityIndicator();

			var methods = new MessagingMethods();
			await methods.UpdatePushNotificationAlertSettings(enable, Activity);

			HideActivityIndicator();

			if (_model?.Result != null)
			{
				_model.Result.Enabled = enable;
			}

			GetPushNotificationSettings();
		}

		public async void EnableSecurityAlerts(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			var enable = switchSecurityAlerts.Checked;

			ShowActivityIndicator();

			var methods = new MessagingMethods();
			await methods.UpdateSecurityAlertSettings(enable, Activity);

			HideActivityIndicator();

			if (_model?.Result?.SecurityAlertsSetting != null)
			{
				_model.Result.SecurityAlertsSetting.Enabled = enable;
			}
		}

		private void TurnOnSystemAlerts()
		{
			/* TODO: Get this working in Android
			// Check to see if we are registered in Android and turn on notifications if we are not.
			var notificationListeners = Settings.Secure.GetString(Activity.ApplicationContext.ContentResolver, "enabled_notification_listeners");

			if (notificationListeners != null && !notificationListeners.Contains(Activity.ApplicationContext.PackageName))
			{
				var response = await AlertMethods.Alert(Activity, "SunMobile", "System notifications are off for SunMobile alerts.  To manage your Android notifications, go to Settings.", "Close", "Settings");

				if (response == "Settings")
				{
					var intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
					StartActivity(intent);
				}
			}
			*/
		}

		private async void GetPushNotificationSettings()
		{
			if (_model == null)
			{
				ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("A6CEA528-1440-4CD4-B21B-02484B8A5AC5", "C12A8539-0892-4815-B6E9-6831B0A16533", "Loading settings..."));

				var methods = new MessagingMethods();
				_model = await methods.GetPushNotificationSettings(null, Activity);

				HideActivityIndicator();
			}

			if (_model?.Result?.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings != null)
			{
				switchAlerts.CheckedChange -= EnableAlerts;
				switchAlerts.Checked = _model.Result.Enabled;
				switchAlerts.CheckedChange += EnableAlerts;

				if (_model?.Result?.SecurityAlertsSetting != null)
				{
					switchSecurityAlerts.CheckedChange -= EnableSecurityAlerts;
					switchSecurityAlerts.Checked = _model.Result.SecurityAlertsSetting.Enabled;
					switchSecurityAlerts.CheckedChange += EnableSecurityAlerts;
				}

				var listAdapter = new ManageAlertsListAdapter(Activity, Resource.Layout.AlertSettingsListViewItem, _model.Result.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings, _model.Result.Enabled);
				ListAdapter = listAdapter;
			}

			if (switchAlerts.Checked)
			{
				TurnOnSystemAlerts();
			}
		}

		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);

			try
			{
				var item = (AccountSpecificAlertModel)((ManageAlertsListAdapter)l.Adapter).GetAccountItem(position);

				var alertSettingsFragment = new AlertSettingsFragment();
				// Making a copy of item so our model doesn't get inadvertantly changed.
				var json = Json.Serialize(item);
				var itemCopy = Json.Deserialize<AccountSpecificAlertModel>(json);
				alertSettingsFragment.Model = itemCopy;

				alertSettingsFragment.ItemChanged += accountSpecificAlertModel =>
				{
					// I'm updating the model instead of doing a refresh for speed.
					int index = _model.Result.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings.FindIndex(x => x.AccountId == accountSpecificAlertModel.AccountId);

					if (index >= 0)
					{
						_model.Result.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings[index] = accountSpecificAlertModel;
					}

					GetPushNotificationSettings();
				};

				if (alertSettingsFragment != null)
				{
					NavigationService.NavigatePush(alertSettingsFragment, true, false);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectAccountActivity:OnListItemClick");
			}
		}
	}
}