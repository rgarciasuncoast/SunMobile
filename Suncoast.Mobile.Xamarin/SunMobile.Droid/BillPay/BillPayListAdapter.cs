using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.Droid.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.BillPay
{
    public class BillPayListAdapter : GenericListAdapter
    {
        public BillPayListAdapter(Activity activity, int listViewResourceId, List<ListViewItem> list, int[] textViewResourceIds, string[] classFields) : base(activity, listViewResourceId, list, textViewResourceIds, classFields)
        {
        }

        protected override void PopulateGenericViews(View row, object listItem)
        {
            base.PopulateGenericViews(row, listItem);

            try
            {
                TextView tv = null;

                for (int i = 0; i < _textViewResourceIds.Length; i++)
                {
                    tv = (TextView)row.FindViewById(_textViewResourceIds[i]);
                    tv.Text = Reflector(listItem, _classFields[i]);
                    tv.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(_activity, Resource.Color.TextViewTextColor)));

                    // Bill Pay Status
                    if (_textViewResourceIds[i] == Resource.Id.lblItem2Text && ((Payment)((ListViewItem)listItem).Data).StatusCode == 103)
                    {
                        tv.SetTextColor(Android.Graphics.Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPayListAdapter:PopulateGenericView");
            }
        }
    }
}