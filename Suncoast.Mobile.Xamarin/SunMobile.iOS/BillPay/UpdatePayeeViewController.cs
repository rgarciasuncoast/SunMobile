using System;
using System.Collections.Generic;
using System.Linq;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.States;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class UpdatePayeeViewController : BaseTableViewController
	{
		static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";
		public Payee PayeeToEdit { get; set; }
		public event Action<bool> Updated = delegate { };

		public UpdatePayeeViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();			

			var textUpdatePayee = CultureTextProvider.GetMobileResourceText(cultureViewId, "B4E716B8-BAE6-4455-B875-6E9E85AB9959", "Update Payee");
			var textAddPayee = CultureTextProvider.GetMobileResourceText(cultureViewId, "56FD0B81-DF1D-43C7-BF1E-5C51E79378CD", "Add Payee");
			var textUpdate = CultureTextProvider.GetMobileResourceText(cultureViewId, "05C52C09-DB35-46DB-BA5F-1C43E637BE80", "Update");
			var textAdd = CultureTextProvider.GetMobileResourceText(cultureViewId, "5B79EC78-2D23-4E79-B85E-42EA2E16A1E3", "Add");
			var textDays = CultureTextProvider.GetMobileResourceText(cultureViewId, "E778DD5B-03A2-49B8-9C94-2C5B3ED4E246", "Days");

			Title = null != PayeeToEdit ? textUpdatePayee : textAddPayee;

			var rightButton = null != PayeeToEdit ? new UIBarButtonItem(textUpdate, UIBarButtonItemStyle.Plain, null) : new UIBarButtonItem(textAdd, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			rightButton.Clicked += (sender, e) => UpdatePayee();

			// Hides the remaining rows.
			mainTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			txtPhone.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 10;
			};

			ClearAll();

			var stateNames = new List<string>(USStates.USStateList.Values);

			if (PayeeToEdit != null)
			{
				txtPayeeName.Text = PayeeToEdit.PayeeName;
				txtPayeeName.Enabled = false;
				txtNickname.Text = PayeeToEdit.PayeeAlias;
				txtAddress1.Text = PayeeToEdit.Address1;
				txtAddress2.Text = PayeeToEdit.Address2;
				txtCity.Text = PayeeToEdit.City;
				txtState.Text = PayeeToEdit.State;

				string stateName;
				USStates.USStateList.TryGetValue(txtState.Text, out stateName);

				if (!string.IsNullOrEmpty(stateName))
				{
					txtState.Text = stateName;
				}

				txtZipcode.Text = PayeeToEdit.PostalCode;
				txtPhone.Text = PayeeToEdit.Phone;
				txtNameOnBill.Text = PayeeToEdit.NameOnAccount;
				txtAccountNumber.Text = PayeeToEdit.PayeeAccountNumber;
				txtDeliveryTime.Text = string.Format("{0} {1}", PayeeToEdit.DeliveryDays, textDays);
				switchActive.On = PayeeToEdit.Active;

				txtNickname.BecomeFirstResponder();

				CheckForPendingPayments();
			}
			else
			{
				txtDeliveryTime.Text = string.Format("6 {0}", textDays); ;
				txtPayeeName.BecomeFirstResponder();
			}

			CommonMethods.CreateDropDownFromTextField(txtState, stateNames);
		}

		public override void SetCultureConfiguration()
		{
			CultureTextProvider.SetMobileResourceText(lblPayeeName, cultureViewId, "32958A0D-BB47-4AA6-901D-3949A57EBE25", "Payee Name");
			CultureTextProvider.SetMobileResourceText(lblNickname, cultureViewId, "3D82BBCE-8883-40A1-AA40-D422EEBEFA46", "Nickname");
			CultureTextProvider.SetMobileResourceText(lblAddress1, cultureViewId, "F2E062D5-EBD6-4C17-B39E-CFAB7FC6D4A4", "Street Address 1");
			CultureTextProvider.SetMobileResourceText(lblAddress2, cultureViewId, "5C8386BA-632D-4BCB-92CE-2810C50361C0", "Street Address 2");
			CultureTextProvider.SetMobileResourceText(lblCity, cultureViewId, "0302C778-CC67-4B58-9891-DA533B7BEA3C", "City");
			CultureTextProvider.SetMobileResourceText(lblState, cultureViewId, "A52BCD65-567F-4A49-A233-0756B91996B7", "State");
			CultureTextProvider.SetMobileResourceText(lblZipcode, cultureViewId, "3A9A1B88-6F81-40BA-8A5A-B6BDF1C24C98", "Zipcode");
			CultureTextProvider.SetMobileResourceText(lblPhone, cultureViewId, "352BACEC-B46C-4296-B809-E8B6CA0BF12C", "Phone");
			CultureTextProvider.SetMobileResourceText(lblNameOnBill, cultureViewId, "4FF51913-0BB8-43AE-9135-60A03C5F4001", "Name On Bill");
			CultureTextProvider.SetMobileResourceText(lblAccountNumber, cultureViewId, "32496C74-67FC-4B91-B960-9E5301DF2F68", "Account Number");
			CultureTextProvider.SetMobileResourceText(lblDeliveryTime, cultureViewId, "49AAA7D8-5FD5-4A4D-9A03-0B04EB4F9BEA", "Delivery Time");
            CultureTextProvider.SetMobileResourceText(lblActive, cultureViewId, "65BF6355-7D3B-4D4E-A0F1-D9EA155B503F", "Active");
		}

		private async void CheckForPendingPayments()
		{
			var request = new DoesPayeeHavePendingPaymentsRequest
			{
				MemberId = GeneralUtilities.GetMemberIdAsInt(),
				MemberPayeeId = PayeeToEdit.MemberPayeeId
			};

			var methods = new BillPayMethods();

			ShowActivityIndicator();

			var response = await methods.DoesPayeeHavePendingPayments(request, null);

			HideActivityIndicator();

			if (response != null && response.Success && response.Result)
			{
				txtPayeeName.Enabled = false;
				txtAddress1.Enabled = false;
				txtAddress2.Enabled = false;
				txtCity.Enabled = false;
				txtState.Enabled = false;
				txtZipcode.Enabled = false;
				txtPhone.Enabled = false;
				txtNameOnBill.Enabled = false;
				txtAccountNumber.Enabled = false;
				txtDeliveryTime.Enabled = false;
				switchActive.Enabled = false;
				CultureTextProvider.SetMobileResourceText(lblNote, cultureViewId, "E1CC57C7-90E4-4AF2-99B5-C4B059414071", "NOTE: This payee information cannot be edited while payments are currently pending.");
			}
		}

		private void ClearAll()
		{
			txtPayeeName.Text = string.Empty;
			txtNickname.Text = string.Empty;
			txtAddress1.Text = string.Empty;
			txtAddress2.Text = string.Empty;
			txtCity.Text = string.Empty;
			txtState.Text = string.Empty;
			txtZipcode.Text = string.Empty;
			txtPhone.Text = string.Empty;
			txtNameOnBill.Text = string.Empty;
			txtAccountNumber.Text = string.Empty;
			txtDeliveryTime.Text = string.Empty;
			switchActive.On = true;
			lblNote.Text = string.Empty;
		}

		private Payee PopulatePayee()
		{
			var payee = new Payee();

			if (PayeeToEdit != null)
			{
				payee.MemberPayeeId = PayeeToEdit.MemberPayeeId;
			}

			payee.Active = switchActive.On;
			payee.Address1 = txtAddress1.Text;
			payee.Address2 = txtAddress2.Text;
			payee.City = txtCity.Text;
			payee.DeliveryDays = txtDeliveryTime.Text;
			payee.MemberId = GeneralUtilities.GetMemberIdAsInt();
			payee.NameOnAccount = txtNameOnBill.Text;
			payee.PayeeAccountNumber = txtAccountNumber.Text;
			payee.PayeeAlias = txtNickname.Text;
			payee.PayeeName = txtPayeeName.Text;
			payee.Phone = txtPhone.Text;
			payee.PostalCode = txtZipcode.Text;

			payee.State = txtState.Text;
			var stateAbbreviation = USStates.USStateList.FirstOrDefault(x => x.Value == txtState.Text).Key;

			if (!string.IsNullOrEmpty(stateAbbreviation))
			{
				payee.State = stateAbbreviation;
			}

			return payee;
		}

		private async void UpdatePayee()
		{
			StatusResponse<Payee> response;

			var payee = PopulatePayee();

			var methods = new BillPayMethods();

			ShowActivityIndicator();

			response = PayeeToEdit == null ? await methods.AddPayee(payee, null) : await methods.UpdatePayee(payee, null);

			HideActivityIndicator();

			if (response != null && response.Success)
			{
				Updated(true);
				NavigationController.PopViewController(true);
			}
			else if (response != null && !response.Success)
			{
				await AlertMethods.Alert(View, "SunMobile", response.FailureMessage, "OK");
			}
			else
			{
				var errorUpdatingPayeeTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "9F8D3FC3-8622-4F0A-884B-4A57E9A56143", "Error updating payee.");
				await AlertMethods.Alert(View, "SunMobile", errorUpdatingPayeeTxt, "OK");
			}
		}
	}
}