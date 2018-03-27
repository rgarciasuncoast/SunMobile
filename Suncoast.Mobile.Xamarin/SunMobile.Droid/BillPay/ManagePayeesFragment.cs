using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.Droid.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;

namespace SunMobile.Droid.BillPay
{
	[Activity(Label = "BillPayPayees", Theme = "@style/CustomHoloLightTheme")]
	public class ManagePayeesFragment : BaseListFragment
	{
		private StatusResponse<List<Payee>> _payeeViewModel;
		private SwipeRefreshLayout payeeRefresher;
		private SegmentedGroup segmentPayeeType;
		private RadioButton btnActivePayees;
		private RadioButton btnInactivePayees;
		private TextView btnAddAPayee;

		private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.BillPayManagePayees, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			if (_payeeViewModel != null)
			{
				var json = JsonConvert.SerializeObject(_payeeViewModel);
				outState.PutString("PayeeViewModel", json);
			}

			if (segmentPayeeType.CheckedRadioButtonId == btnActivePayees.Id)
			{
				outState.PutString("PayeeType", "Active");
			}
			else
			{
				outState.PutString("PayeeType", "Inactive");
			}

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			payeeRefresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.payeesListRefresher);
			payeeRefresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			payeeRefresher.Refresh += (sender, e) => Refresh();

			btnActivePayees = Activity.FindViewById<RadioButton>(Resource.Id.btnPayeeActive);
			btnActivePayees.Click += ActiveChanged;

			btnInactivePayees = Activity.FindViewById<RadioButton>(Resource.Id.btnPayeeInactive);
			btnInactivePayees.Click += ActiveChanged;

			segmentPayeeType = Activity.FindViewById<SegmentedGroup>(Resource.Id.segmentPayeeType);

			btnAddAPayee = Activity.FindViewById<TextView>(Resource.Id.txtAddPayee);
			btnAddAPayee.Click += (sender, e) => AddAPayee();

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("PayeeViewModel");
				_payeeViewModel = JsonConvert.DeserializeObject<StatusResponse<List<Payee>>>(json);

				if (savedInstanceState.GetString("PayeeType") == "Active")
				{
					LoadPayees(true);
				}
				else
				{
					LoadPayees(false);
				}
			}
			else
			{
				LoadPayees(true);
			}
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "5DA51339-4D43-4A6E-ADB7-0616BB5CCB77", "Manage Payees");

                if (!string.IsNullOrEmpty(viewText))
                {
                    ((MainActivity)Activity).SetActionBarTitle(viewText);
                }

                CultureTextProvider.SetMobileResourceText(btnActivePayees, cultureViewId, "65BF6355-7D3B-4D4E-A0F1-D9EA155B503F");
                CultureTextProvider.SetMobileResourceText(btnInactivePayees, cultureViewId, "B44F5732-E9CE-40AF-AB30-6B40EAD0CDCE");
                CultureTextProvider.SetMobileResourceText(btnAddAPayee, cultureViewId, "5B79EC78-2D23-4E79-B85E-42EA2E16A1E3");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "ManagePayeesFragment:SetCultureConfiguration");
			}
		}

		private void AddAPayee()
		{
			var intent = new Intent(Activity, typeof(UpdatePayeeActivity));

			StartActivityForResult(intent, 100);
		}

		private void Refresh()
		{
			_payeeViewModel = null;

			LoadPayees(segmentPayeeType.CheckedRadioButtonId == btnActivePayees.Id);
		}

		private void ActiveChanged(object sender, EventArgs e)
		{
			LoadPayees(segmentPayeeType.CheckedRadioButtonId == btnActivePayees.Id);
		}

		private async void LoadPayees(bool isActive)
		{
			var methods = new BillPayMethods();

			var request = new GetPayeesRequest
			{
				MemberId = GeneralUtilities.GetMemberIdAsInt()
			};

			if (_payeeViewModel == null)
			{
				if (!payeeRefresher.Refreshing)
				{
					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText(cultureViewId, "FE18B185-E8E8-452A-9EDA-E6F9055E5898", "Loading payees..."));
				}

				_payeeViewModel = await methods.GetPayees(request, this);
			}

			if (!payeeRefresher.Refreshing)
			{
				HideActivityIndicator();
			}
			else
			{
				payeeRefresher.Refreshing = false;
			}

			if (_payeeViewModel != null && _payeeViewModel.Success)
			{
				var tableViewSource = ViewUtilities.ConvertPayeeV2ListTextViewTableSource(_payeeViewModel.Result, isActive);

				if (tableViewSource != null)
				{
					int[] resourceIds = { Resource.Id.lblHeaderText, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text };
					string[] fields = { "HeaderText", "Header2Text", "Value1Text" };
					var listAdapter = new GenericListAdapter(Activity, Resource.Layout.BillPayPayeeListViewItem, tableViewSource.Items, resourceIds, fields);
					ListAdapter = listAdapter;
				}
			}
		}

		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);

			try
			{
				var listViewItem = ((GenericListAdapter)l.Adapter).GetListViewItem(position).Data;
				var intent = new Intent(Activity, typeof(UpdatePayeeActivity));

				var json = JsonConvert.SerializeObject(listViewItem);
				intent.PutExtra("Payee", json);
				listViewItem = null;
				StartActivityForResult(intent, 0);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayPayees:OnListItemClick");
			}
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == (int)Result.Ok && data != null)
			{
				if (data.GetBooleanExtra("Refresh", false))
				{
					Refresh();
				}
			}
		}
	}
}