using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Droid.Accounts;
using SunMobile.Droid.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;
using SunBlock.DataTransferObjects.BillPay.V2;
using System.Threading.Tasks;
using System.Linq;

namespace SunMobile.Droid.BillPay
{
    public class BillPaySchedulePaymentFragment : BaseFragment
    {
        private TextView txtSourceHeaderText;
        private TextView txtSourceHeader2Text;
        private TextView txtSourceItem1Text;
        private TextView txtSourceValue1Text;
        private TextView txtSourceItem2Text;
        private TextView txtSourceValue2Text;

        private TextView txtPayee;
        private EditText txtAmount;
        private Spinner spinnerFrequency;
        private ArrayAdapter _spinnerAdapter;
        private Switch switchUntilCanceled;
        private TextView txtRepeat;
        private TableRow accountRow;
        private TableRow payeeRow;
        private TableRow withdrawDateRow;
        private EditText txtMemo;
        private Button btnAddPayment;

        private TextView lblTrnFrom;
        private TextView lblPayee;
        private TextView lblAmountLabel;
        private TextView lblSendOnDate;
        private TextView lblSpecify;
        private TextView lblUntilCanceled;
        private TextView lblBillPayDeliverBy;

        private TextView txtDate;
        private TextView txtDeliverBy;

        private TableLayout tblFrequency;
        private TableLayout tblRepeat;
        private TableLayout tblRepeat2;
        private TableLayout tblMemo;

        public event Action<bool> PaymentScheduled = delegate { };
        public Payment PaymentToEdit { get; set; }
        private List<Frequency> _frequencies;
        private string _suffix;
        private long _userPayeeId;
        private string _paymentMethod;
        private bool _isRecurring;
        private DateTime _deliveryByDate;
        private DateTime _willProcessDate;
        private int _spinnerCount;
        private string _singlePaymentDescription;

        private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

        private string sendOnDate;
        private string deliverBy;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.BillPayScheduleView, null);
            RetainInstance = true;

            // This will make sure the toolbar stays visible on rotation.  
            // It was being hidden because of the keyboard open.
            Activity.Window.SetSoftInputMode(SoftInput.AdjustResize);

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("Amount", txtAmount.Text);
            outState.PutString("Payee", txtPayee.Text);
            outState.PutString("WithDrawDate", txtDeliverBy.Text);
            outState.PutString("SourceHeader", txtSourceHeaderText.Text);
            outState.PutString("SourceHeader2", txtSourceHeader2Text.Text);
            outState.PutString("SourceItem", txtSourceItem1Text.Text);
            outState.PutString("SourceValue", txtSourceValue1Text.Text);
            outState.PutString("SourceItem2", txtSourceItem2Text.Text);
            outState.PutString("SourceValue2", txtSourceValue2Text.Text);

            outState.PutString("Suffix", _suffix);
            outState.PutString("PaymentMethod", _paymentMethod);
            outState.PutLong("PayeeId", _userPayeeId);

            var json = JsonConvert.SerializeObject(_deliveryByDate);
            outState.PutString("DeliveryDate", json);
            json = JsonConvert.SerializeObject(_willProcessDate);
            outState.PutString("WillProcessDate", json);

            base.OnSaveInstanceState(outState);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            base.SetupView();

