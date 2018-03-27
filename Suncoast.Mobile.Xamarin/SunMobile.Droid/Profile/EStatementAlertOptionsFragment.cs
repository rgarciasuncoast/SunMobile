using System;
using System.Collections.Generic;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.StringUtilities;

namespace SunMobile.Droid.Profile
{
	public class EStatementAlertOptionsFragment : BaseFragment
	{
		private AlertSettings _alertSettings;
		private Switch switchEStatementAlerts;
		private Switch switchSendPushAlert;
		private Spinner spinnerAlertType;
		private TextView lblAlertType;
		private EditText txtAlertAddress;
		private Button btnUpdate;
		private ArrayAdapter _spinnerAdapter;
		private bool _enableChanged;
		private bool _gotAlertSettings;
		private TextView lblNotify;
		private TextView lblEmailTextNumber;
		private TextView lblSendAnAlert;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.EDocumentsAlertPreferencesView, null);
			RetainInstance = true;

			return view;
		}

        public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "178ee94e-7e62-4c82-9a54-2f5e7ebd0e5d", "Alert Settings"));
                CultureTextProvider.SetMobileResourceText(lblNotify, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "d7a28196-c526-408d-9ca4-bb978be9f9ac", "Notify me when my eNotice or eStatement is available");
                CultureTextProvider.SetMobileResourceText(lblEmailTextNumber, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "1fc6b9ff-64d5-40cc-b44c-6f3c5aafabc3", "Email Address");
                CultureTextProvider.SetMobileResourceText(lblSendAnAlert, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "9ed58b25-1536-4df1-a1e9-1573ddf8d7f6", "Send an alert to my device");
                CultureTextProvider.SetMobileResourceText(btnUpdate, "a6cea528-1440-4cd4-b21b-02484b8a5ac5", "4f470296-f72c-47c9-86af-054ff8133cb7", "Update");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "EStatementAlertOptionsFragment:SetCultureConfiguration");
            }
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			lblNotify = Activity.FindViewById<TextView>(Resource.Id.lblEDocumentsAlertPrefNotify);
			lblEmailTextNumber = Activity.FindViewById<TextView>(Resource.Id.lblEmailTextNumber);
			lblSendAnAlert = Activity.FindViewById<TextView>(Resource.Id.lblEDocumentsPrefSendAlert);

			switchEStatementAlerts = Activity.FindViewById<Switch>(Resource.Id.switchAlertNotificationsAvailable);
			switchEStatementAlerts.CheckedChange += (sender, e) =>
			{
				_enableChanged = true;
				Validate();
			};

			switchSendPushAlert = Activity.FindViewById<Switch>(Resource.Id.switchDeviceAlerts);
			switchSendPushAlert.CheckedChange += (sender, e) =>
			{
				_enableChanged = true;
				Validate();
			};

			spinnerAlertType = Activity.FindViewById<Spinner>(Resource.Id.spinnerAlertType);
			var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");
			var textMessage = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "49f62c66-802f-48d9-8220-b342f8116da7", "Text Message");
			var items = new List<string> { email, textMessage };
			_spinnerAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, items);
			spinnerAlertType.Adapter = _spinnerAdapter;

			lblAlertType = Activity.FindViewById<TextView>(Resource.Id.lblEmailTextNumber);

			txtAlertAddress = Activity.FindViewById<EditText>(Resource.Id.txtEmailTextNumber);
			txtAlertAddress.AfterTextChanged += (sender, e) =>
			{
				Validate();
			};

			btnUpdate = Activity.FindViewById<Button>(Resource.Id.btnUpdate);
			btnUpdate.Click += (sender, e) => Update();

			if (savedInstanceState != null)
			{
				_gotAlertSettings = savedInstanceState.GetBoolean("GotAlertSettings");
				_enableChanged = savedInstanceState.GetBoolean("EnableChanged");
				switchEStatementAlerts.Checked = savedInstanceState.GetBoolean("SwitchEStatementAlerts");
				switchSendPushAlert.Checked = savedInstanceState.GetBoolean("SwitchSendPushAlert");
				var selectedPosition = savedInstanceState.GetInt("SpinnerAlertType");
				spinnerAlertType.SetSelection(selectedPosition);
				lblAlertType.Text = savedInstanceState.GetString("lblAlertType");
				txtAlertAddress.Text = savedInstanceState.GetString("TxtAlertAddress");
			}

			GetEDocumentAlertSettings();

			spinnerAlertType.ItemSelected += SpinnerAlertType_ItemSelected;			
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			outState.PutBoolean("GotAlertSettings", _gotAlertSettings);
			outState.PutBoolean("EnableChanged", _enableChanged);
			outState.PutBoolean("SwitchEStatementAlerts", switchEStatementAlerts.Checked);
			outState.PutBoolean("SwitchSendPushAlert", switchSendPushAlert.Checked);
			outState.PutString("TxtAlertAddress", txtAlertAddress.Text);
			outState.PutString("lblAlertType", lblAlertType.Text);
			outState.PutInt("SpinnerAlertType", spinnerAlertType.SelectedItemPosition);
		}

		void SpinnerAlertType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var text = _spinnerAdapter.GetItem(((Spinner)sender).SelectedItemPosition).ToString();
			var emailAddress = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "1fc6b9ff-64d5-40cc-b44c-6f3c5aafabc3", "Email Address");
			var phoneNumber = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "423794e8-1ed5-4c86-8778-5a60bf11740b", "Phone Number");
			var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");
			lblAlertType.Text = (text == email) ? emailAddress : phoneNumber;
			txtAlertAddress.Text = string.Empty;
			txtAlertAddress.InputType = text == email ? InputTypes.TextVariationEmailAddress : InputTypes.ClassPhone;
			var textLength = text == email ? 255 : 10; ;
			txtAlertAddress.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(textLength) });
			Validate();
		}

		private async void GetEDocumentAlertSettings()
		{
			if (!_gotAlertSettings)
			{
				ShowActivityIndicator();

				var methods = new DocumentMethods();
				var response = await methods.GetEDocumentAlertSettings(null, Activity);

				var messagingMethods = new MessagingMethods();
				var pushSettingsResponse = await messagingMethods.GetPushNotificationSettings(null, Activity);

				HideActivityIndicator();

				if (response != null && response.Success && response.Result != null)
				{
					_alertSettings = response.Result;
					switchEStatementAlerts.Checked = _alertSettings.AlertEnabled;

					if (_alertSettings.AlertType == AlertTypes.Email.ToString())
					{
						spinnerAlertType.SetSelection(0);
					}
					else
					{
						spinnerAlertType.SetSelection(1);
					}
					var emailAddress = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "1fc6b9ff-64d5-40cc-b44c-6f3c5aafabc3", "Email Address");
					var phoneNumber = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "423794e8-1ed5-4c86-8778-5a60bf11740b", "Phone Number");
					lblAlertType.Text = _alertSettings.AlertType == AlertTypes.Email.ToString() ? emailAddress : phoneNumber;
					txtAlertAddress.Text = _alertSettings.AlertEmail;
				}

				if (pushSettingsResponse?.Result?.AccountSpecificAlertSettingsSet.AccountSpecificAlertSettings != null)
				{
					if (pushSettingsResponse?.Result?.EDocumentAlertsSetting != null)
					{
						switchSendPushAlert.Checked = pushSettingsResponse.Result.EDocumentAlertsSetting.Enabled;
					}
				}

				_gotAlertSettings = true;

				// Delay the setting of txtAlertAddress until after SpinnerChanged.
				var updateText = new Handler();
				updateText.PostDelayed(() =>
			   {
					txtAlertAddress.Text = _alertSettings.AlertEmail;				   
					btnUpdate.Enabled = false;
			   }, 200);
			}
		}

		private void Validate()
		{
			bool validated = false;

			if (switchEStatementAlerts.Checked)
			{
				var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");
				if (_spinnerAdapter.GetItem(spinnerAlertType.SelectedItemPosition).ToString() == email)
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

			btnUpdate.Enabled = validated;
			spinnerAlertType.Enabled = txtAlertAddress.Enabled = switchEStatementAlerts.Checked;
		}

		private async void Update()
		{
			var email = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "75a39833-6556-4a98-bf6d-abbfdcc2926b", "Email");
			var request = new EDocumentSetAlertRequest
			{
				AlertEnabled = switchEStatementAlerts.Checked,
				AlertType = _spinnerAdapter.GetItem(spinnerAlertType.SelectedItemPosition).ToString() == email ? AlertTypes.Email.ToString() : AlertTypes.Sms.ToString(),
				EmailAddress = txtAlertAddress.Text
			};

			ShowActivityIndicator();

			var methods = new DocumentMethods();
			var response = await methods.SetEDocumentAlertSettings(request, Activity);
			var messagingMethods = new MessagingMethods();
			var messagingResponse = await messagingMethods.UpdateEDocumentAlertSettings(switchSendPushAlert.Checked, Activity);

			HideActivityIndicator();

			if (response != null && response.Success && messagingResponse != null && messagingResponse.Success)
			{
				NavigationService.NavigatePop();
			}
			else
			{
				var errorText = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "09968389-0119-41f8-b411-ffeb05af2483", "Unable to update alert settings.");
				var ok = CultureTextProvider.GetMobileResourceText("a6cea528-1440-4cd4-b21b-02484b8a5ac5", "580eb21e-0104-44d2-ae36-ab6bf95a6cb8", "OK");
				await AlertMethods.Alert(View, "SunMobile", errorText, ok);
			}
		}
	}
}