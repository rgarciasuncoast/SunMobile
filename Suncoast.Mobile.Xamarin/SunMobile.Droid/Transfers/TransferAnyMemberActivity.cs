using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using SunMobile.Droid.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid
{
	[Activity(Label = "TransferAnyMemberActivity", Theme = "@style/CustomHoloLightTheme")]
	public class TransferAnyMemberActivity : BaseActivity
	{
		private ImageButton btnCloseWindow;
		private Button btnSutmit;
		private EditText txtAccount;
		private TextView txtTitle;
		private TextView lblAccount;
		private TextView lblSuffix;
		private TextView lblName;
		private EditText txtSuffix;
		private EditText txtLastName;
		private RadioButton radioTypeShare;
		private RadioButton radioTypeLoan;
		private RadioButton radioTypeCreditCard;

		private string suffixText;
		private string last4Text;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetupView();			
		}

		public override void SetupView()
		{
			SetContentView(Resource.Layout.AccountTransferAnyMemberView);

			Title = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "9D59AD3D-BFC3-44EF-83F4-FEF687D9429F", "Transfers");

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();
			btnSutmit = FindViewById<Button>(Resource.Id.btnSubmit);
			btnSutmit.Click += (sender, e) => Submit();
			txtAccount = FindViewById<EditText>(Resource.Id.txtAccount);
			txtAccount.AfterTextChanged += (sender, e) => Validate();
			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
			lblAccount = FindViewById<TextView>(Resource.Id.lblAccount);
			lblSuffix = FindViewById<TextView>(Resource.Id.lblSuffix);
			lblName = FindViewById<TextView>(Resource.Id.lblName);
			txtSuffix = FindViewById<EditText>(Resource.Id.txtSuffix);
			txtSuffix.AfterTextChanged += (sender, e) => Validate();
			txtLastName = FindViewById<EditText>(Resource.Id.txtName);
			txtLastName.AfterTextChanged += (sender, e) => Validate();
			radioTypeShare = FindViewById<RadioButton>(Resource.Id.radTypeShare);
			radioTypeShare.CheckedChange += AccountTypeChanged;
			radioTypeLoan = FindViewById<RadioButton>(Resource.Id.radTypeLoan);
			radioTypeLoan.CheckedChange += AccountTypeChanged;
			radioTypeCreditCard = FindViewById<RadioButton>(Resource.Id.radTypeCreditCard);
			radioTypeCreditCard.CheckedChange += AccountTypeChanged;

			ClearAll();

			var anyMemberInfo = RetainedSettings.Instance.AnyMemberInfo;

			if (anyMemberInfo != null)
			{
				txtAccount.Text = anyMemberInfo.Account;
				txtSuffix.Text = anyMemberInfo.Suffix;
				txtLastName.Text = anyMemberInfo.LastName;

				switch (anyMemberInfo.AccountType)
				{
					case "Loans":
						radioTypeLoan.Checked = true;
						break;
					case "CreditCards":
						radioTypeCreditCard.Checked = true;
						break;
					default:
						radioTypeShare.Checked = true;
						break;
				}
			}

		}

		public override void SetCultureConfiguration()
		{
            try
            {
                last4Text = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "734B5E85-CE70-47AD-BC2E-57E489C5E815", "Last 4 of Credit Card");
                suffixText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "A72AB6BD-9BCC-4F94-9188-E9C10622661A", "Suffix");
                CultureTextProvider.SetMobileResourceText(lblSuffix, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "A72AB6BD-9BCC-4F94-9188-E9C10622661A", "Suffix");
                CultureTextProvider.SetMobileResourceText(txtTitle, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "B2B8B00A-1852-48D3-BF0A-D3845FE3CAD9", "Other Member");
                CultureTextProvider.SetMobileResourceText(lblAccount, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "D7FBE45D-4051-49D4-8611-FC9F20371E7A", "Member Number");
                CultureTextProvider.SetMobileResourceText(lblName, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "9E616841-EA09-437F-9DB3-2FC2245255B9", "Last Name");
                CultureTextProvider.SetMobileResourceText(radioTypeShare, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "658949D5-CD88-4783-9DAD-F33D016AA845", "Share");
                CultureTextProvider.SetMobileResourceText(radioTypeLoan, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "E7592204-E593-46C6-8AE4-BEF472381553", "Loan");
                CultureTextProvider.SetMobileResourceText(radioTypeCreditCard, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "5FF200AE-12F2-4FFA-95D5-89794C43894F", "Credit Card");
                CultureTextProvider.SetMobileResourceText(btnSutmit, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "D8E6C34F-BA33-4439-AA67-EE6A729A3259", "Submit");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "TransferAnyMemberActivity:SetCultureConfiguration");
			}
		}

		private void AccountTypeChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (e.IsChecked)
			{
				lblSuffix.Text = sender == radioTypeShare || sender == radioTypeLoan ? suffixText : last4Text;
			}
		}

		private void ClearAll()
		{
			txtAccount.Text = string.Empty;
			txtSuffix.Text = string.Empty;
			txtLastName.Text = string.Empty;			
			radioTypeShare.Checked = true;
		}

		private AnyMemberInfo PopulateAnyMemberInfo()
		{
			var anyMemberInfo = new AnyMemberInfo();

			anyMemberInfo.IsAnyMember = true;
			anyMemberInfo.Account = txtAccount.Text;

			if (radioTypeShare.Checked)
			{
				anyMemberInfo.AccountType = "Shares";
			}
			else if (radioTypeLoan.Checked)
			{
				anyMemberInfo.AccountType = "Loans";
			}
			else if (radioTypeCreditCard.Checked)
			{
				anyMemberInfo.AccountType = "CreditCards";
			}

			anyMemberInfo.LastName = txtLastName.Text;
			anyMemberInfo.Suffix = txtSuffix.Text;

			return anyMemberInfo;
		}

		private void Validate()
		{
			var methods = new AccountMethods();
			btnSutmit.Enabled = methods.ValidateAnyMemberTransfer(PopulateAnyMemberInfo());
		}

		private void Submit()
		{
			var anyMemberInfo = PopulateAnyMemberInfo();
			RetainedSettings.Instance.AnyMemberInfo = anyMemberInfo;
			var json = JsonConvert.SerializeObject(anyMemberInfo);
			var intent = new Intent();
			intent.PutExtra("AnyMemberInfo", json);
			SetResult(Result.Ok, intent);
			Finish();
		}
	}
}