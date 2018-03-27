using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Views;

namespace SunMobile.Droid
{
	public class TaxDocumentsListAdapter : BaseAdapter<ListViewItem>
	{
		private Activity _activity;
		private List<ListViewItem> _list;

		public TaxDocumentsListAdapter(Activity activity, List<ListViewItem> documentsList)
		{
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
                return _list.Count;
			}
		}		

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
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
	}
}