            try
            {
                ((MainActivity)Activity).SetActionBarTitle("Schedule Payment");

                txtSourceHeaderText = Activity.FindViewById<TextView>(Resource.Id.txtSourceHeaderText);
                txtSourceHeader2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceHeader2Text);
                txtSourceItem1Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceItem1Text);
                txtSourceValue1Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceValue1Text);
                txtSourceItem2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceItem2Text);
                txtSourceValue2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceValue2Text);

                txtAmount = Activity.FindViewById<EditText>(Resource.Id.txtAmount);
                txtAmount.AfterTextChanged += OnTextChanged;
                spinnerFrequency = Activity.FindViewById<Spinner>(Resource.Id.spinnerFrequency);
                txtRepeat = Activity.FindViewById<EditText>(Resource.Id.txtRepeat);
                txtRepeat.TextChanged += (sender, e) => ValidatePayment();
                txtMemo = Activity.FindViewById<EditText>(Resource.Id.txtBillPayMemo);

                txtPayee = Activity.FindViewById<TextView>(Resource.Id.txtBillPayPayee);
                txtDate = Activity.FindViewById<TextView>(Resource.Id.lblDate);
                txtDeliverBy = Activity.FindViewById<TextView>(Resource.Id.lblDeliverBy);

                tblFrequency = Activity.FindViewById<TableLayout>(Resource.Id.tblBillPayFrequency);
                switchUntilCanceled = Activity.FindViewById<Switch>(Resource.Id.switchUntilCanceled);
                switchUntilCanceled.CheckedChange += SwitchUntilCanceled_CheckedChange;
                tblRepeat = Activity.FindViewById<TableLayout>(Resource.Id.tblBillPayRepeat);
                tblRepeat2 = Activity.FindViewById<TableLayout>(Resource.Id.tblBillPayRepeat2);
                tblMemo = Activity.FindViewById<TableLayout>(Resource.Id.tblBillPayMemo);

                accountRow = Activity.FindViewById<TableRow>(Resource.Id.accountRow);
                accountRow.Click += (sender, e) => SelectAccount();

                payeeRow = Activity.FindViewById<TableRow>(Resource.Id.bpPayeeRow);
                payeeRow.Click += (sender, e) => SelectPayee();

                withdrawDateRow = Activity.FindViewById<TableRow>(Resource.Id.bpWithdrawDateRow);
                withdrawDateRow.Click += (sender, e) => SelectDate();

                btnAddPayment = Activity.FindViewById<Button>(Resource.Id.btnAddPayment);
                btnAddPayment.Enabled = false;
                btnAddPayment.Text = null != PaymentToEdit ? "Update" : "Schedule";
                btnAddPayment.Click += (sender, e) => ConfirmPayment();

                lblTrnFrom = Activity.FindViewById<TextView>(Resource.Id.lblTrnFrom);
                lblPayee = Activity.FindViewById<TextView>(Resource.Id.lblPayee);
                lblAmountLabel = Activity.FindViewById<TextView>(Resource.Id.txtAmountLabel);
                lblSendOnDate = Activity.FindViewById<TextView>(Resource.Id.lblSendOnDate);
                lblBillPayDeliverBy = Activity.FindViewById<TextView>(Resource.Id.lblBillPayDeliverBy);
                lblSpecify = Activity.FindViewById<TextView>(Resource.Id.lblSpecify);
                lblUntilCanceled = Activity.FindViewById<TextView>(Resource.Id.lblUntilCanceled);

                ClearAll();

                if (savedInstanceState != null)
                {
                    txtAmount.Text = savedInstanceState.GetString("Amount");
                    txtAmount.Text = PaymentToEdit.Amount.ToString();
                    txtPayee.Text = savedInstanceState.GetString("Payee");
                    txtDeliverBy.Text = savedInstanceState.GetString("WithDrawDate");
                    txtSourceHeaderText.Text = savedInstanceState.GetString("SourceHeader");
                    txtSourceHeader2Text.Text = savedInstanceState.GetString("SourceHeader2");
                    txtSourceItem1Text.Text = savedInstanceState.GetString("SourceItem");
                    txtSourceValue1Text.Text = savedInstanceState.GetString("SourceValue");
                    txtSourceItem2Text.Text = savedInstanceState.GetString("SourceItem2");
                    txtSourceValue2Text.Text = savedInstanceState.GetString("SourceValue2");

                    _suffix = savedInstanceState.GetString("Suffix");
                    _paymentMethod = savedInstanceState.GetString("PaymentMethod");
                    _userPayeeId = savedInstanceState.GetLong("PayeeId");

                    var json = savedInstanceState.GetString("DeliveryDate");
                    _deliveryByDate = JsonConvert.DeserializeObject<DateTime>(json);
                    json = savedInstanceState.GetString("WillProcessDate");
                    _willProcessDate = JsonConvert.DeserializeObject<DateTime>(json);

                    SetDeliveryByDate();
                    ValidatePayment();
                }

                PostLoad();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:ViewDidLoad");
            }
        }

        private void HideRecurringFields()
        {
            tblRepeat.Visibility = _isRecurring ? ViewStates.Visible : ViewStates.Gone;
            tblRepeat2.Visibility = _isRecurring ? ViewStates.Visible : ViewStates.Gone;
            tblMemo.Visibility = _isRecurring ? ViewStates.Gone : ViewStates.Visible;
            lblSendOnDate.Text = _isRecurring ? "Deliver By" : "Send On Date";
            lblBillPayDeliverBy.Text = _isRecurring ? "Initial Send On" : "Delivery By";
            txtRepeat.Text = _isRecurring ? "1" : "0";
            lblUntilCanceled.Enabled = !switchUntilCanceled.Checked;

            if (PaymentToEdit != null && !_isRecurring)
            {
                tblFrequency.Visibility = ViewStates.Gone;
            }
        }

        private async void PostLoad()
        {
            try
            {
                await GetFrequencies();

                spinnerFrequency.ItemSelected += async (sender, e) =>
                {
                    ShowActivityIndicator();

                    var spinnerText = _spinnerAdapter.GetItem(((Spinner)sender).SelectedItemPosition).ToString();

                    if (spinnerText == _singlePaymentDescription)
                    {
                        _isRecurring = false;
                    }
                    else
                    {
                        _isRecurring = true;
                    }

                    HideRecurringFields();

                    if (_spinnerCount > 0)
                    {
                        var methods = new BillPayMethods();
                        _willProcessDate = await methods.GetFirstPossibleDeliveryDate(_isRecurring, _paymentMethod, Activity);
                        txtDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);
                        SetDeliveryByDate();
                        ValidatePayment();
                    }

                    _spinnerCount++;

                    HideActivityIndicator();
                };

                if (PaymentToEdit != null)
                {
                    _suffix = PaymentToEdit.SourceAccount;
                    _paymentMethod = PaymentToEdit.DeliveryMethod;
                    _userPayeeId = PaymentToEdit.MemberPayeeId;
                    txtPayee.Text = PaymentToEdit.PayeeName;
                    _deliveryByDate = PaymentToEdit.DeliverBy;
                    _willProcessDate = PaymentToEdit.SendOn;
                    txtMemo.Text = PaymentToEdit.Memo;

                    if (!string.IsNullOrEmpty(PaymentToEdit.PayeeAlias))
                    {
                        txtPayee.Text += " (" + PaymentToEdit.PayeeAlias + ")";
                    }

                    string amount = PaymentToEdit.Amount.ToString();

                    if (amount.Length == 1)
                    {
                        amount += ".00";
                    }

                    txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(amount);

                    var frequency = _frequencies.Where(x => x.FrequencyId == PaymentToEdit.FrequencyId).FirstOrDefault().FrequencyDescription;
                    var index = _frequencies.FindIndex(x => x.FrequencyDescription == frequency);
                    spinnerFrequency.SetSelection(index);

                    switchUntilCanceled.Checked = PaymentToEdit.FrequencyIndefinite;
                    txtRepeat.Text = PaymentToEdit.RemainingPayments.ToString();

                    _isRecurring = !((string)spinnerFrequency.GetItemAtPosition(index) == _singlePaymentDescription);

                    if (_isRecurring)
                    {
                        var saveDate = _willProcessDate;
                        _willProcessDate = _deliveryByDate;
                        _deliveryByDate = saveDate;
                    }

                    if (_deliveryByDate != DateTime.MinValue)
                    {
                        txtDeliverBy.Text = string.Format("{0:MM/dd/yyyy}", _deliveryByDate);
                    }

                    if (_willProcessDate != DateTime.MinValue)
                    {
                        txtDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);
                    }

                    GetAccountDescription();
                    SetDeliveryByDate();
                }
                else
                {
                    _isRecurring = false;
                    spinnerFrequency.SetSelection(0);
                    txtAmount.Enabled = false;
                    spinnerFrequency.Enabled = false;
                    txtMemo.Enabled = false;
                }

                HideRecurringFields();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:PostLoad");
            }
        }

        void SwitchUntilCanceled_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                lblSpecify.Enabled = false;
                txtRepeat.Enabled = false;
                lblUntilCanceled.Enabled = true;
            }
            else
            {
                lblSpecify.Enabled = true;
                txtRepeat.Enabled = true;
                lblUntilCanceled.Enabled = false;
            }

            ValidatePayment();
        }

        private async Task GetFrequencies()
        {
            try
            {
                ShowActivityIndicator();

                var methods = new BillPayMethods();
                var response = await methods.GetFrequencies(PaymentToEdit, Activity);

                HideActivityIndicator();

                if (response != null)
                {
                    _frequencies = response.Item1;
                    _singlePaymentDescription = response.Item2;
                }

                _spinnerAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, _frequencies.Select(x => x.FrequencyDescription).ToList());
                spinnerFrequency.Adapter = _spinnerAdapter;
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:GetFrequencies");
            }
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "94F29FF8-C65F-49DF-81E1-15FAD7B34BFB", "Schedule Payment");

                if (!string.IsNullOrEmpty(viewText))
                {
                    ((MainActivity)Activity).SetActionBarTitle(viewText);
                }

                CultureTextProvider.SetMobileResourceText(lblTrnFrom, cultureViewId, "F95C5221-1B2A-4816-9428-E0357BBD7541");
                CultureTextProvider.SetMobileResourceText(lblPayee, cultureViewId, "4B165436-E53E-4F50-B2FA-31894B1A1F92");
                CultureTextProvider.SetMobileResourceText(lblAmountLabel, cultureViewId, "012710F9-07D8-4DDA-B149-EFFF47306668");
                CultureTextProvider.SetMobileResourceText(lblSendOnDate, cultureViewId, "2D33C5C8-80C5-4710-86EE-8D13EF46BE22");
                CultureTextProvider.SetMobileResourceText(lblBillPayDeliverBy, cultureViewId, "55C049B8-62DA-4238-B0C4-32F3CB30D030");
                CultureTextProvider.SetMobileResourceText(btnAddPayment, cultureViewId, "E245D4C0-D5DF-43F3-8A4F-261E0864E421");
                txtAmount.Hint = CultureTextProvider.GetMobileResourceText(cultureViewId, "5D008B93-B5C9-4D8D-83FF-B3D5831A9C25", "Enter Amount");

                sendOnDate = lblSendOnDate.Text;
                deliverBy = lblBillPayDeliverBy.Text;
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:SetCultureConfiguration");
            }
        }

        private void ClearAll()
        {
            txtSourceHeaderText.Text = string.Empty;
            txtSourceHeader2Text.Text = string.Empty;
            txtSourceItem1Text.Text = string.Empty;
            txtSourceValue1Text.Text = string.Empty;
            txtSourceItem2Text.Text = string.Empty;
            txtSourceValue2Text.Text = string.Empty;

            txtPayee.Text = string.Empty;
            txtDate.Text = string.Empty;
            txtDeliverBy.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtMemo.Text = string.Empty;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            txtAmount.AfterTextChanged -= OnTextChanged;

            txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((TextView)sender).Text);
            ValidatePayment();

            txtAmount.SetSelection(txtAmount.Text.Length);

            txtAmount.AfterTextChanged += OnTextChanged;
        }

        private async void GetAccountDescription()
        {
            var methods = new AccountMethods();
            var request = new SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.AccountListRequest();

            ShowActivityIndicator();

            var response = await methods.BillPaySourceAccountList(request, Activity);

            HideActivityIndicator();

            if (response != null && response.ClientViewState != null && response.ClientViewState == "BillPaySourceAccountList")
            {
                foreach (var headerSection in response.AccountSections)
                {
                    foreach (var itemSection in headerSection.Items)
                    {
                        if (itemSection.Data.Suffix.PadSuffix() == _suffix.PadSuffix())
                        {
                            var listViewItem = ViewUtilities.GetAccountListViewItem(headerSection, itemSection, false);

                            txtSourceHeaderText.Text = listViewItem.HeaderText;
                            txtSourceHeader2Text.Text = listViewItem.Header2Text;
                            txtSourceItem1Text.Text = listViewItem.Item1Text;
                            txtSourceItem2Text.Text = listViewItem.Item2Text;
                            txtSourceValue1Text.Text = listViewItem.Value1Text;
                            txtSourceValue2Text.Text = listViewItem.Value2Text;
                        }
                    }
                }
            }
        }

        private void SelectAccount()
        {
            GeneralUtilities.CloseKeyboard(View);

            var intent = new Intent(Activity, typeof(SelectAccountActivity));
            const AccountListTypes accountListType = AccountListTypes.BillPayAccounts;
            var json = JsonConvert.SerializeObject(accountListType);
            intent.PutExtra("AccountListType", json);

            StartActivityForResult(intent, 0);
        }

        private void SelectPayee()
        {
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                if (PaymentToEdit == null)
                {
                    var intent = new Intent(Activity, typeof(SelectPayeeActivity));
                    StartActivityForResult(intent, 0);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:SelectPayee");
            }
        }

        private async void SelectDate()
        {
            try
            {
                GeneralUtilities.CloseKeyboard(View);

                if (!string.IsNullOrEmpty(txtPayee.Text))
                {
                    var methods = new BillPayMethods();
                    var startDate = await methods.GetFirstPossibleDeliveryDate(_isRecurring, _paymentMethod, Activity);
                    var endDate = DateTime.Today.AddMonths(6);

                    var intent = new Intent(Activity, typeof(SelectDateActivity));
                    intent.PutExtra("DisableHolidays", true);
                    intent.PutExtra("DisableWeekends", true);
                    intent.PutExtra("StartDate", startDate.ToString());
                    intent.PutExtra("EndDate", endDate.ToString());

                    if (_willProcessDate != DateTime.MinValue)
                    {
                        intent.PutExtra("SelectDate", _willProcessDate.ToString());
                    }

                    StartActivityForResult(intent, 0);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:SelectDate");
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
                    FrequencyId = _frequencies.Where(x => x.FrequencyDescription == _frequencies[spinnerFrequency.SelectedItemPosition].FrequencyDescription).FirstOrDefault().FrequencyId,
                    MemberPayeeId = _userPayeeId,
                    DeliveryMethod = _paymentMethod,
                    DeliveryDays = _paymentMethod.ToLower() == "electronic" ? "2" : "6",
                    DeliverBy = _isRecurring ? _willProcessDate : _deliveryByDate,
                    SendOn = _isRecurring ? _deliveryByDate : _willProcessDate,
                    Memo = txtMemo.Text,
                    IsRecurring = _isRecurring,
                    FrequencyIndefinite = switchUntilCanceled.Checked,
                    RemainingPayments = switchUntilCanceled.Checked ? 0 : remainingPayments,
                    PaymentId = null != PaymentToEdit && PaymentToEdit.PaymentId > 0 ? PaymentToEdit.PaymentId : 0
                };
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:PopulatePaymentRequest");
            }

            return request;
        }

        private void ValidatePayment()
        {
            var request = PopulatePaymentRequest();

            var methods = new BillPayMethods();
            btnAddPayment.Enabled = methods.ValidatePaymentRequest(request);
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
                var response = await AlertMethods.Alert(Activity, confirmPayment,
                    string.Format(responseText,
                    request.Amount,
                    request.SourceAccount,
                    txtPayee.Text,
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
                var response = await methods.AddPayment(request, Activity, null);

                HideActivityIndicator();

                var alertTitle = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");
                var alertButton = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "3D605DD2-9985-419F-B75A-5C23B5802385", "OK");

                if (response != null && response.Success && !response.OutOfBandChallengeRequired)
                {
                    var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "B812C5F5-7915-4B88-9DC2-3B2DA7EE35C8", "Payment scheduled successfully.");
                    await AlertMethods.Alert(Activity, alertTitle, alertText, alertButton);

                    ClearAll();

                    try
                    {
                        var keyValue = new Dictionary<string, string>();
                        keyValue.Add("Payment", JsonConvert.SerializeObject(payment));
                        keyValue.Add("Response", JsonConvert.SerializeObject(response));
                        Logging.Track("Payment scheduled.", keyValue);
                    }
                    catch { }

                    NavigationService.NavigatePop(false);
                    PaymentScheduled(true);
                }
                else if ((response != null && !response.Success && !response.OutOfBandChallengeRequired) || response == null)
                {
                    var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "CD2A8362-A5A8-4757-B538-A7B21A6D6C1B", "Payment failed.") + "  {0}";
                    await AlertMethods.Alert(Activity, alertTitle, string.Format(alertText, response?.FailureMessage ?? string.Empty), alertButton);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:InsertPayment");
            }
        }

        private async void UpdatePayment(Payment payment)
        {
            try
            {
                ShowActivityIndicator();

                var request = new MobileDeviceVerificationRequest<Payment> { Payload = RetainedSettings.Instance.Payload.Payload, Request = payment };
                var methods = new BillPayMethods();
                var response = await methods.UpdatePayment(request, Activity, null);

                HideActivityIndicator();

                var alertTitle = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "7C9B25A3-C56A-4112-99C6-4A34798BFF02", "Bill Pay");
                var alertButton = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "3D605DD2-9985-419F-B75A-5C23B5802385", "OK");

                if (response != null && response.Success && !response.OutOfBandChallengeRequired)
                {
                    var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "B812C5F5-7915-4B88-9DC2-3B2DA7EE35C8", "Payment scheduled successfully.");
                    await AlertMethods.Alert(Activity, alertTitle, alertText, alertButton);

                    ClearAll();

                    try
                    {
                        var keyValue = new Dictionary<string, string>();
                        keyValue.Add("Payment", JsonConvert.SerializeObject(payment));
                        keyValue.Add("Response", JsonConvert.SerializeObject(response));
                        Logging.Track("Payment scheduled.", keyValue);
                    }
                    catch { }

                    NavigationService.NavigatePop(false);
                    PaymentScheduled(true);
                }
                else if ((response != null && !response.Success && !response.OutOfBandChallengeRequired) || response == null)
                {
                    var alertText = CultureTextProvider.GetMobileResourceText("C42013B2-73B4-4942-87F3-4724E5D88592", "CD2A8362-A5A8-4757-B538-A7B21A6D6C1B", "Payment failed.") + "  {0}";
                    await AlertMethods.Alert(Activity, alertTitle, string.Format(alertText, response?.FailureMessage ?? string.Empty), alertButton);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentFragment:UpdatePayment");
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
                        _deliveryByDate = await methods.CalculateDeliverByDate(_willProcessDate, _paymentMethod, Activity);
                    }
                    else
                    {
                        _deliveryByDate = await methods.CalculateInitialSendOnDate(_willProcessDate, _paymentMethod, Activity);
                    }

                    txtDeliverBy.Text = string.Format("{0:MM/dd/yyyy}", _deliveryByDate);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "BillPaySchedulePaymentViewController:SetDeliveryByDate");
            }
        }

        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == (int)Result.Ok && data != null)
            {
                var className = data.GetStringExtra("ClassName");
                string json;
                ListViewItem listViewItem;

                switch (className)
                {
                    case "SelectAccountActivity":
                        json = data.GetStringExtra("ListViewItem");
                        listViewItem = JsonConvert.DeserializeObject<ListViewItem>(json);
                        json = data.GetStringExtra("Account");
                        var account = JsonConvert.DeserializeObject<Account>(json);
                        listViewItem.Data = account;

                        txtSourceHeaderText.Text = listViewItem.HeaderText;
                        txtSourceHeader2Text.Text = listViewItem.Header2Text;
                        txtSourceItem1Text.Text = listViewItem.Item1Text;
                        txtSourceItem2Text.Text = listViewItem.Item2Text;
                        txtSourceValue1Text.Text = listViewItem.Value1Text;
                        txtSourceValue2Text.Text = listViewItem.Value2Text;
                        _suffix = account.Suffix;
                        ValidatePayment();
                        break;
                    case "SelectPayeeActivity":
                        json = data.GetStringExtra("ListViewItem");
                        listViewItem = JsonConvert.DeserializeObject<ListViewItem>(json);
                        json = data.GetStringExtra("Payee");
                        var payee = JsonConvert.DeserializeObject<Payee>(json);
                        listViewItem.Data = payee;
                        txtPayee.Text = listViewItem.HeaderText;
                        _userPayeeId = ((Payee)listViewItem.Data).MemberPayeeId;
                        _paymentMethod = ((Payee)listViewItem.Data).DeliveryMethod;

                        var methods = new BillPayMethods();
                        _willProcessDate = await methods.GetFirstPossibleDeliveryDate(_isRecurring, _paymentMethod, Activity);
                        txtDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);

                        txtAmount.Enabled = true;
                        spinnerFrequency.Enabled = true;
                        txtMemo.Enabled = true;

                        SetDeliveryByDate();
                        ValidatePayment();
                        break;
                    case "SelectDateActivity":
                        json = data.GetStringExtra("SelectedDate");
                        _willProcessDate = JsonConvert.DeserializeObject<DateTime>(json);
                        txtDate.Text = string.Format("{0:MM/dd/yyyy}", _willProcessDate);
                        SetDeliveryByDate();
                        ValidatePayment();
                        break;
                }
            }
        }
    }
}