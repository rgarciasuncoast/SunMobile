using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Droid.BillPay;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Culture;

namespace SunMobile.Droid
{
	public class BillPayMenuFragment : BaseFragment
	{
		private TableRow _tableRowSchedule;
		private TableRow _tableRowPendingPayments;
		private TableRow _tableRowViewHistory;
		private TableRow _tableRowPayees;
		private bool _gotBillPayEnrollment;

		private TextView lblSchedulePayment;
		private TextView lblPendingPayments;
		private TextView lblViewHistory;
		private TextView lblManagePayees;

		private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.billpaymenuview, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();
		}

		public override void SetupView()
		{
			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Bill Pay");

			_tableRowSchedule = Activity.FindViewById<TableRow>(Resource.Id.rowSchedule);
			_tableRowSchedule.Click += (sender, e) => ListItemClicked(0);
			_tableRowPendingPayments = Activity.FindViewById<TableRow>(Resource.Id.rowPending);
			_tableRowPendingPayments.Click += (sender, e) => ListItemClicked(1);
			_tableRowViewHistory = Activity.FindViewById<TableRow>(Resource.Id.rowHistory);
			_tableRowViewHistory.Click += (sender, e) => ListItemClicked(2);
			_tableRowPayees = Activity.FindViewById<TableRow>(Resource.Id.rowPayees);
			_tableRowPayees.Click += (sender, e) => ListItemClicked(3);

			lblSchedulePayment = Activity.FindViewById<TextView>(Resource.Id.lblSchedulePayment);
			lblPendingPayments = Activity.FindViewById<TextView>(Resource.Id.lblPendingPayments);
			lblViewHistory = Activity.FindViewById<TextView>(Resource.Id.lblViewHistory);
			lblManagePayees = Activity.FindViewById<TextView>(Resource.Id.lblManagePayees);

			if (!_gotBillPayEnrollment)
			{
				_gotBillPayEnrollment = true;
				GetBillPayEnrollment();
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

                CultureTextProvider.SetMobileResourceText(lblSchedulePayment, cultureViewId, "94F29FF8-C65F-49DF-81E1-15FAD7B34BFB");
                CultureTextProvider.SetMobileResourceText(lblPendingPayments, cultureViewId, "8B782F41-D7E5-4952-8D99-CB7DB932939A");
                CultureTextProvider.SetMobileResourceText(lblViewHistory, cultureViewId, "BE706CA5-0A94-48B9-8B7C-8853740AB0AB");
                CultureTextProvider.SetMobileResourceText(lblManagePayees, cultureViewId, "5DA51339-4D43-4A6E-ADB7-0616BB5CCB77");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayMenuFragment:SetCultureConfiguration");
			}
		}

		private async void GetBillPayEnrollment()
		{
			try
			{	
				ShowActivityIndicator();

				var methods = new BillPayMethods();
				var response = await methods.IsMemberEnrolledInBillPay(null, Activity);

				HideActivityIndicator();

				if (response?.IsMemberEnrolledInBillPay == false)
				{
                    var bpaHeader = CultureTextProvider.GetMobileResourceText(cultureViewId, "91890DFB-1A8D-4D21-9546-C97545C739F8", "Bill Pay Agreement");
                    var agreeText = CultureTextProvider.GetMobileResourceText(cultureViewId, "6DF6EB75-22D3-4FF2-B2D5-1EE7CBCC8B79", "I Agree / Enroll");
                    var emailText = CultureTextProvider.GetMobileResourceText(cultureViewId, "0F03A750-3B15-4FB5-8BD9-B292FAC75535", "Email");
                    var cancelText = CultureTextProvider.GetMobileResourceText(cultureViewId, "B3A73D02-2DAF-433D-A266-EA46DA6479D6", "Cancel");
					var agreementText = response.BillPayEnrollmentAgreementText.Replace("\\n", "\n");
					var agreementResponse = await AlertMethods.Alert(Activity, bpaHeader, agreementText, agreeText, emailText, cancelText);

					if (agreementResponse == agreeText)
					{
						var memberBillPayEnrollementRequest = new SunBlock.DataTransferObjects.BillPay.MemberBillPayEnrollementRequest
                        {
							IsUnEnroll = false,
							MemberId = GeneralUtilities.GetMemberIdAsInt()
						};

						ShowActivityIndicator();

						await methods.UpdateBillPayEnrollment(memberBillPayEnrollementRequest, Activity);

						HideActivityIndicator();
					}
					else if (agreementResponse == emailText)
					{
						GeneralUtilities.DisableView((ViewGroup)View, true);
						GeneralUtilities.SendEmail(Activity, null, bpaHeader, agreementText, false, true);
					}
					else
					{
						GeneralUtilities.DisableView((ViewGroup)View, true);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ClassicBillPayMenuFragment:GetBillPayEnrollment");
			}
		}

		public void ListItemClicked(int position)
		{
			Android.Support.V4.App.Fragment fragment = null;

			switch (position)
			{
				case 0:
					fragment = new BillPaySchedulePaymentFragment();
					((BillPaySchedulePaymentFragment)fragment).PaymentScheduled += isPending =>
					{
						var billPayFragment = new BillPayFragment();
						billPayFragment.StartWithPending = isPending;

						NavigationService.NavigatePush(billPayFragment, true, false);
					};
					break;
				case 1:
					fragment = new BillPayFragment();
					((BillPayFragment)fragment).StartWithPending = true;
					break;
				case 2:
					fragment = new BillPayFragment();
					((BillPayFragment)fragment).StartWithPending = false;
					break;
				case 3:
					fragment = new ManagePayeesFragment();
					break;
			}

			if (fragment != null)
			{
				NavigationService.NavigatePush(fragment, true, false);
			}
		}
	}
}