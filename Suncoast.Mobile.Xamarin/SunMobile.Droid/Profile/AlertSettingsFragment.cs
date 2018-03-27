using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Common.Utilities.Serialization;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Profile
{
	public class AlertSettingsFragment : BaseListFragment
	{
		public AccountSpecificAlertModel Model { get; set; }
		public event Action<AccountSpecificAlertModel> ItemChanged = delegate{};
		public List<AlertSetting> _itemsToChange;
		public bool _isDirty;
		private List<AlertSetting> _itemsChanged;
		private Android.Support.V4.App.FragmentActivity activityHolder;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AlertSettingsListView, null);

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = Json.Serialize(Model);
			outState.PutString("Model", json);
			json = Json.Serialize(_itemsToChange);
			outState.PutString("ItemsToChange", json);
			json = Json.Serialize (_itemsChanged);
			outState.PutString("ItemsChanged", json);
			outState.PutBoolean("IsDirty", _isDirty);

			base.OnSaveInstanceState (outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			activityHolder = Activity;

			_itemsChanged = new List<AlertSetting>();

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("Model");
				Model = Json.Deserialize<AccountSpecificAlertModel>(json);
				json = savedInstanceState.GetString("ItemsToChange");
				_itemsToChange = Json.Deserialize<List<AlertSetting>>(json);
				json = savedInstanceState.GetString("ItemsChanged");
				_itemsChanged = Json.Deserialize<List<AlertSetting>>(json);
				_isDirty = savedInstanceState.GetBoolean("IsDirty");
			}

			LoadSettings();

			if (Model != null)
			{
				((MainActivity)Activity).SetActionBarTitle(Model.DisplayText);
			}
			else
			{
				((MainActivity)Activity).SetActionBarTitle("Manage Alert Settings");
			}
		}

		public override async void OnDestroyView()
		{
			base.OnDestroyView();

			if (_isDirty)
			{
				ShowActivityIndicator();

				var methods = new MessagingMethods();
				await methods.SaveAlertSettings(_itemsChanged, Model, Activity);

				HideActivityIndicator();

				ItemChanged(Model);
			}
		}

		public override void OnListItemClick(Android.Widget.ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);

			var listViewItem = ((AlertSettingListAdapter)l.Adapter).GetListViewItem(position);

			if (listViewItem.MoreIconVisible)
			{
				var alertSettingsDetailFragment = new AlertSettingsDetailFragment();
				alertSettingsDetailFragment.Model = (AvailableBalanceThresholdAlertModel)listViewItem.Data;

				alertSettingsDetailFragment.ItemChanged += async (AlertSetting setting) =>
				{
					_isDirty = true;

					// Update the model
					Model.AvailableBalaceThresholdAlertSettings.Enabled = setting.Value;
					Model.AvailableBalaceThresholdAlertSettings.ThreshHoldAmount = setting.Amount;

					LoadSettings();

					// Update the database
					var request = new AvailableBalanceThresholdSettingsUpdateRequest 
					{
						Suffix = Model.AccountId,
						AccountSettingType = Model.AccountSettingType,
						Value = setting.Value,
						ThresholdAmount = setting.Amount
					};

					var methods = new MessagingMethods();
					await methods.UpdateAvailableBalanceAlertSettings(request, activityHolder);
				};

				NavigationService.NavigatePush(alertSettingsDetailFragment, true, false);
			}
		}

		private void LoadSettings()
		{
			if (Model != null)
			{
				var methods = new MessagingMethods();
				var listViewItems = methods.LoadAlertSettingsFromModel(Model);

				var listAdapter = new AlertSettingListAdapter(Activity, Resource.Layout.AlertSettingsListViewItem, listViewItems);
				ListAdapter = listAdapter;

				listAdapter.ItemsChanged += async (List<AlertSetting> items) =>
				{
					_itemsChanged = items;
					_isDirty = _itemsChanged.Count > 0;

					if (_isDirty)
					{
						await methods.SaveAlertSettings(_itemsChanged, Model, View, false);
					}
				};
			}
		}
	}
}