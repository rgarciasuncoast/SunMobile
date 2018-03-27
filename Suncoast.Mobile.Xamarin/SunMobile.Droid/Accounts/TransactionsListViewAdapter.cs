using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Common
{
	public class TransactionsListViewAdapter : GenericListAdapter
	{
		public event Action<ListViewItem> ViewCheckSelected = delegate {};
		public event Action<ListViewItem> DisputeSelected = delegate {};
		private SwipeLayout _swipeLayout;
		private string _memberId;

		public TransactionsListViewAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int[] textViewResourceIds, string[] classFields, string memberId)
		: base(activity, listViewResourceId, list, textViewResourceIds, classFields)
		{
			_memberId = memberId;
		}

        public void AddItems(List<ListViewItem> list)
        {
            _list.AddRange(list);
        }

		public void ReplaceItems(List<ListViewItem> list)
		{
            _list = list;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = null;

			row = _activity.LayoutInflater.Inflate(_listViewResourceId, null);

			object listItem = _list[position];
			var transaction = (Transaction)_list[position].Data;

			try
			{
				_swipeLayout = row.FindViewById<SwipeLayout>(Resource.Id.swipe);
				var imageMore = row.FindViewById<ImageView>(Resource.Id.imgMore);
				var imageCheck = row.FindViewById<ImageView>(Resource.Id.imgCheck);

				if (imageMore != null)
					imageMore.SetImageResource(Resource.Drawable.listitemselect);

                // Can't hide image holder because it loses its width
                if (!_list[position].MoreIconVisible)
                {
                    if (imageCheck != null)
                    {
                        imageCheck.SetImageResource(Resource.Color.transparent);
                    }
                }
                else
                {
                    if (imageCheck != null)
                    {
                        imageCheck.SetImageResource(Resource.Drawable.transactioncheck);
                    }
				}
			}			
			catch {}

			try
			{
				var btnViewCheck = row.FindViewById<Button>(Resource.Id.btnViewCheck);
                CultureTextProvider.SetMobileResourceText(btnViewCheck, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "CB26E0E2-30CC-4225-9936-32ECCA4D67A0", "View Check");				

				btnViewCheck.Click += (sender, e) =>
				{
					_swipeLayout.Close(true);
					ViewCheckSelected(_list[position]);
				};

				if (!_list[position].MoreIconVisible)
				{
					btnViewCheck.Visibility = ViewStates.Gone;
				}
				else
				{
					btnViewCheck.Visibility = ViewStates.Visible;
				}

				var btnDispute = row.FindViewById<Button>(Resource.Id.btnDispute);
                CultureTextProvider.SetMobileResourceText(btnDispute, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "6E08242D-1C53-426F-98F3-957E59BF4F54", "Dispute");

				btnDispute.Click += (sender, e) =>
				{
					_swipeLayout.Close(true);
					DisputeSelected(_list[position]);
				};

				var accountMethods = new AccountMethods();

				if (accountMethods.GetDisputeInfo(transaction, _memberId, _list[position].Item5Text == "CreditCard").AllowDispute)
				{				
					btnDispute.Visibility = ViewStates.Visible;
				}
				else
				{
					btnDispute.Visibility = ViewStates.Gone;
				}

				if (btnDispute.Visibility == ViewStates.Gone && btnViewCheck.Visibility == ViewStates.Gone)
				{
					_swipeLayout.SwipeEnabled = false;
				}
			}
			catch {}

			TextView tv = null;

			for (int i = 0; i<_textViewResourceIds.Length; i++)
			{
				tv = (TextView)row.FindViewById(_textViewResourceIds[i]);
				tv.Text = Reflector(listItem, _classFields[i]);
				tv.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(_activity, Resource.Color.TextViewTextColor)));

				if (_classFields[i] == "HeaderText")
                {
                    tv.SetTextColor(transaction.IsPending ? Android.Graphics.Color.ParseColor("#1b506e") : Android.Graphics.Color.Black);

                    if (transaction.IsPending)
                    {
                        tv.Text = "Pending";  
                        CultureTextProvider.SetMobileResourceText(tv, "D335A6E9-8ADB-4C29-912D-625090EAD031", "4FCFEB18-9278-4B44-A75E-651F1D68258E", "Pending");
                    }
		        }

				if (_classFields[i] == "Value1Text")
				{
					var amount = StringUtilities.StripInvalidCurrencyChars(tv.Text);

					decimal result;
					decimal.TryParse(amount, out result);
					if (result < 0)
					{
						tv.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(_activity, Resource.Color.TextViewTextColorRed)));
					}
					else
					{
						tv.SetTextColor(Android.Graphics.Color.ParseColor("#1b506e"));
					}
				}

				if (_classFields[i] == "Value2Text")
				{
					tv.SetTextColor(Android.Graphics.Color.ParseColor("#968b88"));
				}				
			}

            // Pending items
            row.SetBackgroundColor(transaction.IsPending ? Android.Graphics.Color.ParseColor("#f1faff") : Android.Graphics.Color.White);		
            var rowTableLayout = row.FindViewById<TableLayout>(Resource.Id.rowTableLayout);

            if (rowTableLayout != null)
            {
				rowTableLayout.SetBackgroundColor(transaction.IsPending ? Android.Graphics.Color.ParseColor("#f1faff") : Android.Graphics.Color.White);      
			}

			return row;
		}	
	}
}