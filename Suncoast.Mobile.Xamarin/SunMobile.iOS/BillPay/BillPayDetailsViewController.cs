using System;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class BillPayDetailsViewController : BaseTableViewController
	{
		public event Action<bool> PaymentChanged = delegate { };
		public Payment CurrentPayment { get; set; }
		public bool IsPending { get; set; }

        private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

		public BillPayDetailsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();			

			lblPayee.Text = CurrentPayment.PayeeName;

            if (!string.IsNullOrEmpty(CurrentPayment.PayeeAlias))
			{
                lblPayee.Text += " (" + CurrentPayment.PayeeAlias + ")";
			}

			lblAccountNumber.Text = CurrentPayment.PayeeAccountNumber;
			lblAmount.Text = StringUtilities.FormatAsCurrency(CurrentPayment.Amount.ToString());
            lblFrequency.Text = CurrentPayment.Frequency;         
            lblDeliveryBy.Text = string.Format("{0:MM/dd/yyyy}", CurrentPayment.DeliverBy);
            lblAccount.Text = CurrentPayment.SourceAccount;
            lblReferenceId.Text = CurrentPayment.PaymentId.ToString();
            lblStatus.Text = CurrentPayment.Status;

            var methods = new BillPayMethods();

            if (!methods.CanPaymentBeEditedOrCanceled(CurrentPayment))
            {			
				btnCancel.Enabled = false;
				btnCancel.TintColor = UIColor.White;
				btnUpdate.Enabled = false;
				btnUpdate.TintColor = UIColor.White;
			}

			btnCancel.Clicked += (sender, e) => CancelPayment();
			btnUpdate.Clicked += (sender, e) => UpdatePayment();
		}

		public override void SetCultureConfiguration()
		{
			try
			{
                var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "CF4993E9-71F4-4D2A-902A-B90AB4D5AAD6");

                if(!string.IsNullOrEmpty(viewText))
                {
                    Title = viewText;
                }

				CultureTextProvider.SetMobileResourceText(lblPayeeText, cultureViewId, "4B165436-E53E-4F50-B2FA-31894B1A1F92");
				CultureTextProvider.SetMobileResourceText(lblAccountNumberText, cultureViewId, "140832FD-8C1E-47FF-87A9-54F2C35B082E");
				CultureTextProvider.SetMobileResourceText(lblAmountText, cultureViewId, "012710F9-07D8-4DDA-B149-EFFF47306668");
				CultureTextProvider.SetMobileResourceText(lblFrequencyText, cultureViewId, "2F73A49C-84B9-4D0C-965B-1498DAC1B253");				
				CultureTextProvider.SetMobileResourceText(lblAccountText, cultureViewId, "39262576-25AB-4457-BCF8-5AE2DBE0E701");
				CultureTextProvider.SetMobileResourceText(lblStatusText, cultureViewId, "8F83DB56-9A0F-4372-BACF-A05E83618815");
				CultureTextProvider.SetMobileResourceText(btnCancel, cultureViewId, "9E288CC2-A80F-4D6D-905E-28C848ED224E");
				CultureTextProvider.SetMobileResourceText(btnUpdate, cultureViewId, "E53AB6E4-64EB-4B37-8CC6-431607618E4D");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayDetailsViewController:SetCultureConfiguration");
			}
		}

		private async void CancelPayment()
		{
			try
			{
				var textConfirm = CultureTextProvider.GetMobileResourceText(cultureViewId, "EBDC7240-0AB9-47D5-A4E6-FB920A10660E", "Confirm");
				var textMsg = CultureTextProvider.GetMobileResourceText(cultureViewId, "DCF92478-399D-41C8-9019-051CB218198D", "Are you sure you want to cancel this payment?");
				var textYes = CultureTextProvider.GetMobileResourceText(cultureViewId, "EFE07CAA-E0A5-4C85-B3A3-FC5C4F5AA654", "Yes");
				var textNo = CultureTextProvider.GetMobileResourceText(cultureViewId, "170A8E76-31C5-485B-9191-B91E24C757A3", "No");

				var response = await AlertMethods.Alert(View, textConfirm, textMsg, textYes, textNo);

				if (response == textYes)
				{
                    var request = new Payment();
					request = CurrentPayment;

					var methods = new BillPayMethods();

					ShowActivityIndicator();

					var cancelResponse = await methods.CancelPayment(request, View);

					HideActivityIndicator();

					Logging.Track("Payment cancelled.");

					NavigationController.PopViewController(true);
					PaymentChanged(false);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPayDetailsViewController:CancelPayment");
			}
		}

		private void UpdatePayment()
		{
			var controller = AppDelegate.StoryBoard.InstantiateViewController("BillPaySchedulePaymentViewController") as BillPaySchedulePaymentViewController;
			controller.PaymentToEdit = CurrentPayment;
			controller.PaymentScheduled += (isPending) =>
			{
				NavigationController.PopViewController(true);
				PaymentChanged(isPending);
			};

			NavigationController.PushViewController(controller, true);
		}
	}
}