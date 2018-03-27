using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Common
{
	public class StringWithCheckBoxListAdapter : BaseAdapter<ListViewItem>
	{
		public List<string> SelectedItems { get; set; }
		private Activity _activity;
		private List<ListViewItem> _list;
		private int _listViewResourceId;
		private int _textViewResourceId;
		private int _textView2ResourceId;
		private int _checkBoxResourceId;

		public StringWithCheckBoxListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int textViewResourceId, int checkBoxResourceId) 
		{
			_activity = activity;
			_list = list;
			_listViewResourceId = listViewResourceId;
			_textViewResourceId = textViewResourceId;
			_checkBoxResourceId = checkBoxResourceId;
			SelectedItems = new List<string>();
		}

		public StringWithCheckBoxListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int textViewResourceId, int textView2ResourceId, int checkBoxResourceId) 
		{
			_activity = activity;
			_list = list;
			_listViewResourceId = listViewResourceId;
			_textViewResourceId = textViewResourceId;
			_textView2ResourceId = textView2ResourceId;
			_checkBoxResourceId = checkBoxResourceId;
			SelectedItems = new List<string>();
		}

		public override int Count
		{
			get { return _list.Count; }
		}

		public ListViewItem GetListViewItem(int position)
		{
			return _list[position];
		}

		public override ListViewItem this[int index]
		{
			get { return _list[index]; }
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
			View row ;

			if (convertView == null) 
			{
				row = _activity.LayoutInflater.Inflate(_listViewResourceId, null);
			}
			else
			{
				row = convertView;
			}

			var textView = row.FindViewById<TextView>(_textViewResourceId);

			if (textView != null)
			{
				textView.Text = _list[position].Item1Text; 
			}

			var textView2 = row.FindViewById<TextView>(_textView2ResourceId);

			if (textView2 != null)
			{
				textView2.Text = _list[position].Item2Text; 
			}

			var checkBox = row.FindViewById<CheckBox>(_checkBoxResourceId);
			checkBox.Tag = _list[position].Item1Text;

			_list[position].IsChecked = SelectedItems.Contains((checkBox).Tag.ToString());

			checkBox.CheckedChange += (sender, e) =>
			{
				if (e.IsChecked)
				{
					if (!SelectedItems.Contains(((CheckBox)sender).Tag.ToString()))
					{
						SelectedItems.Add(((CheckBox)sender).Tag.ToString());
					}
				}
				else
				{
					SelectedItems.Remove(((CheckBox)sender).Tag.ToString());
				}
			};

			if (checkBox != null)
			{
				checkBox.Checked = _list[position].IsChecked;
			}

			return row;
		}
	}
}