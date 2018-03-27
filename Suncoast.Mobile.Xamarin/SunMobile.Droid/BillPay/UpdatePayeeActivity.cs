using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.Droid.Common;
using SunMobile.Shared.Methods;
using SunMobile.Shared.States;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Culture;
using System;
using SunMobile.Shared.Logging;

namespace SunMobile.Droid.BillPay
{
	[Activity(Label = "BillPayAddPayee", Theme = "@style/CustomHoloLightTheme")]
	public class UpdatePayeeActivity : BaseActivity
	{
		private Payee _payeeToEdit { get; set; }
		private TextView txtTitle;
		private ImageButton btnCloseWindow;
		private EditText txtPayeeName;
		private EditText txtPayeeNickname;
		private EditText txtPayeeAddress1;
		private EditText txtPayeeAddress2;
		private EditText txtPayeeCity;
		private Spinner spinnerState;
		private EditText txtPayeeZip;
		private EditText txtPayeePhone;
		private EditText txtPayeeNameOnBill;
		private EditText txtPayeeAccountNumber;
		private TextView lblPayeeDeliveryTime;
		private TextView lblNote;
		private TextView btnAddPayee;
		private Button btnActive;
		private Button btnInactive;
		private SegmentedGroup segmentedActiveInactive;
		private List<string> _stateNames;
		private StatusResponse<bool> _doesPayeeHavePendingPayments;

		private TextView lblPayeeName;
		private TextView lblPayeeNickname;
		private TextView lblPayeeAddress1;
		private TextView lblPayeeAddress2;
		private TextView lblPayeeCity;
		private TextView lblSpinnerState;
		private TextView lblPayeeZip;
		private TextView lblPayeePhone;
		private TextView lblPayeeNameOnBill;
		private TextView lblPayeeAccountNumber;
		private TextView lblDeliveryTime;
		private TextView lblPayeeActive;

		private static readonly string cultureViewId = "C42013B2-73B4-4942-87F3-4724E5D88592";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			try
			{
				var json = Intent.GetStringExtra("Payee");
				_payeeToEdit = JsonConvert.DeserializeObject<Payee>(json);
			}
			catch
			{
				_payeeToEdit = null;
			}

			SetupView();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			if (_doesPayeeHavePendingPayments != null)
			{
				var json = JsonConvert.SerializeObject(_doesPayeeHavePendingPayments);
				outState.PutString("DoesPayeeHavePendingPayments", json);
			}

			base.OnSaveInstanceState(outState);
		}

		protected override void OnRestoreInstanceState(Bundle savedInstanceState)
		{
			var json = savedInstanceState.GetString("DoesPayeeHavePendingPayments");
			_doesPayeeHavePendingPayments = JsonConvert.DeserializeObject<StatusResponse<bool>>(json);

			base.OnRestoreInstanceState(savedInstanceState);
		}

