using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Views;
using SunMobile.Shared.Data;

namespace SunMobile.Droid.Messaging
{
	public class MessagesListAdapter : BaseAdapter<MessageViewModel>
	{
		private Activity _activity;
		private List<MessageViewModel> _model;
		private int _listViewResourceId;
		private int _textViewResourceId;
		private int _textView2ResourceId;
		private int _textView3ResourceId;

		public MessagesListAdapter(Activity activity, int listViewResourceId, List<MessageViewModel> model, int textViewResourceId, int textView2ResourceId, int textView3ResourceId) 
		{
			_activity = activity;
			_model = model;
			_listViewResourceId = listViewResourceId;
			_textViewResourceId = textViewResourceId;
			_textView2ResourceId = textView2ResourceId;
			_textView3ResourceId = textView3ResourceId;
		}

		public override int Count
		{
			get { return _model.Count; }
		}

		public MessageViewModel GetListViewItem(int position)
		{
			return _model[position];
		}

		public override MessageViewModel this[int position]
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

			var textView = row.FindViewById<TextView>(_textViewResourceId);

			if (textView != null)
			{
				textView.Text = _model[position].FriendlyDate; 
			}

			var textView2 = row.FindViewById<TextView>(_textView2ResourceId);

			if (textView2 != null)
			{
				textView2.Text = _model[position].BodyStrippedOfHtml;
			}

			var textView3 = row.FindViewById<TextView>(_textView3ResourceId);

			if (textView3 != null)
			{
				textView3.Text = _model[position].Subject; 
			}

			var unreadImage = row.FindViewById<ImageView>(Resource.Id.imgUnread);

			if (unreadImage != null)
			{
				if (_model[position].IsRead)
				{
					unreadImage.Visibility = ViewStates.Invisible;
				}
			}

			return row;
		}
	}
}