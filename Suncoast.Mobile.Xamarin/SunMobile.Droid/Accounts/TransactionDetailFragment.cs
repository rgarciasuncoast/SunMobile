using System;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Extensions;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;

namespace SunMobile.Droid.Accounts
{
	public class TransactionDetailFragment : BaseFragment
	{
		public event Action<ListViewItem> ViewCheckSelected = delegate { };
		public event Action<ListViewItem> DisputeSelected = delegate { };
		public Transaction SelectedTransaction { get; set; }
		public string SelectedMemberId { get; set; }
		public bool IsCreditCard { get; set; }
        public TransactionDisputeRestrictions DisputeRestrictions { get; set; }

		private TextView txtTransactionDate;
		private TextView txtDescription;
		private TextView txtAmount;
		private Button btnViewCheck;
		private Button btnDispute;
		private TextView lblTransactionDate;
		private TextView lblDescription;
		private TextView lblAmount;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AccountTransactionDetailView, null);
			RetainInstance = true;

			if (savedInstanceState != null)
			{
				string json = savedInstanceState.GetString("Transaction");
				SelectedTransaction = JsonConvert.DeserializeObject<Transaction>(json);
				SelectedMemberId = savedInstanceState.GetString("MemberId");
				IsCreditCard = savedInstanceState.GetBoolean("IsCreditCard");
			}

			return view;
		}

        public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "44209182-896C-45E8-9359-FB118F940271", "Transaction Details"));
                CultureTextProvider.SetMobileResourceText(lblTransactionDate, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "0516EDB5-7973-4D10-9017-CB38AC106421", "Transaction Date:");
                CultureTextProvider.SetMobileResourceText(lblDescription, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "17F62DB7-2CF9-4B08-99E9-C299EAEC8752", "Description:");
                CultureTextProvider.SetMobileResourceText(lblAmount, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "C047552C-6C0D-4DFC-8841-A1AD56C127B4", "Amount:");
                CultureTextProvider.SetMobileResourceText(btnViewCheck, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "CB26E0E2-30CC-4225-9936-32ECCA4D67A0", "View Check");
                CultureTextProvider.SetMobileResourceText(btnDispute, "AB401808-AC62-4A09-B4E9-C8FE740CD699", "6E08242D-1C53-426F-98F3-957E59BF4F54", "Dispute");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDetailFragment:SetCultureConfiguration");
			}
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			string json = JsonConvert.SerializeObject(SelectedTransaction);
			outState.PutString("Transaction", json);
			outState.PutString("MemberId", SelectedMemberId);
			outState.PutBoolean("IsCreditCard", IsCreditCard);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Transaction Detail");

			txtTransactionDate = Activity.FindViewById<TextView>(Resource.Id.txtTransactionDate);
			txtDescription = Activity.FindViewById<TextView>(Resource.Id.txtDescription);
			txtAmount = Activity.FindViewById<TextView>(Resource.Id.txtAmount);
			btnViewCheck = Activity.FindViewById<Button>(Resource.Id.btnViewCheck);
			btnDispute = Activity.FindViewById<Button>(Resource.Id.btnDispute);
			lblTransactionDate = Activity.FindViewById<TextView>(Resource.Id.lblTransactionDate);
			lblDescription = Activity.FindViewById<TextView>(Resource.Id.lblDescription);
			lblAmount = Activity.FindViewById<TextView>(Resource.Id.lblAmount);

			txtTransactionDate.Text = string.Format("{0:MM/dd/yyyy}", SelectedTransaction.PostingDate.UtcToEastern());
			txtDescription.Text = SelectedTransaction.Description;
			txtAmount.Text = StringUtilities.FormatAsCurrency(SelectedTransaction.TransactionAmount.ToString());

			if (SelectedTransaction.TransactionAmount < 0)
			{
				txtAmount.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.TextViewTextColorRed)));
			}

			var accountMethods = new AccountMethods();
			var disputeInfoResponse = accountMethods.GetDisputeInfo(SelectedTransaction, SelectedMemberId, IsCreditCard);


			if (disputeInfoResponse != null)
			{
				btnDispute.Enabled = (disputeInfoResponse.AllowDispute);
			}
			else
			{
				btnDispute.Enabled = false;
			}

			btnViewCheck.Enabled = ((!string.IsNullOrEmpty(SelectedTransaction.CheckNumber)) || (!string.IsNullOrEmpty(SelectedTransaction.TraceNumber)) && !IsCreditCard);


			btnDispute.Click += async (sender, e) =>
			{
                if (DisputeRestrictions != null && DisputeRestrictions.IsFrequentDisputer)
                {
                    var message = DisputeRestrictions.FrequentDisputeInstructions;
                    await AlertMethods.Alert(Activity, "SunMobile", message, "OK");
                }
                else
                {
                    var listViewItem = new ListViewItem();
                    listViewItem.Data = SelectedTransaction;
                    NavigationService.NavigatePop(false);
                    DisputeSelected(listViewItem);
                }
			};

			btnViewCheck.Click += (sender, e) =>
			{
				var listViewItem = new ListViewItem();
				listViewItem.MoreIconVisible = true;
				listViewItem.Data = SelectedTransaction; NavigationService.NavigatePop(false);
				ViewCheckSelected(listViewItem);
			};			
		}
	}
}