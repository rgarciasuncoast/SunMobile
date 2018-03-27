using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunMobile.Shared.Data;
using SunMobile.Shared.StringUtilities;

namespace SunMobile.Droid.Profile
{
	public class AlertSettingsDetailFragment : BaseFragment
	{
		public AvailableBalanceThresholdAlertModel Model { get; set; }
		public event Action<AlertSetting> ItemChanged = delegate{};
		private TextView lblDescription;
		private Switch switchEnabled;
		private EditText txtAmount;
		private bool _isDirty;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AlertSettingsDetailView, null);

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = JsonConvert.SerializeObject(Model);
			outState.PutString("Model", json);
			outState.PutString("Description", lblDescription.Text);
			outState.PutBoolean("Enabled", switchEnabled.Checked);
			outState.PutString("Amount", txtAmount.Text);
			outState.PutBoolean("IsDirty", _isDirty);

			base.OnSaveInstanceState (outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Account Alert");

			lblDescription = Activity.FindViewById<TextView>(Resource.Id.lblDescription);
			switchEnabled = Activity.FindViewById<Switch>(Resource.Id.switchEnabled);
			txtAmount = Activity.FindViewById<EditText>(Resource.Id.txtAlertSetting);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString ("Model");
				Model = JsonConvert.DeserializeObject<AvailableBalanceThresholdAlertModel> (json);
			}

			lblDescription.Text = Model.DisplayText;
			switchEnabled.Checked = Model.Enabled;

			if (Model.ThreshHoldAmount > 0)
			{
				txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(Model.ThreshHoldAmount.ToString());
			}

			txtAmount.AfterTextChanged += OnTextChanged;

			switchEnabled.CheckedChange += (sender, e) => 
			{
				_isDirty = Model.Enabled != e.IsChecked;
			};

			if (savedInstanceState != null)
			{
				lblDescription.Text = savedInstanceState.GetString ("Description");
				switchEnabled.Checked = savedInstanceState.GetBoolean ("Enabled");
				txtAmount.Text = savedInstanceState.GetString ("Amount");
				var dirtyOr = savedInstanceState.GetBoolean ("IsDirty");
				if (dirtyOr)
				{
					_isDirty = true;
				}
			}
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			txtAmount.AfterTextChanged -= OnTextChanged;

			var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(((EditText)sender).Text));

			decimal result;
			decimal.TryParse(amount, out result);

			_isDirty = Model.ThreshHoldAmount != result;

			txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((TextView)sender).Text);

			txtAmount.SetSelection(txtAmount.Text.Length);

			txtAmount.AfterTextChanged += OnTextChanged;
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();

			if (_isDirty)
			{
				var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(txtAmount.Text));

				decimal result;
				decimal.TryParse(amount, out result);

				var alertSetting = new AlertSetting
				{
					Description = "AvailableBalaceThresholdAlertSettings",
					Value = switchEnabled.Checked,
					Amount = result
				};

				ItemChanged(alertSetting);
			}
		}
	}
}