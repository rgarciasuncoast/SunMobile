using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Documents
{
	public class DocumentsListAdapter : BaseAdapter<ListViewItem>
	{
		private Activity _activity;
		private bool _isUploadDocuments = true;
		private List<ListViewItem> _list;

		public DocumentsListAdapter(Activity activity, bool isUploadDocuments, List<ListViewItem> documentsList)
		{
			_activity = activity;

			try
			{
				_isUploadDocuments = isUploadDocuments;
				_list = documentsList;
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DocumentsListAdapter");
			}
		}

		public override int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public ListViewItem GetListViewItem(int position)
		{
			var returnValue = new ListViewItem();

			if (position >= 0 && position < _list.Count)
			{
				returnValue = _list[position];
			}

			return returnValue;
		}

		public override ListViewItem this[int position]
		{
			get { return _list[position]; }
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
			View row = null;

			var listItem = _list[position];

			if (_isUploadDocuments)
			{
				if (!listItem.Bool1Value)
				{
					if (string.IsNullOrEmpty(_list[position].Item3Text))
					{
						row = _activity.LayoutInflater.Inflate(Resource.Layout.DocumentsCenterListViewItem, null);
					}
					else
					{
						row = _activity.LayoutInflater.Inflate(Resource.Layout.DocumentsCenterListViewItemWithDescription, null);
					}

					var imageMore = row.FindViewById<ImageView>(Resource.Id.imgMore);
					imageMore.Visibility = ViewStates.Visible;

					var imageUploadButton = row.FindViewById<ImageView>(Resource.Id.imgUploadButton);

					// Using listItem.MoreIconVisible to toggle upload button
					if (!listItem.MoreIconVisible)
					{
						imageUploadButton.Enabled = false;
						imageUploadButton.SetImageResource(Resource.Color.transparent);
					}
					else
					{
						imageUploadButton.Enabled = true;
						imageUploadButton.SetImageResource(Resource.Drawable.upload);
					}

					var header = row.FindViewById<TextView>(Resource.Id.lblHeaderText);
					var status = row.FindViewById<TextView>(Resource.Id.lblStatus);
					var header2 = row.FindViewById<TextView>(Resource.Id.lblHeader2Text);

					header.Text = listItem.HeaderText;
					header2.Text = listItem.Item1Text;

					if (listItem.Item2Text == DocumentUploadStatusTypes.Requested.ToString() || listItem.Item2Text == DocumentUploadStatusTypes.Rejected.ToString())
					{
						status.Text = listItem.Item2Text;
					}
					else
					{
						status.Text = string.Empty;
					}

					if (!string.IsNullOrEmpty(listItem.Item3Text))
					{
						var note = row.FindViewById<TextView>(Resource.Id.lblNoteText);
						note.Text = listItem.Item3Text;
					}

					if (listItem.Item2Text == DocumentUploadStatusTypes.Rejected.ToString())
					{
						status.SetTextColor(Android.Graphics.Color.Red);
					}

				}
				else
				{
					row = _activity.LayoutInflater.Inflate(Resource.Layout.ListViewSectionHeader, null);

					var header = row.FindViewById<TextView>(Resource.Id.lblHeaderText);
					header.Text = listItem.HeaderText;
				}
			}
			else
			{
				if (!listItem.Bool1Value)
				{
					row = _activity.LayoutInflater.Inflate(Resource.Layout.DocumentsCenterListViewItem, null);

					var header = row.FindViewById<TextView>(Resource.Id.lblHeaderText);
					var status = row.FindViewById<TextView>(Resource.Id.lblStatus);
					var header2 = row.FindViewById<TextView>(Resource.Id.lblHeader2Text);

					header.Text = listItem.HeaderText;
					header2.Text = listItem.Item1Text;
					status.Text = string.Empty;

					var imageUploadButton = row.FindViewById<ImageView>(Resource.Id.imgUploadButton);
					imageUploadButton.Visibility = ViewStates.Gone;
				}
				else
				{
					row = _activity.LayoutInflater.Inflate(Resource.Layout.ListViewSectionHeader, null);

					var header = row.FindViewById<TextView>(Resource.Id.lblHeaderText);
					header.Text = listItem.HeaderText;
				}
			}

			return row;
		}
	}
}