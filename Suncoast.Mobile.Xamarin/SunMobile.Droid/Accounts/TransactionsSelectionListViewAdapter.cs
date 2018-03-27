using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Common
{
	public class TransactionsSelectionListViewAdapter : BaseAdapter<ListViewItem>
	{
		public List<Transaction> SelectedItems { get; set; }
		public event Action<bool, Transaction> AddRemove = delegate { };
		public Func<Transaction, bool> IsInList;
		public Func<bool> AtMaximumTransactions;
		public string MaxSelectedTransactionsMessage;
		private Activity _activity;
		private List<ListViewItem> _list;
		private int _listViewResourceId;
		private int _textViewResourceId;
		private int _textView2ResourceId;
		private int _textView3ResourceId;
		private int _checkBoxResourceId;

		public TransactionsSelectionListViewAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int textViewResourceId, int textView2ResourceId, int textView3ResourceId, int checkBoxResourceId)
		{
			_activity = activity;
			_list = list;
			_listViewResourceId = listViewResourceId;
			_textViewResourceId = textViewResourceId;
			_textView2ResourceId = textView2ResourceId;
			_textView3ResourceId = textView3ResourceId;
			_checkBoxResourceId = checkBoxResourceId;
			SelectedItems = new List<Transaction>();
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
			View row = null;

			row = _activity.LayoutInflater.Inflate(_listViewResourceId, null);

			try
			{
				var imageMore = row.FindViewById<ImageView>(Resource.Id.imgMore);
				var imageCheck = row.FindViewById<ImageView>(Resource.Id.imgCheck);

				if (imageMore != null)
					imageMore.SetImageResource(Resource.Color.transparent);
				if (imageCheck != null)
					imageCheck.SetImageResource(Resource.Color.transparent);
				
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			var textView = row.FindViewById<TextView>(_textViewResourceId);

			if (textView != null)
			{
				textView.Text = _list[position].HeaderText;
			}

			var textView2 = row.FindViewById<TextView>(_textView2ResourceId);

			if (textView2 != null)
			{
				textView2.Text = _list[position].Item1Text;
			}
			var textView3 = row.FindViewById<TextView>(_textView3ResourceId);

			if (textView3 != null)
			{
				textView3.Text = _list[position].Value1Text;
				var amount = decimal.Parse(StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(textView3.Text)));
				textView3.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(_activity, amount < 0 ? Resource.Color.Red : Resource.Color.black)));
			}

			var checkBox = row.FindViewById<CheckBox>(_checkBoxResourceId);

			if (checkBox != null)
			{
				checkBox.Tag = JsonConvert.SerializeObject(_list[position].Data) + "|" + position;
				checkBox.CheckedChange -= CheckBoxChanged;
				checkBox.Checked = (_list[position].IsChecked || IsInList((Transaction)_list[position].Data));
				checkBox.CheckedChange += CheckBoxChanged;
			}

			return row;
		}

		private void CheckBoxChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			var json = ((string)((CheckBox)sender).Tag).Split('|')[0];
			var transactionTag = JsonConvert.DeserializeObject<Transaction>(json);

			if (((CheckBox)sender).Checked)
			{
				var doAdd = 0;

				for (int i = 0; i < SelectedItems.Count; i++)
				{
					if (SelectedItems[i] == transactionTag)
					{
						doAdd++;
					}
				}

				if (doAdd == 0)
				{
					if (!AtMaximumTransactions())
					{
						SelectedItems.Add(transactionTag);
						var pos = int.Parse(((string)((CheckBox)sender).Tag).Split('|')[1]);
						_list[pos].IsChecked = true;
						AddRemove(true, transactionTag);
					}
					else
					{
						((CheckBox)sender).CheckedChange -= CheckBoxChanged;
						((CheckBox)sender).Checked = false;
						((CheckBox)sender).CheckedChange += CheckBoxChanged;

						AlertMethods.Alert(_activity, "Transaction Selection", MaxSelectedTransactionsMessage, "OK");
					}
				}
			}
			else
			{
				for (int i = 0; i<SelectedItems.Count; i++)
				{
					if (SelectedItems[i] == transactionTag)
					{
						SelectedItems.RemoveAt(i);
					}
				}

				var pos = int.Parse(((string)((CheckBox)sender).Tag).Split('|')[1]);
				_list[pos].IsChecked = false;

				AddRemove(false, transactionTag);
			}
		}
	}
}