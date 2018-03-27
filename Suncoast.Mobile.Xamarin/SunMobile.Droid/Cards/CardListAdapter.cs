using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Cards
{
	public class CardListAdapter : BaseAdapter<ListViewItem>
	{
		public List<BankCard> SelectedItems { get; set; }
        public bool SingleSelection { get; set; }
		private Activity _activity;
		private List<ListViewItem> _list;
		private int _listViewResourceId;
		private int _textViewResourceId;
		private int _textView2ResourceId;
		private int _checkBoxResourceId;

		public CardListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int textViewResourceId, int textView2ResourceId, int checkBoxResourceId) 
		{
			_activity = activity;
			_list = list;
			_listViewResourceId = listViewResourceId;
			_textViewResourceId = textViewResourceId;
			_textView2ResourceId = textView2ResourceId;
			_checkBoxResourceId = checkBoxResourceId;
			SelectedItems = new List<BankCard>();
		}

		public override int Count
		{
			get { return _list.Count; }
		}

		public ListViewItem GetListViewItem(int position)
		{
			return _list[position];
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
			checkBox.Tag = JsonConvert.SerializeObject(_list[position].Data);

            if (SingleSelection)
            {
                checkBox.Visibility = ViewStates.Invisible;
            }

			checkBox.CheckedChange += (sender, e) =>
			{
				if (e.IsChecked)
				{
					var bankCard = JsonConvert.DeserializeObject<BankCard>((string)((CheckBox)sender).Tag);
					var doAdd = 0;

					for (int i = 0; i < SelectedItems.Count; i++)
					{
						if (SelectedItems[i].CardAccountNumber == bankCard.CardAccountNumber)
						{
							doAdd ++;
						}
					}

					if (doAdd == 0)
					{
						SelectedItems.Add(JsonConvert.DeserializeObject<BankCard>((string)((CheckBox)sender).Tag));
					}
				}
				else
				{
					var bankCard = JsonConvert.DeserializeObject<BankCard>((string)((CheckBox)sender).Tag);

					for (int i = 0; i < SelectedItems.Count; i++)
					{
						if (SelectedItems[i].CardAccountNumber == bankCard.CardAccountNumber)
						{
							SelectedItems.RemoveAt(i);
						}
					}
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