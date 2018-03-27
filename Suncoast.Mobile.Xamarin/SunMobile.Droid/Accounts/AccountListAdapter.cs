using System;
using System.Collections.Generic;
using System.Reflection;
using Android.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts
{
	public class AccountListAdapter : BaseAdapter<ListViewItem>
	{
		private Activity _activity;
		private List<ListViewItem> _list;
		private int[] _textViewResourceIds;
		private string[] _classFields;

		public AccountListAdapter(Activity activity, List<ListViewItem> list, int[] textViewResourceIds, string[] classFields) 
		{
			_activity = activity;
			_list = list;
			_textViewResourceIds = textViewResourceIds;
			_classFields = classFields;
		}

		public override int Count
		{
			get { return _list.Count; }
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

			object listItem = _list[position];

			if (string.IsNullOrEmpty(_list[position].Item3Text)) 
			{
				row = _activity.LayoutInflater.Inflate(Resource.Layout.AccountListItem2Rows, null);
			} 
			else 
			{
				row = _activity.LayoutInflater.Inflate(Resource.Layout.AccountListItem4Rows, null);
			}				

			try 
			{
				var imageMore = row.FindViewById<ImageView>(Resource.Id.imgMore);

				if (!_list[position].MoreIconVisible)
				{
					imageMore.Visibility = ViewStates.Gone;
				}
				else
				{
					imageMore.Visibility = ViewStates.Visible;
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			// Set background color based on ownership type
			try 
			{
				var account = (Account)_list[position].Data;
				var tableLayout = row.FindViewById<TableLayout>(Resource.Id.tableLayout);

				switch (account.OwnershipType)
				{
					case "Primary":						
						tableLayout.SetBackgroundColor(AppStyles.RegularAccountsBackgroundColor);
						break;
					case "Secondary":
						tableLayout.SetBackgroundColor(AppStyles.SecondaryAccountsBackgroundColor);
						break;
					case "Joint":
						tableLayout.SetBackgroundColor(AppStyles.JointAccountsBackgroundColor);
						break;
					default:
						tableLayout.SetBackgroundColor(AppStyles.RegularAccountsBackgroundColor);
						break;
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			TextView tv = null;

			for (int i = 0; i < _textViewResourceIds.Length; i++)
			{
				tv = (TextView)row.FindViewById(_textViewResourceIds[i]);

				if (tv != null)
				{					
					tv.Text = Reflector(listItem, _classFields[i]);

					tv.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(_activity, Resource.Color.TextViewTextColor)));

					if (_classFields[i] == "Value1Text" || _classFields[i] == "Value2Text" || _classFields[i] == "Value3Text" || _classFields[i] == "Value4Text")
					{
						var amount = StringUtilities.StripInvalidCurrencyChars(tv.Text);

						decimal result;
						decimal.TryParse(amount, out result);
						if (result < 0)
						{
							tv.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(_activity, Resource.Color.TextViewTextColorRed)));
						}
					}
				}				
			}

			return row;
		}

		public string Reflector(object obj, string fieldPath)
		{
			string returnValue = string.Empty;
			object topObject = obj;

			Type type = topObject.GetType();

			PropertyInfo[] properties = type.GetProperties();

			foreach (var property in properties)
			{
				var shortPropertyName = property.Name.Substring(property.Name.IndexOf(" ", StringComparison.Ordinal) + 1, property.Name.Length - (property.Name.IndexOf(" ", StringComparison.Ordinal) + 1));

				if (shortPropertyName == fieldPath)
				{
					returnValue = property.GetValue(topObject).ToString();
					break;
				}
			}

			return returnValue;
		}
	}
}