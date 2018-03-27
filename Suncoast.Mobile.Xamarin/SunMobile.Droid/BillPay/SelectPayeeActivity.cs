using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunMobile.Droid.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;
using SunBlock.DataTransferObjects;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.BillPay.V2;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "SelectPayeeActivity", Theme = "@style/CustomHoloLightTheme")]
	public class SelectPayeeActivity : BaseListActivity
	{
		private TextView txtTitle;
		private ImageButton btnCloseWindow;
		private SwipeRefreshLayout refresher;
		private StatusResponse<List<Payee>> _payees;

		private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("payees");
                _payees = JsonConvert.DeserializeObject<StatusResponse<List<Payee>>>(json);
			}

			SetupView();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			string json = JsonConvert.SerializeObject(_payees);
			outState.PutString("payees", json);

			base.OnSaveInstanceState(outState);
		}

		public override void SetupView()
		{
			base.SetupView(Resource.Layout.BillPayPayeeListView);

			refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.payeeListViewRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += (sender, e) =>
			{
				_payees = null;
				LoadPayees();
			};

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            txtTitle.Text = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "140EE20C-3240-428A-8AA5-4D7CEAC739E3", "Select a Payee");
			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			LoadPayees();
		}

		private async void LoadPayees()
		{
			var methods = new BillPayMethods();			

			if (_payees == null)
			{
				if (!refresher.Refreshing)
				{
					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText(cultureViewId, "FE18B185-E8E8-452A-9EDA-E6F9055E5898", "Loading payees..."));
				}

                _payees = await methods.GetPayees(null, this);
			}

			if (!refresher.Refreshing)
			{
				HideActivityIndicator();
			}
			else
			{
				refresher.Refreshing = false;
			}

            if (_payees?.Result != null)
			{
                var tableViewSource = ViewUtilities.ConvertPayeeV2ListTextViewTableSource(_payees.Result, true);

                if (tableViewSource != null)
                {
                    int[] resourceIds = { Resource.Id.lblHeaderText, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text };
                    string[] fields = { "HeaderText", "Header2Text", "Value1Text" };
                    var listAdapter = new GenericListAdapter(this, Resource.Layout.BillPayPayeeListViewItem, tableViewSource.Items, resourceIds, fields);
                    ListAdapter = listAdapter;
                }
			}
		}

		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);

			try
			{
				var listViewItem = ((GenericListAdapter)l.Adapter).GetListViewItem(position);

				var intent = new Intent();
				intent.PutExtra("ClassName", "SelectPayeeActivity");
				var json = JsonConvert.SerializeObject(listViewItem.Data);
				intent.PutExtra("Payee", json);
				listViewItem.Data = null;
				json = JsonConvert.SerializeObject(listViewItem);
				intent.PutExtra("ListViewItem", json);
				SetResult(Result.Ok, intent);
				Finish();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectPayeeActivity:OnListItemClick");
			}
		}
	}
}