using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Data;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Profile
{
	public class AlertSettingListAdapter : BaseAdapter<ListViewItem>
	{
		public event Action<List<AlertSetting>> ItemsChanged = delegate{};
		public List<AlertSetting> _itemsChanged;
		private readonly List<ListViewItem> _model;
		private Activity _activity;
		private int _listViewResourceId;

		public AlertSettingListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> model) 
		{
			_activity = activity;
			_listViewResourceId = listViewResourceId;
			_model = model;
			_itemsChanged = new List<AlertSetting>();
		}

		public override int Count
		{
			get { return _model.Count; }
		}

		public ListViewItem GetListViewItem(int position)
		{
			return _model[position];
		}

		public override ListViewItem this[int position]
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

			var item = _model[position];

			var lbleAccountName = row.FindViewById<TextView>(Resource.Id.lblAccountName);

			if (lbleAccountName != null)
			{
				lbleAccountName.Text = item.Item1Text;
			}

			var lblEnabled = row.FindViewById<TextView>(Resource.Id.lblAccountAlertsEnabled);

			if (lblEnabled != null)
			{
				lblEnabled.Text = item.IsChecked ? "On" : "Off"; 
			}

			var switchEnabled = row.FindViewById<Switch>(Resource.Id.switchEnabled);
			switchEnabled.Tag = item.Item1Text;

			var imgMore = row.FindViewById<ImageView>(Resource.Id.imgMore);

			if (item.MoreIconVisible)
			{
				switchEnabled.Visibility = ViewStates.Gone;
				lblEnabled.Visibility = ViewStates.Visible;
				imgMore.Visibility = ViewStates.Visible;
			}
			else
			{
				switchEnabled.Visibility = ViewStates.Visible;
				lblEnabled.Visibility = ViewStates.Gone;
				imgMore.Visibility = ViewStates.Gone;
			}

			if (switchEnabled != null)
			{
				switchEnabled.CheckedChange += (sender, e) =>
				{
					if (((Switch)sender).Tag.ToString() == item.Item1Text)
					{
						var alertSetting = new AlertSetting
						{
							Description = item.Item3Text,
							Value = e.IsChecked
						};

						int itemIndex = -1;

						for (int i = 0; i < _itemsChanged.Count; i++)
						{
							if (_itemsChanged[i].Description == alertSetting.Description)
							{
								itemIndex = i;
								break;
							}
						}

						if (switchEnabled.Checked != item.IsChecked)
						{
							if (itemIndex < 0)
							{
								_itemsChanged.Add(alertSetting);
								ItemsChanged(_itemsChanged);
							}
						}
						else
						{
							if (itemIndex >= 0)
							{
								_itemsChanged.RemoveAt(itemIndex);
								ItemsChanged(_itemsChanged);						
							}
						}
					}
				};

				switchEnabled.Checked = item.IsChecked;
			}

			return row;
		}
	}
}