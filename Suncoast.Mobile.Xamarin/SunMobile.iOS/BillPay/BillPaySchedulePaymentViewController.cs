using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public partial class BillPaySchedulePaymentViewController : BaseTableViewController
	{
		public event Action<bool> PaymentScheduled = delegate { };
		public Payment PaymentToEdit { get; set; }
        private List<Frequency> _frequencies;
		private string _suffix;
		private long _userPayeeId;
		private string _paymentMethod;
        private bool _isRecurring;
		private DateTime _deliveryByDate;
        private DateTime _willProcessDate;
        private string _singlePaymentDescription;

		public BillPaySchedulePaymentViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            try
            {
                var schedule = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "E245D4C0-D5DF-43F3-8A4F-261E0864E421", "Schedule");
                var update = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "05C52C09-DB35-46DB-BA5F-1C43E637BE80", "Update");
                var rightButton = null != PaymentToEdit ? new UIBarButtonItem(update, UIBarButtonItemStyle.Plain, null) : new UIBarButtonItem(schedule, UIBarButtonItemStyle.Plain, null);

                rightButton.TintColor = AppStyles.TitleBarItemTintColor;
                NavigationItem.SetRightBarButtonItem(rightButton, false);
                NavigationItem.RightBarButtonItem.Enabled = false;
                rightButton.Clicked += (sender, e) => ConfirmPayment();

                txtAmount.EditingChanged += (sender, e) =>
                {
                    txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((UITextField)sender).Text);
                    ValidatePayment();
                };

                txtAmount.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return replacementString.IsNumericOrEmpty() && newLength <= 15;
                };

                txtMemo.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return newLength <= 255;
                };

                txtRepeat.EditingChanged += (sender, e) => ValidatePayment();

                txtRepeat.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return replacementString.IsNumericOrEmpty() && newLength <= 4;
                };

                switchUntilCanceled.ValueChanged += (sender, e) => RepeatChanged();
                switchUntilCanceled.On = false;
                RepeatChanged();

                // Hides the remaining rows.
                tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

                ClearAll();

                PostLoad();
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:ViewDidLoad");
            }
		}

        private void RepeatChanged()
        {
            lblUntilCanceled.Enabled = switchUntilCanceled.On;
            lblSpecify.Enabled = !switchUntilCanceled.On;
            txtRepeat.Enabled = !switchUntilCanceled.On;
            ValidatePayment();
        }

        public override void SetCultureConfiguration()
        {
            Title = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "94F29FF8-C65F-49DF-81E1-15FAD7B34BFB", "Schedule Payment");
            txtAmount.Placeholder = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "5D008B93-B5C9-4D8D-83FF-B3D5831A9C25", "Enter Amount");
            CultureTextProvider.SetMobileResourceText(lblBPSFundingAccount, "C42013B2-73B4-4942-87F3-4724E5D88592", "F95C5221-1B2A-4816-9428-E0357BBD7541", "Funding Account");
            CultureTextProvider.SetMobileResourceText(lblBPSPayee, "C42013B2-73B4-4942-87F3-4724E5D88592", "4B165436-E53E-4F50-B2FA-31894B1A1F92", "Payee");
            CultureTextProvider.SetMobileResourceText(lblBPSAmount, "C42013B2-73B4-4942-87F3-4724E5D88592", "012710F9-07D8-4DDA-B149-EFFF47306668", "Amount");
            CultureTextProvider.SetMobileResourceText(lblBPSSendOnDate, "C42013B2-73B4-4942-87F3-4724E5D88592", "2D33C5C8-80C5-4710-86EE-8D13EF46BE22", "Send On Date");
            CultureTextProvider.SetMobileResourceText(lblBPSDeliverBy, "C42013B2-73B4-4942-87F3-4724E5D88592", "55C049B8-62DA-4238-B0C4-32F3CB30D030", "Deliver By");
        }

        private async void PostLoad()
        {
            try
            {
                await GetFrequencies();

                var methods = new BillPayMethods();

                txtFrequency.EditingDidEnd += async (sender, e) =>
                {
                    ShowActivityIndicator();

                    _isRecurring = !(txtFrequency.Text == _singlePaymentDescription);

                    HideRecurringFields();

                    _willProcessDate = await methods.GetFirstPossibleDeliveryDate(_isRecurring, _paymentMethod, View);
                    lblDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);
                    SetDeliveryByDate();
                    ValidatePayment();

                    HideActivityIndicator();
                };

                if (PaymentToEdit != null)
                {
                    _suffix = PaymentToEdit.SourceAccount;
                    _paymentMethod = PaymentToEdit.DeliveryMethod;
                    _userPayeeId = PaymentToEdit.MemberPayeeId;
                    _deliveryByDate = PaymentToEdit.DeliverBy;
                    _willProcessDate = PaymentToEdit.SendOn;
                    lblPayee.Text = PaymentToEdit.PayeeName;
                    txtFrequency.Text = _frequencies.Where(x => x.FrequencyId == PaymentToEdit.FrequencyId).FirstOrDefault().FrequencyDescription;
                    txtMemo.Text = PaymentToEdit.Memo;

                    if (!string.IsNullOrEmpty(PaymentToEdit.PayeeAlias))
                    {
                        lblPayee.Text += " (" + PaymentToEdit.PayeeAlias + ")";
                    }

                    string amount = PaymentToEdit.Amount.ToString();

                    if (amount.Length == 1)
                    {
                        amount += ".00";
                    }

                    txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(amount);

                    _isRecurring = !(txtFrequency.Text == _singlePaymentDescription);

                    switchUntilCanceled.On = PaymentToEdit.FrequencyIndefinite;
                    txtRepeat.Text = PaymentToEdit.RemainingPayments.ToString();

                    if (_isRecurring)
                    {
                        var saveDate = _willProcessDate;
                        _willProcessDate = _deliveryByDate;
                        _deliveryByDate = saveDate;
                    }

                    if (_deliveryByDate != DateTime.MinValue)
                    {
                        lblDeliverBy.Text = string.Format("{0:MM/dd/yyyy}", _deliveryByDate);
                    }

                    if (_willProcessDate != DateTime.MinValue)
                    {
                        lblDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);
                    }

                    /*
                    if (_deliveryByDate != DateTime.MinValue)
                    {
                        lblDeliverBy.Text = string.Format("{0:MM/dd/yyyy}", _isRecurring ? _willProcessDate : _deliveryByDate);
                    }

                    if (_willProcessDate != DateTime.MinValue)
                    {
                        lblDate.Text = string.Format("{0:MM/dd/yyyy}", _isRecurring ? _deliveryByDate : _willProcessDate);
                    }
                    */

                    GetAccountDescription();
                    SetDeliveryByDate();
                }
                else
                {
                    _isRecurring = false;
                    txtFrequency.Text = _singlePaymentDescription;
                    txtAmount.Enabled = false;
                    txtFrequency.Enabled = false;
                    txtMemo.Enabled = false;
                }

                HideRecurringFields();
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:PostLoad");
            }
        }       

        private void HideRecurringFields()
        {
            cellRepeat.Hidden = !_isRecurring;
            cellMemo.Hidden = _isRecurring;
            lblBPSSendOnDate.Text = _isRecurring ? "Delivery By" : "Send On Date";
            lblBPSDeliverBy.Text = _isRecurring ? "Initial Send On" : "Delivery By";
            txtRepeat.Text = _isRecurring ? "1": "0";
            

            if (PaymentToEdit != null && !_isRecurring)
            {
                cellFrequency.Hidden = true;
            }

            tableView.ReloadData();
        }

        private async void GetAccountDescription()
        {
            try
            {
                var methods = new AccountMethods();
                var request = new SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.AccountListRequest();

                ShowActivityIndicator();

                var response = await methods.BillPaySourceAccountList(request, null);
                HideActivityIndicator();

                if (response != null && response.ClientViewState != null && response.ClientViewState == "BillPaySourceAccountList")
                {
                    var model = ViewUtilities.ConvertTextViewModelToGroupedTextViewTableSource(response, false);

                    foreach (var section in model.Sections)
                    {
                        foreach (var listViewItem in section.Value.ListViewItems)
                        {
                            var account = (Account)listViewItem.Data;

                            if (_suffix == account.Suffix || account.Suffix.Contains(_suffix))
                            {
                                lblAccount.Text = listViewItem.HeaderText;
                                lblAccount2.Text = listViewItem.Header2Text;
                                lblText1.Text = listViewItem.Item1Text;
                                lblText2.Text = listViewItem.Item2Text;
                                lblValue1.Text = listViewItem.Value1Text;
                                lblValue2.Text = listViewItem.Value2Text;
                                lblText1.AccessibilityLabel = listViewItem.Item1Text + listViewItem.Value1Text;
                                lblValue1.AccessibilityLabel = string.Empty;
                                lblText2.AccessibilityLabel = listViewItem.Item2Text + listViewItem.Value2Text;
                                lblValue2.AccessibilityLabel = string.Empty;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:GetAccountDescription");
            }
		}

        private async Task GetFrequencies()
        {
            try
            {
                ShowActivityIndicator();

                var methods = new BillPayMethods();
                var response = await methods.GetFrequencies(PaymentToEdit, View);

                HideActivityIndicator();

                if (response != null)
                {
                    _frequencies = response.Item1;
                    _singlePaymentDescription = response.Item2;
                }

                CommonMethods.CreateDropDownFromTextField(txtFrequency, _frequencies.Select(x => x.FrequencyDescription).ToList());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:GetFrequencies");
            }
        }

		private void ClearAll()
		{
			lblAccount.Text = string.Empty;
			lblAccount2.Text = string.Empty;
			lblText1.Text = string.Empty;
			lblText2.Text = string.Empty;
			lblValue1.Text = string.Empty;
			lblValue2.Text = string.Empty;
			lblPayee.Text = string.Empty;
			txtAmount.Text = string.Empty;
			lblDate.Text = string.Empty;
			lblDeliverBy.Text = string.Empty;
            txtMemo.Text = string.Empty;
            txtFrequency.Text = string.Empty;
            txtRepeat.Text = string.Empty;
		}

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            // Frequency row
            if (PaymentToEdit != null && !_isRecurring && indexPath.Row == 3)
            {
                return 0;
            }

            // Repeat row
            if (!_isRecurring && indexPath.Row == 4)
            {
                return 0;
            }

            // Memo Row
            if (_isRecurring && indexPath.Row == 7)
            {
                return 0;
            }

            return base.GetHeightForRow(tableView, indexPath);
        }

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row)
			{
				case 0: // Account
					SelectAccount();
					break;
				case 1: // Payee
					if (PaymentToEdit == null)
					{
						SelectPayee();
					}
					break;
				case 5: // Date
					SelectDate();
					break;
			}
		}

		private void SelectAccount()
		{
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                var selectAccountViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectAccountViewController") as SelectAccountViewController;
                selectAccountViewController.AccountListType = AccountListTypes.BillPayAccounts;
                selectAccountViewController.ShowJoints = false;
                selectAccountViewController.ShowAnyMember = false;

                selectAccountViewController.AccountSelected += listViewItem =>
                {
                    lblAccount.Text = listViewItem.HeaderText;
                    lblAccount2.Text = listViewItem.Header2Text;
                    lblText1.Text = listViewItem.Item1Text;
                    lblText2.Text = listViewItem.Item2Text;
                    lblValue1.Text = listViewItem.Value1Text;
                    lblValue2.Text = listViewItem.Value2Text;
                    lblText1.AccessibilityLabel = listViewItem.Item1Text + listViewItem.Value1Text;
                    lblValue1.AccessibilityLabel = string.Empty;
                    lblText2.AccessibilityLabel = listViewItem.Item2Text + listViewItem.Value2Text;
                    lblValue2.AccessibilityLabel = string.Empty;
                    _suffix = ((Account)listViewItem.Data).Suffix;

                    ValidatePayment();
                };

                NavigationController.PushViewController(selectAccountViewController, true);
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:SelectAccount");
            }
		}

		private void SelectPayee()
		{
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                var selectPayeeTableViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectPayeeTableViewController") as SelectPayeeTableViewController;

                selectPayeeTableViewController.PayeeSelected += async listViewItem =>
                {
                    lblPayee.Text = listViewItem.HeaderText;
                    _userPayeeId = ((Payee)listViewItem.Data).MemberPayeeId;
                    _paymentMethod = ((Payee)listViewItem.Data).DeliveryMethod;

                    var methods = new BillPayMethods();
                    _willProcessDate = await methods.GetFirstPossibleDeliveryDate(_isRecurring, _paymentMethod, View);
                    lblDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);

                    txtAmount.Enabled = true;
                    txtFrequency.Enabled = true;
                    txtMemo.Enabled = true;

                    SetDeliveryByDate();

                    ValidatePayment();
                };

                NavigationController.PushViewController(selectPayeeTableViewController, true);
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:SelectPayee");
            }
		}              

		private async void SelectDate()
		{
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                if (!string.IsNullOrEmpty(lblPayee.Text))
                {
                    var methods = new BillPayMethods();
                    var startDate = await methods.GetFirstPossibleDeliveryDate(_isRecurring, _paymentMethod, View);
                    var endDate = DateTime.Today.AddMonths(6);

                    var datePickerViewController = AppDelegate.StoryBoard.InstantiateViewController("DatePickerViewController") as DatePickerViewController;
                    datePickerViewController.DisableHolidays = true;
                    datePickerViewController.DisableWeekends = true;
                    datePickerViewController.StartDate = startDate;
                    datePickerViewController.EndDate = endDate;

                    if (_willProcessDate != DateTime.MinValue)
                    {
                        datePickerViewController.SelectDate = _willProcessDate;
                    }

                    datePickerViewController.ItemSelected += date =>
                    {
                        _willProcessDate = date;
                        lblDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);

                        SetDeliveryByDate();

                        ValidatePayment();
                    };

                    NavigationController.PushViewController(datePickerViewController, true);
                }
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:SelectDate");
            }
		}

		private async void SetDeliveryByDate()
		{
            try
            {
                if (_willProcessDate != DateTime.MinValue)
                {
                    var methods = new BillPayMethods();

                    if (!_isRecurring)
                    {
                        _deliveryByDate = await methods.CalculateDeliverByDate(_willProcessDate, _paymentMethod, View);
                    }
                    else
                    {
                        _deliveryByDate = await methods.CalculateInitialSendOnDate(_willProcessDate, _paymentMethod, View);
                    }

                    lblDeliverBy.Text = string.Format("{0:MM/dd/yyyy}", _deliveryByDate);
                }
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:SetDeliveryByDate");
            }
		}

        private Payment PopulatePaymentRequest()
		{
            var request = new Payment();

            try
            {
                decimal result = 0;
                decimal.TryParse(StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(txtAmount.Text)), out result);

                int remainingPayments = 0;
                int.TryParse(txtRepeat.Text, out remainingPayments);

                request = new Payment
                {
                    SourceAccount = _suffix,
                    Amount = result,
                    FrequencyId = _frequencies.Where(x => x.FrequencyDescription == txtFrequency.Text).FirstOrDefault().FrequencyId,
                    MemberPayeeId = _userPayeeId,
                    DeliveryMethod = _paymentMethod,
                    DeliveryDays = _paymentMethod.ToLower() == "electronic" ? "2" : "6",
                    DeliverBy = _isRecurring ? _willProcessDate : _deliveryByDate,
                    SendOn = _isRecurring ? _deliveryByDate : _willProcessDate,
                    Memo = txtMemo.Text,
                    IsRecurring = _isRecurring,
                    FrequencyIndefinite = switchUntilCanceled.On,
                    RemainingPayments = switchUntilCanceled.On ? 0 : remainingPayments,
                    PaymentId = null != PaymentToEdit && PaymentToEdit.PaymentId > 0 ? PaymentToEdit.PaymentId : 0
                };
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:PopulatePaymentRequest");
            }

			return request;
		}

		private void ValidatePayment()
		{
			var request = PopulatePaymentRequest();

			var methods = new BillPayMethods();
            NavigationItem.RightBarButtonItem.Enabled = methods.ValidatePaymentRequest(request);
		}

		private async void ConfirmPayment()
		{
            try
            {
                var request = PopulatePaymentRequest();

                var responseText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "199514DA-42A9-4718-A498-895035939F75", "Would you like to schedule the payment in the amount of {0:C} from your {1} suffix to {2} on {3}?");
                var schedulePayment = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "94F29FF8-C65F-49DF-81E1-15FAD7B34BFB", "Schedule Payment");
                var noReview = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "170A8E76-31C5-485B-9191-B91E24C757A3", "No, Review");
                var confirmPayment = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "42520BB0-879D-4E20-B399-0518A8BF956C", "Confirm Payment");
                var response = await AlertMethods.Alert(View, confirmPayment,
                    string.Format(responseText,
                    request.Amount,
                    request.SourceAccount,
                    lblPayee.Text,
                    string.Format("{0:MM/dd/yyyy}",
                    request.SendOn)),
                    schedulePayment, noReview);

                if (response == schedulePayment)
                {
                    if (PaymentToEdit?.PaymentId > 0)
                    {
                        UpdatePayment(request);
                    }
                    else
                    {
                        AddPayment(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:ConfirmPayment");
            }
		}

        private async void AddPayment(Payment payment)
		{
			try
			{
				ShowActivityIndicator();

				var request = new MobileDeviceVerificationRequest<Payment> { Payload = RetainedSettings.Instance.Payload.Payload, Request = payment };
				var methods = new BillPayMethods();
                var response = await methods.AddPayment(request, View, NavigationController);

				HideActivityIndicator();

				var alertTitle = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");
				var alertButton = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "3D605DD2-9985-419F-B75A-5C23B5802385", "OK");

				if (response != null && response.Success && !response.OutOfBandChallengeRequired)
				{
					var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "B812C5F5-7915-4B88-9DC2-3B2DA7EE35C8", "Payment scheduled successfully.");
					await AlertMethods.Alert(View, alertTitle, alertText, alertButton);

					ClearAll();

					try
					{
						var keyValue = new Dictionary<string, string>();
                        keyValue.Add("Payment", JsonConvert.SerializeObject(payment));
                        keyValue.Add("Response", JsonConvert.SerializeObject(response));
						Logging.Track("Payment scheduled.", keyValue);
					}
					catch { }					

					NavigationController.PopViewController(true);
					PaymentScheduled(true);
				}
				else if ((response != null && !response.Success && !response.OutOfBandChallengeRequired) || response == null)
				{
					var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "CD2A8362-A5A8-4757-B538-A7B21A6D6C1B", "Payment failed.") + "  {0}";
					await AlertMethods.Alert(View, alertTitle, string.Format(alertText, response?.FailureMessage ?? string.Empty), alertButton);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPaySchedulePaymentViewController:InsertPayment");
			}
		}

		private async void UpdatePayment(Payment payment)
		{
			try
			{
				ShowActivityIndicator();

				var request = new MobileDeviceVerificationRequest<Payment> { Payload = RetainedSettings.Instance.Payload.Payload, Request = payment };
				var methods = new BillPayMethods();
				var response = await methods.UpdatePayment(request, View, NavigationController);

				HideActivityIndicator();

				var alertTitle = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");
				var alertButton = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "3D605DD2-9985-419F-B75A-5C23B5802385", "OK");

				if (response != null && response.Success && !response.OutOfBandChallengeRequired)
				{
					var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "B812C5F5-7915-4B88-9DC2-3B2DA7EE35C8", "Payment scheduled successfully.");
					await AlertMethods.Alert(View, alertTitle, alertText, alertButton);

					ClearAll();

					try
					{
						var keyValue = new Dictionary<string, string>();
						keyValue.Add("Payment", JsonConvert.SerializeObject(payment));
						keyValue.Add("Response", JsonConvert.SerializeObject(response));
						Logging.Track("Payment scheduled.", keyValue);
					}
					catch { }

					NavigationController.PopViewController(true);
					PaymentScheduled(true);
				}
				else if ((response != null && !response.Success && !response.OutOfBandChallengeRequired) || response == null)
				{
					var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "CD2A8362-A5A8-4757-B538-A7B21A6D6C1B", "Payment failed.") + "  {0}";
					await AlertMethods.Alert(View, alertTitle, string.Format(alertText, response?.FailureMessage ?? string.Empty), alertButton);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "BillPaySchedulePaymentViewController:UpdatePayment");
			}
		}
	}
}