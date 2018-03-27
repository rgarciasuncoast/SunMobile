using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunBlock.DataTransferObjects.Notifications.AlertSettings;

namespace SunMobile.Droid.Profile
{
	public class ManageAlertsListAdapter : BaseAdapter<IAccountSpecificAlertSettings>
	{
		private Activity _activity;
		private int _listViewResourceId;
		private readonly List<IAccountSpecificAlertSettings> _model = null;
		private bool _globalEnabled;

		public ManageAlertsListAdapter(Activity activity, int listViewResourceId, List<IAccountSpecificAlertSettings> model, bool globalEnabled) 
		{
			_activity = activity;
			_listViewResourceId = listViewResourceId;
			_model = model;
			_globalEnabled = globalEnabled;
		}

		public override int Count
		{
			get { return _model.Count; }
		}

		public IAccountSpecificAlertSettings GetAccountItem(int position)
		{
			return _model[position];
		}

		public override IAccountSpecificAlertSettings this[int position]
		{
			get { return _model[position]; }
		}

		public override Java.Lang.Object GetItem(int position) 
		{
			return null;
		}

		public override long GetItemId(int position) 
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row;

			if (convertView == null) 
			{
				row = _activity.LayoutInflater.Inflate(_listViewResourceId, null);
			}
			else
			{
				row = convertView;
			}

			var item = (AccountSpecificAlertModel)_model[position];

			var lblAccountName = row.FindViewById<TextView>(Resource.Id.lblAccountName);
			var lblAccountAlertsEnabled = row.FindViewById<TextView>(Resource.Id.lblAccountAlertsEnabled);
			var switchEnabled = row.FindViewById<Switch>(Resource.Id.switchEnabled);

			lblAccountName.Text = item.DisplayText;
			lblAccountAlertsEnabled.Text = ((item.AvailableBalaceThresholdAlertSettings != null && item.AvailableBalaceThresholdAlertSettings.Enabled) ||
	            (item.DirectDepositAlertSettings != null && item.DirectDepositAlertSettings.Enabled) ||
	            (item.NsfAlertSettings != null && item.NsfAlertSettings.Enabled) ||
	            item.PaymentReminderAlertSettings != null && item.PaymentReminderAlertSettings.Enabled) && _globalEnabled ? "On" : "Off";
			switchEnabled.Visibility = ViewStates.Gone;

			row.Clickable = !_globalEnabled;

			return row;
		}
	}
}	