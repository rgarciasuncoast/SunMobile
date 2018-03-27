using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using SunMobile.Droid.Common;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunBlock.DataTransferObjects.BillPay.V2;

namespace SunMobile.Droid.BillPay
{
	public class BillPayFragment : BaseListFragment
	{	
        private RadioButton btnPending;
		private RadioButton btnHistory;		
		private SwipeRefreshLayout refresher;		

        public bool StartWithPending { get; set; }
        private GenericListAdapter _listAdapter;
        List<ListViewItem> _listViewItems;

		private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";				

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.BillPayListView, null);
			RetainInstance = true;			

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();
		}

		protected override void SetupView()
		{
			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Payments");

			refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.billPayRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += RefreshList;						

			btnPending = Activity.FindViewById<RadioButton>(Resource.Id.btnPending);
			btnPending.Click += RefreshList;

			btnHistory = Activity.FindViewById<RadioButton>(Resource.Id.btnHistory);
			btnHistory.Click += RefreshList;						

			ListViewMain.ItemClick += (sender, e) =>
			{
				var item = _listAdapter.GetListViewItem(e.Position);
				var payment = (Payment)item.Data;

                var billPayDetailsFragment = new BillPayDetailsFragment();
                billPayDetailsFragment.CurrentPayment = payment;
                billPayDetailsFragment.IsPending = StartWithPending;

				billPayDetailsFragment.PaymentChanged += isPending =>
				{
					StartWithPending = isPending;
                    RefreshList(null, null);
				};				

				NavigationService.NavigatePush(billPayDetailsFragment, true, false);
			};

            if (StartWithPending)
			{
				RefreshPending();
			}
			else
			{
				RefreshHistory();
			}			
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");

                if (!string.IsNullOrEmpty(viewText))
                {
                    ((MainActivity)Activity).SetActionBarTitle(viewText);
                }

                CultureTextProvider.SetMobileResourceText(btnPending, cultureViewId, "4FCFEB18-9278-4B44-A75E-651F1D68258E");
                CultureTextProvider.SetMobileResourceText(btnHistory, cultureViewId, "C60D552C-A5D5-4017-8FDC-8FDF441F39BC");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayFragment:SetCultureConfiguration");
			}
		}

		private void RefreshList(object sender, EventArgs e)
		{
			// Clear the table
			PopulateListView(null);

			if (sender == btnPending)
			{
				StartWithPending = true;				
			}

			if (sender == btnHistory)
			{
				StartWithPending = false;				
			}

			_listViewItems = null;

            if (StartWithPending)
			{
				RefreshPending();
			}
			else
			{
				RefreshHistory();
			}
		}

		private async void RefreshHistory()
		{
            try
            {
                var endDate = DateTime.Now.AddDays(1);
                var startDate = endDate.AddYears(-2);

                var methods = new BillPayMethods();

                var request = new PaymentSearchRequest
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                if (_listViewItems == null)
                {
                    if (!refresher.Refreshing)
                    {
                        ShowActivityIndicator();
                    }

                    var response = await methods.GetHistoryPayments(request, Activity);

                    if (!refresher.Refreshing)
                    {
                        HideActivityIndicator();
                    }
                    else
                    {
                        refresher.Refreshing = false;
                    }

                    if (response?.Result != null)
                    {
                        PopulateListView(response.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPayFragment:RefreshHistory");
            }
		}

		private async void RefreshPending()
		{
            try
            {
                var methods = new BillPayMethods();

                if (_listViewItems == null)
                {
                    if (!refresher.Refreshing)
                    {
                        ShowActivityIndicator(CultureTextProvider.GetMobileResourceText(cultureViewId, "BA6CCD3F-C4DF-4DCB-91C8-C3582B134230", "Loading payments..."));
                    }

                    var response = await methods.GetPendingPayments(null, Activity);

                    if (!refresher.Refreshing)
                    {
                        HideActivityIndicator();
                    }
                    else
                    {
                        refresher.Refreshing = false;
                    }

                    if (response?.Result != null)
                    {
                        PopulateListView(response.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPayFragment:RefreshPending");
            }
		}

		private void PopulateListView(List<Payment> payments)
		{
            _listViewItems = ViewUtilities.ConvertPaymentsToListViewItems(payments, StartWithPending);

			int[] resourceIds = { Resource.Id.lblHeaderText, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text, Resource.Id.lblValue2Text };
			string[] fields = { "HeaderText", "Item1Text", "Item2Text", "Value2Text" };
            _listAdapter = new BillPayListAdapter(Activity, Resource.Layout.BillPayListViewItem, _listViewItems, resourceIds, fields);
			ListAdapter = _listAdapter;			

            // Putting this here because it is not working in SetupView
            btnPending.Checked = StartWithPending;
            btnHistory.Checked = !StartWithPending;
		}
	}
}