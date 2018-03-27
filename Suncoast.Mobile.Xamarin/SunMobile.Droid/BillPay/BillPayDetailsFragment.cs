using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Culture;
using SunBlock.DataTransferObjects.BillPay.V2;

namespace SunMobile.Droid.BillPay
{
	public class BillPayDetailsFragment : BaseFragment
	{
		private TextView txtPayee;
		private TextView txtAccount;
		private TextView txtAmount;
		private TextView txtDeliverBy;
		private TextView txtFromAccount;
		private TextView txtFrequency;
        private TextView txtReferenceID;
		private TextView txtStatus;
		private TableRow rowButtons;
		private Button btnCancel;
		private Button btnUpdate;

		private TextView lblPayee;
		private TextView lblAccount;
		private TextView lblAmount;
		private TextView lblFrequency;
		private TextView lblDeliverBy;
		private TextView lblFromAccount;
        private TextView lblReferenceID;
		private TextView lblStatus;

		private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

		public event Action<bool> PaymentChanged = delegate { };
        public Payment CurrentPayment { get; set; }
        public bool IsPending { get; set; }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.BillPayDetailsView, null);
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

			txtPayee = Activity.FindViewById<TextView>(Resource.Id.txtPayee);
			txtAccount = Activity.FindViewById<TextView>(Resource.Id.txtAccount);
			txtAmount = Activity.FindViewById<TextView>(Resource.Id.txtAmount);
            txtDeliverBy = Activity.FindViewById<TextView>(Resource.Id.txtDeliverBy);
			txtFrequency = Activity.FindViewById<TextView>(Resource.Id.txtFrequency);
			txtFromAccount = Activity.FindViewById<TextView>(Resource.Id.txtFromAccount);
            txtReferenceID = Activity.FindViewById<TextView>(Resource.Id.txtReferenceID);
			txtStatus = Activity.FindViewById<TextView>(Resource.Id.txtStatus);
			rowButtons = Activity.FindViewById<TableRow>(Resource.Id.rowButtons);

			btnCancel = Activity.FindViewById<Button>(Resource.Id.btnCancelPayment);
			btnUpdate = Activity.FindViewById<Button>(Resource.Id.btnUpdatePayment);

			txtPayee.Text = CurrentPayment.PayeeName;
			txtAccount.Text = CurrentPayment.PayeeAccountNumber;
			txtAmount.Text = StringUtilities.FormatAsCurrency(CurrentPayment.Amount.ToString());
            txtDeliverBy.Text = string.Format("{0:MM/dd/yyyy}", CurrentPayment.DeliverBy);
            txtFrequency.Text = CurrentPayment.Frequency;      
            txtFromAccount.Text = CurrentPayment.SourceAccount;
            txtReferenceID.Text = CurrentPayment.PaymentId.ToString();

			var textPending = CultureTextProvider.GetMobileResourceText(cultureViewId, "4FCFEB18-9278-4B44-A75E-651F1D68258E", "Pending");

            txtStatus.Text = CurrentPayment.Status;

			lblPayee = Activity.FindViewById<TextView>(Resource.Id.lblPayee);
			lblAccount = Activity.FindViewById<TextView>(Resource.Id.lblAccount);
			lblAmount = Activity.FindViewById<TextView>(Resource.Id.lblAmount);
			lblFrequency = Activity.FindViewById<TextView>(Resource.Id.lblFrequency);
            lblDeliverBy = Activity.FindViewById<TextView>(Resource.Id.lblDeliverBy);
			lblFromAccount = Activity.FindViewById<TextView>(Resource.Id.lblFromAccount);
            lblReferenceID = Activity.FindViewById<TextView>(Resource.Id.lblReferenceID);
			lblStatus = Activity.FindViewById<TextView>(Resource.Id.lblStatus);

            var methods = new BillPayMethods();

            if (!methods.CanPaymentBeEditedOrCanceled(CurrentPayment))
            {
                rowButtons.Visibility = ViewStates.Gone;
            }

			btnCancel.Click += (sender, e) => CancelPayment();
			btnUpdate.Click += (sender, e) => UpdatePayment();
		}

		public override void SetCultureConfiguration()
		{
            try
            {
				var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "CF4993E9-71F4-4D2A-902A-B90AB4D5AAD6");

				if (!string.IsNullOrEmpty(viewText))
				{
					((MainActivity)Activity).SetActionBarTitle(viewText);
				}

                CultureTextProvider.SetMobileResourceText(lblPayee, cultureViewId, "4B165436-E53E-4F50-B2FA-31894B1A1F92");
                CultureTextProvider.SetMobileResourceText(lblAccount, cultureViewId, "140832FD-8C1E-47FF-87A9-54F2C35B082E");
                CultureTextProvider.SetMobileResourceText(lblAmount, cultureViewId, "012710F9-07D8-4DDA-B149-EFFF47306668");
                CultureTextProvider.SetMobileResourceText(lblFrequency, cultureViewId, "2F73A49C-84B9-4D0C-965B-1498DAC1B253");
                CultureTextProvider.SetMobileResourceText(lblDeliverBy, cultureViewId, "59444AF7-1E8A-4083-977D-41B6806282BF");
                CultureTextProvider.SetMobileResourceText(lblFromAccount, cultureViewId, "39262576-25AB-4457-BCF8-5AE2DBE0E701");
                CultureTextProvider.SetMobileResourceText(lblStatus, cultureViewId, "8F83DB56-9A0F-4372-BACF-A05E83618815");
                CultureTextProvider.SetMobileResourceText(btnCancel, cultureViewId, "9E288CC2-A80F-4D6D-905E-28C848ED224E", "Cancel Payment");
                CultureTextProvider.SetMobileResourceText(btnUpdate, cultureViewId, "E53AB6E4-64EB-4B37-8CC6-431607618E4D", "Update Payment");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayDetailsFragment:SetCultureConfiguration");
			}
		}

		private async void CancelPayment()
		{

			var alertMsgTitle = CultureTextProvider.GetMobileResourceText(cultureViewId, "EBDC7240-0AB9-47D5-A4E6-FB920A10660E", "Confirm");
			var alertMsg = CultureTextProvider.GetMobileResourceText(cultureViewId, "DCF92478-399D-41C8-9019-051CB218198D", "Are you sure you want to cancel this payment?");
			var yesButtonText = CultureTextProvider.GetMobileResourceText(cultureViewId, "EFE07CAA-E0A5-4C85-B3A3-FC5C4F5AA654", "Yes");
			var noButtonText = CultureTextProvider.GetMobileResourceText(cultureViewId, "170A8E76-31C5-485B-9191-B91E24C757A3", "No");

			var response = await AlertMethods.Alert(Activity, alertMsgTitle, alertMsg, yesButtonText, noButtonText);

			if (response == yesButtonText)
			{
                var request = CurrentPayment;				

				var methods = new BillPayMethods();

				ShowActivityIndicator();

				await methods.CancelPayment(request, Activity);

				HideActivityIndicator();

				Logging.Track("Payment cancelled.");

				NavigationService.NavigatePop(false);
				PaymentChanged(false);
			}
		}

		private void UpdatePayment()
		{
            var billPaySchedulePaymentFragment = new BillPaySchedulePaymentFragment();
            billPaySchedulePaymentFragment.PaymentToEdit = CurrentPayment;			

			billPaySchedulePaymentFragment.PaymentScheduled += (isPending) =>
			{
				NavigationService.NavigatePop(false);
				PaymentChanged(isPending);
			};			

			NavigationService.NavigatePush(billPaySchedulePaymentFragment, true, false);
		}
	}
}