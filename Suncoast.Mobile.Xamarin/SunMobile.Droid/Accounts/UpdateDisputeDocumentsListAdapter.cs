using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunMobile.Droid.Common;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts
{
	public class UpdateDisputeDocumentsListAdapter : GenericListAdapter
	{
		public event Action<ListViewItem> RemoveSelected = delegate { };

		public UpdateDisputeDocumentsListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int[] textViewResourceIds, string[] classFields)
			: base(activity, listViewResourceId, list, textViewResourceIds, classFields)
		{
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = null;

			row = _activity.LayoutInflater.Inflate(_listViewResourceId, null);
			object listItem = _list[position];

			var imgRemove = row.FindViewById<ImageButton>(Resource.Id.btnRemove);
			imgRemove.Click += (sender, e) =>
			{
				RemoveSelected(_list[position]);
			};

			PopulateGenericViews(row, listItem);

			var item2Text = _list[position].Item2Text;
			var errorRow = row.FindViewById<TableRow>(Resource.Id.trowItemMessage);

			if (item2Text.Contains("Error"))
			{
				errorRow.Visibility = ViewStates.Visible;

			}
			else
			{
				errorRow.Visibility = ViewStates.Gone;
			}

			return row;
		}
	}
}