		public override void SetupView()
		{
			SetContentView(Resource.Layout.BillPayAddPayee);

			//translation text
			var updatePayeeTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "B4E716B8-BAE6-4455-B875-6E9E85AB9959", "Update Payee");
			var addPayeeTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "56FD0B81-DF1D-43C7-BF1E-5C51E79378CD", "Add Payee");
			var selectStateTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "BB9C9346-C5C2-4CF7-8A93-AE92D35DE656", "Select State");
			var daysTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "E778DD5B-03A2-49B8-9C94-2C5B3ED4E246", "Days");
			var updateTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "05C52C09-DB35-46DB-BA5F-1C43E637BE80", "Update");
			var addTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "5B79EC78-2D23-4E79-B85E-42EA2E16A1E3", "Add");

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
			txtTitle.Text = null != _payeeToEdit && _payeeToEdit.MemberId != 0 ? updatePayeeTxt : addPayeeTxt;

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			txtPayeeName = FindViewById<EditText>(Resource.Id.txtPayeeName);
			txtPayeeNickname = FindViewById<EditText>(Resource.Id.txtPayeeNickname);
			txtPayeeAddress1 = FindViewById<EditText>(Resource.Id.txtPayeeAddress1);
			txtPayeeAddress2 = FindViewById<EditText>(Resource.Id.txtPayeeAddress2);
			txtPayeeCity = FindViewById<EditText>(Resource.Id.txtPayeeCity);
			spinnerState = FindViewById<Spinner>(Resource.Id.spinnerState);
			spinnerState.Prompt = selectStateTxt;
			txtPayeeZip = FindViewById<EditText>(Resource.Id.txtPayeeZip);
			txtPayeePhone = FindViewById<EditText>(Resource.Id.txtPayeePhone);
			txtPayeeNameOnBill = FindViewById<EditText>(Resource.Id.txtPayeeNameOnBill);
			txtPayeeAccountNumber = FindViewById<EditText>(Resource.Id.txtPayeeAccountNumber);
			lblPayeeDeliveryTime = FindViewById<TextView>(Resource.Id.lblPayeeDeliveryTime);
			lblNote = FindViewById<TextView>(Resource.Id.lblNote);
			btnAddPayee = FindViewById<TextView>(Resource.Id.txtAddPayee);
			btnAddPayee.Text = null != _payeeToEdit && _payeeToEdit.MemberId != 0 ? updateTxt : addTxt;
			btnAddPayee.Click += (sender, e) => UpdatePayee();
			btnActive = FindViewById<Button>(Resource.Id.btnActive);
			btnInactive = FindViewById<Button>(Resource.Id.btnInactive);
			segmentedActiveInactive = FindViewById<SegmentedGroup>(Resource.Id.segmentActiveInactive);

			lblPayeeName = FindViewById<TextView>(Resource.Id.lblPayeeName);
			lblPayeeNickname = FindViewById<TextView>(Resource.Id.lblPayeeNickname);
			lblPayeeAddress1 = FindViewById<TextView>(Resource.Id.lblPayeeAddress1);
			lblPayeeAddress2 = FindViewById<TextView>(Resource.Id.lblPayeeAddress2);
			lblPayeeCity = FindViewById<TextView>(Resource.Id.lblPayeeCity);
			lblSpinnerState = FindViewById<TextView>(Resource.Id.lblPayeeState);
			lblPayeeZip = FindViewById<TextView>(Resource.Id.lblPayeeZip);
			lblPayeePhone = FindViewById<TextView>(Resource.Id.lblPayeePhone);
			lblPayeeNameOnBill = FindViewById<TextView>(Resource.Id.lblNameOnBill);
			lblPayeeAccountNumber = FindViewById<TextView>(Resource.Id.lblAccountNumber);
			lblDeliveryTime = FindViewById<TextView>(Resource.Id.lblDeliveryTime);
			lblPayeeActive = FindViewById<TextView>(Resource.Id.lblActive);

			_stateNames = new List<string>(USStates.USStateList.Values);
			_stateNames.Insert(0, selectStateTxt);

			var adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, _stateNames);
			spinnerState.Adapter = adapter;

			ClearAll();

			if (_payeeToEdit != null && _payeeToEdit.MemberId != 0)
			{
				txtPayeeName.Text = _payeeToEdit.PayeeName;
				txtPayeeName.Enabled = false;
				txtPayeeNickname.Text = _payeeToEdit.PayeeAlias;
				txtPayeeAddress1.Text = _payeeToEdit.Address1;
				txtPayeeAddress2.Text = _payeeToEdit.Address2;
				txtPayeeCity.Text = _payeeToEdit.City;

				string stateName;
				USStates.USStateList.TryGetValue(_payeeToEdit.State, out stateName);

				if (!string.IsNullOrEmpty(stateName))
				{
					spinnerState.SetSelection(_stateNames.IndexOf(stateName));
				}

				txtPayeeZip.Text = _payeeToEdit.PostalCode;
				txtPayeePhone.Text = _payeeToEdit.Phone;
				txtPayeeNameOnBill.Text = _payeeToEdit.NameOnAccount;
				txtPayeeAccountNumber.Text = _payeeToEdit.PayeeAccountNumber;
				lblPayeeDeliveryTime.Text = string.Format("{0} {1}", _payeeToEdit.DeliveryDays, daysTxt);

				if (_payeeToEdit.Active)
				{
					segmentedActiveInactive.Check(btnActive.Id);
				}
				else
				{
					segmentedActiveInactive.Check(btnInactive.Id);
				}

				CheckForPendingPayments();
			}
			else
			{
				spinnerState.SetSelection(-1);
				lblPayeeDeliveryTime.Text = "6 " + daysTxt;
				lblNote.Text = "";
			}			
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                CultureTextProvider.SetMobileResourceText(lblPayeeName, cultureViewId, "32958A0D-BB47-4AA6-901D-3949A57EBE25");
                CultureTextProvider.SetMobileResourceText(lblPayeeNickname, cultureViewId, "3D82BBCE-8883-40A1-AA40-D422EEBEFA46");
                CultureTextProvider.SetMobileResourceText(lblPayeeAddress1, cultureViewId, "F2E062D5-EBD6-4C17-B39E-CFAB7FC6D4A4");
                CultureTextProvider.SetMobileResourceText(lblPayeeAddress2, cultureViewId, "5C8386BA-632D-4BCB-92CE-2810C50361C0");
                CultureTextProvider.SetMobileResourceText(lblPayeeCity, cultureViewId, "0302C778-CC67-4B58-9891-DA533B7BEA3C");
                CultureTextProvider.SetMobileResourceText(lblSpinnerState, cultureViewId, "A52BCD65-567F-4A49-A233-0756B91996B7");
                CultureTextProvider.SetMobileResourceText(lblPayeeZip, cultureViewId, "3A9A1B88-6F81-40BA-8A5A-B6BDF1C24C98");
                CultureTextProvider.SetMobileResourceText(lblPayeePhone, cultureViewId, "352BACEC-B46C-4296-B809-E8B6CA0BF12C");
                CultureTextProvider.SetMobileResourceText(lblPayeeNameOnBill, cultureViewId, "4FF51913-0BB8-43AE-9135-60A03C5F4001");
                CultureTextProvider.SetMobileResourceText(lblPayeeAccountNumber, cultureViewId, "32496C74-67FC-4B91-B960-9E5301DF2F68");
                CultureTextProvider.SetMobileResourceText(lblDeliveryTime, cultureViewId, "49AAA7D8-5FD5-4A4D-9A03-0B04EB4F9BEA");
                CultureTextProvider.SetMobileResourceText(lblPayeeActive, cultureViewId, "65BF6355-7D3B-4D4E-A0F1-D9EA155B503F");
                CultureTextProvider.SetMobileResourceText(btnInactive, cultureViewId, "B44F5732-E9CE-40AF-AB30-6B40EAD0CDCE");
                CultureTextProvider.SetMobileResourceText(btnActive, cultureViewId, "65BF6355-7D3B-4D4E-A0F1-D9EA155B503F");
            }
            catch (Exception ex)
			{
				Logging.Log(ex, "UpdatePayeeActivity:SetCultureConfiguration");
			}
		}

		private async void CheckForPendingPayments()
		{
			var request = new DoesPayeeHavePendingPaymentsRequest
			{
				MemberId = GeneralUtilities.GetMemberIdAsInt(),
				MemberPayeeId = _payeeToEdit.MemberPayeeId
			};

			var methods = new BillPayMethods();

			ShowActivityIndicator();

			if (_doesPayeeHavePendingPayments == null)
			{
				_doesPayeeHavePendingPayments = await methods.DoesPayeeHavePendingPayments(request, this);
			}

			HideActivityIndicator();

			if (_doesPayeeHavePendingPayments != null && _doesPayeeHavePendingPayments.Success && _doesPayeeHavePendingPayments.Result)
			{
				txtPayeeName.Enabled = false;
				txtPayeeAddress1.Enabled = false;
				txtPayeeAddress2.Enabled = false;
				txtPayeeCity.Enabled = false;
				spinnerState.Enabled = false;
				txtPayeeZip.Enabled = false;
				txtPayeePhone.Enabled = false;
				txtPayeeNameOnBill.Enabled = false;
				txtPayeeAccountNumber.Enabled = false;
				lblPayeeDeliveryTime.Enabled = false;
				segmentedActiveInactive.Enabled = false;
				CultureTextProvider.SetMobileResourceText(lblNote, cultureViewId, "E1CC57C7-90E4-4AF2-99B5-C4B059414071");
			}
		}

		private void ClearAll()
		{
			txtPayeeName.Text = string.Empty;
			txtPayeeNickname.Text = string.Empty;
			txtPayeeAddress1.Text = string.Empty;
			txtPayeeAddress2.Text = string.Empty;
			txtPayeeCity.Text = string.Empty;
			txtPayeeZip.Text = string.Empty;
			txtPayeePhone.Text = string.Empty;
			txtPayeeNameOnBill.Text = string.Empty;
			txtPayeeAccountNumber.Text = string.Empty;
			lblNote.Text = "";
			lblPayeeDeliveryTime.Text = "";
			segmentedActiveInactive.Check(btnActive.Id);
		}

		private Payee PopulatePayee()
		{
			var payee = new Payee();

			if (_payeeToEdit != null && _payeeToEdit.MemberId != 0)
			{
				payee.MemberPayeeId = _payeeToEdit.MemberPayeeId;
			}

			payee.Active = (segmentedActiveInactive.CheckedRadioButtonId == btnActive.Id);
			payee.Address1 = txtPayeeAddress1.Text;
			payee.Address2 = txtPayeeAddress2.Text;
			payee.City = txtPayeeCity.Text;
			payee.DeliveryDays = lblPayeeDeliveryTime.Text;
			payee.MemberId = GeneralUtilities.GetMemberIdAsInt();
			payee.NameOnAccount = txtPayeeNameOnBill.Text;
			payee.PayeeAccountNumber = txtPayeeAccountNumber.Text;
			payee.PayeeAlias = txtPayeeNickname.Text;
			payee.PayeeName = txtPayeeName.Text;
			payee.Phone = txtPayeePhone.Text;
			payee.PostalCode = txtPayeeZip.Text;

			payee.State = spinnerState.SelectedItem.ToString();
			var stateAbbreviation = USStates.USStateList.FirstOrDefault(x => x.Value == payee.State).Key;

			if (!string.IsNullOrEmpty(stateAbbreviation))
			{
				payee.State = stateAbbreviation;
			}

			return payee;
		}

		private async void UpdatePayee()
		{
			//Translation stuff
			var okTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "", "OK");
			var errorUpdatingPayeeTxt = CultureTextProvider.GetMobileResourceText(cultureViewId, "9F8D3FC3-8622-4F0A-884B-4A57E9A56143", "Error updating payee.");

			StatusResponse<Payee> response;

			var payee = PopulatePayee();

			var methods = new BillPayMethods();

			ShowActivityIndicator();

			if (_payeeToEdit != null && _payeeToEdit.MemberId != 0)
			{
				response = await methods.UpdatePayee(payee, this);
			}
			else
			{
				response = await methods.AddPayee(payee, this);
			}

			HideActivityIndicator();

			if (response != null && response.Success)
			{
				var intent = new Intent();
				intent.PutExtra("Refresh", true);
				SetResult(Result.Ok, intent);
				Finish();
			}
			else if (response != null && !response.Success)
			{
				await AlertMethods.Alert(this, "SunMobile", response.FailureMessage, okTxt);
			}
			else
			{
				await AlertMethods.Alert(this, "SunMobile", errorUpdatingPayeeTxt, okTxt);
			}
		}
	}
}