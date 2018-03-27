using System;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Extensions;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public partial class TransactionDetailsTableViewController : BaseTableViewController
	{
		public event Action<ListViewItem> ViewCheckSelected = delegate { };
		public event Action<ListViewItem> DisputeSelected = delegate { };
		public Transaction SelectedTransaction { get; set; }
		public string SelectedMemberId { get; set; }
		public bool IsCreditCard { get; set; }
        public TransactionDisputeRestrictions DisputRestrictions { get; set; }

		public TransactionDetailsTableViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
            try
            {
                base.ViewDidLoad();

                lblTransactionDate.Text = string.Format("{0:MM/dd/yyyy}", SelectedTransaction.PostingDate.UtcToEastern());
                lblTransactionDescription.Text = SelectedTransaction.Description;
                lblTransactionAmount.Text = StringUtilities.FormatAsCurrency(SelectedTransaction.TransactionAmount.ToString());
                lblTransactionAmount.TextColor = SelectedTransaction.TransactionAmount < 0 ? UIColor.Red : UIColor.Black;

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

                btnDispute.Clicked += async (sender, e) =>
                {
                    if (DisputRestrictions != null && DisputRestrictions.IsFrequentDisputer)
                    {
                        var message = DisputRestrictions.FrequentDisputeInstructions;
                        await AlertMethods.Alert(View, "SunMobile", message, "OK");
                    }
                    else
                    {
                        var listViewItem = new ListViewItem();
                        listViewItem.Data = SelectedTransaction;
                        NavigationController.PopViewController(true);
                        DisputeSelected(listViewItem); 
                    }
                };

                btnViewCheck.Clicked += (sender, e) =>
                {
                    var listViewItem = new ListViewItem();
                    listViewItem.MoreIconVisible = true;
                    listViewItem.Data = SelectedTransaction;
                    NavigationController.PopViewController(true);
                    ViewCheckSelected(listViewItem);
                };
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "TransactionDetailsTableViewController:ViewDidLoad");
            }
		}

		public override void SetCultureConfiguration()
		{
			NavigationItem.Title = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "44209182-896C-45E8-9359-FB118F940271", "Transaction Details");
			lblDateLabel.Text = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "0516EDB5-7973-4D10-9017-CB38AC106421", "Transaction Date:");
			lblDescriptionLabel.Text = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "17F62DB7-2CF9-4B08-99E9-C299EAEC8752", "Description:");
			lblAmountLabel.Text = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "C047552C-6C0D-4DFC-8841-A1AD56C127B4", "Amount:");
			btnViewCheck.Title = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "CB26E0E2-30CC-4225-9936-32ECCA4D67A0", "View Check");
			btnDispute.Title = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "6E08242D-1C53-426F-98F3-957E59BF4F54", "Dispute");
		}
	}
}