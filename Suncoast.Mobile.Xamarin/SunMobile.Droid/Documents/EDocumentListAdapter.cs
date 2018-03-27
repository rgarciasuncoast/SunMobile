using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Views;

namespace SunMobile.Droid
{
	public class EDocumentListAdapter : BaseAdapter<ListViewItem>
	{
		private Activity _activity;
		private List<ListViewItem> _list;
		public bool _enrolled;

		public EDocumentListAdapter(Activity activity, List<ListViewItem> documentsList, bool enrolled)
		{
			_enrolled = enrolled;
			_activity = activity;

			try
			{
				_list = documentsList;
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "EDocumentListAdapter");
			}
		}

		public override ListViewItem this[int position]
		{
			get
			{
				return _list[position];
			}
		}

		public override int Count
		{
			get
			{
				return _enrolled ? _list.Count : 1;
			}
		}

		public ListViewItem GetListViewItem(int position)
		{
			if (_enrolled)
			{
				var returnValue = new ListViewItem();

				if (position >= 0 && position < _list.Count)
				{
					returnValue = _list[position];
				}

				return returnValue;
			}
			else
			{
				var item = new ListViewItem
				{
					Item2Text = "You are not currently enrolled.",
					Item1Text = string.Empty,
					MoreIconVisible = false
				};

				return item;
			}
		}


		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			if (_enrolled)
			{
				View row = _activity.LayoutInflater.Inflate(Resource.Layout.DocumentsListViewItem, null);

				var listItem = _list[position];

				//The date of the document
				var dateText = row.FindViewById<TextView>(Resource.Id.lblDate);
				dateText.Text = listItem.Item1Text;

				//The document name
				var documentNameText = row.FindViewById<TextView>(Resource.Id.lblDocumentName);
				documentNameText.Text = listItem.Item2Text;

				return row;
			}
			else
			{
				View row = _activity.LayoutInflater.Inflate(Resource.Layout.DocumentsListViewItem, null);

				var listItem = GetListViewItem(0);

				var dateText = row.FindViewById<TextView>(Resource.Id.lblDate);
				dateText.Text = string.Empty;

				var documentNameText = row.FindViewById<TextView>(Resource.Id.lblDocumentName);
				documentNameText.Text = listItem.Item2Text;

				var imgMore = row.FindViewById<ImageView>(Resource.Id.imageArrow);
				imgMore.Visibility = ViewStates.Gone;

				return row;
			}
		}
	}
}