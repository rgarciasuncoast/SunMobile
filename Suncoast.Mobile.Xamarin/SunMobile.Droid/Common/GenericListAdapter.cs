using System;
using System.Collections.Generic;
using System.Reflection;
using Android.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.Shared.Logging;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Common
{
	public class GenericListAdapter : BaseAdapter<ListViewItem>
	{
		protected Activity _activity;
		protected List<ListViewItem> _list;
		protected int _listViewResourceId;
		protected int[] _textViewResourceIds;
		protected string[] _classFields;

		public GenericListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int[] textViewResourceIds, string[] classFields)
		{
			_activity = activity;
			_list = list;
			_listViewResourceId = listViewResourceId;
			_textViewResourceIds = textViewResourceIds;
			_classFields = classFields;
		}

		public override int Count
		{
			get { return _list.Count; }
		}

		public ListViewItem GetListViewItem(int position)
		{
			return _list [position];
		}

		public override ListViewItem this[int position]
		{
			get { return _list [position]; }
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

			if (convertView == null)
			{
				row = _activity.LayoutInflater.Inflate(_listViewResourceId, null);
			}
			else 
			{
				row = convertView;
			}

			object listItem = _list[position];

			try
			{
				var imageMore = row.FindViewById<ImageView>(Resource.Id.imgMore);
				var imageCheck = row.FindViewById<ImageView>(Resource.Id.imgCheck);

				// Can't hide image holder because it loses its width
				if (!_list [position].MoreIconVisible)
				{
					if (imageMore != null)
						imageMore.SetImageResource(Resource.Color.transparent);
					if (imageCheck != null)
						imageCheck.SetImageResource(Resource.Color.transparent);
				}
				else 
				{
					if (imageMore != null)
						imageMore.SetImageResource(Resource.Drawable.listitemselect);
					if (imageCheck != null)
						imageCheck.SetImageResource(Resource.Drawable.transactioncheck);
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			try
			{
				var imageHeader = (ImageView)row.FindViewById(Resource.Id.imgHeader);

				if (!string.IsNullOrEmpty(_list [position].ImageName))
				{
					var resourceName = _list [position].ImageName;
					int pngIndex = resourceName.IndexOf(".png", StringComparison.Ordinal);

					if (pngIndex > 0)
					{
						resourceName = resourceName.Substring(0, pngIndex);
					}

					var resourceId = (int)typeof(Resource.Drawable).GetField(resourceName).GetValue(null);
					imageHeader.SetImageResource(resourceId);
				}
				else 
				{
					imageHeader.SetImageResource(0);
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			PopulateGenericViews(row, listItem);

			return row;
		}

        protected virtual void PopulateGenericViews(View row, object listItem)
		{
			try
			{
				TextView tv = null;

				for (int i = 0; i < _textViewResourceIds.Length; i++)
				{
					tv = (TextView)row.FindViewById(_textViewResourceIds[i]);
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
			catch (Exception ex)
			{
				Logging.Log(ex, "GenericListAdapter:PopulateGenericView");
			}
		}

		public string Reflector(object obj, string fieldPath)
		{
			string returnValue = string.Empty;
			object topObject = obj;

			Type type = topObject.GetType();

			PropertyInfo [] properties = type.GetProperties();

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