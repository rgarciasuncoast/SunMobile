using System;
using Foundation;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using UIKit;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public partial class SubAccountsFundingViewController : SubAccountsBaseContentViewController, ISubAccountsView
	{
        private SubAccountsFundingAccountListResponse _model;
        private Account _account;
        private const decimal MAX_TRANSFER_AMOUNT = 10_000;

        public SubAccountsFundingViewController(IntPtr handle) : base(handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();			

			txtAmount.EditingChanged += (sender, e) =>
			{
				txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((UITextField)sender).Text);				
			};

			txtAmount.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 15;
			};

			var tapGesture = new UITapGestureRecognizer();
            tapGesture.AddTarget(() => SelectAccount());
            viewAccount.AddGestureRecognizer(tapGesture);			

            ClearAll();

            LoadFundingInformation();
        }

		private async void LoadFundingInformation()
		{
			var methods = new AccountMethods();

			if (_model == null)
			{
				ShowActivityIndicator();

				_model = await methods.SubAccountsFundingAccountList(null, View);

				HideActivityIndicator();
			}

			if (_model?.AccountListViewModel?.ClientViewState != null &&
				_model.AccountListViewModel.ClientViewState == "SubAccountsFundingAccountList")

			{
				txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(_model.RequiredFundingAmount.ToString());				
			}
			else
			{
				_model = null;
			}
		}

		private void SelectAccount()
		{
			var selectAccountViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectAccountViewController") as SelectAccountViewController;

            selectAccountViewController.AccountListType = AccountListTypes.FundingAccounts;
            selectAccountViewController.ShowJoints = false;
			selectAccountViewController.ShowAnyMember = false;			

			selectAccountViewController.AccountSelected += listViewItem =>
			{
				_account = (Account)listViewItem.Data;
                ((SubAccountsViewController)ParentViewController.ParentViewController).FundingSuffix = _account.Suffix;
				
				lblSource.Text = listViewItem.HeaderText;
				lblSource2.Text = listViewItem.Header2Text;
				lblSourceText1.Text = listViewItem.Item1Text;
				lblSourceText2.Text = listViewItem.Item2Text;
				lblSourceValue1.Text = listViewItem.Value1Text;
				lblSourceValue2.Text = listViewItem.Value2Text;								
			};			

			NavigationController.PushViewController(selectAccountViewController, true);
		}

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            txtAmount.ResignFirstResponder();
        }		

		private void ClearAll()
		{
			lblSource.Text = string.Empty;
			lblSource2.Text = string.Empty;
			lblSourceText1.Text = string.Empty;
			lblSourceText2.Text = string.Empty;
			lblSourceValue1.Text = string.Empty;
			lblSourceValue2.Text = string.Empty;			
		}

		public string Validate()
		{
            var returnValue = string.Empty;

            if (_account == null)
            {
                returnValue = "A funding account must be selected.";
            }

            decimal fundingAmount = 0;
            decimal.TryParse(StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(txtAmount.Text)), out fundingAmount);

            ((SubAccountsViewController)ParentViewController.ParentViewController).FundingAmount = fundingAmount;

            if (fundingAmount < _model.RequiredFundingAmount)
            {
                returnValue += $"\nFunding amount must be greater than ${_model.RequiredFundingAmount}.";
            }

            if (_account != null && fundingAmount > _account.AvailableBalance)
            {
                returnValue += $"\nThe Funding amount exceeds the available balance of the selected account.";
            }
            else if (fundingAmount >= MAX_TRANSFER_AMOUNT)
            {
                returnValue += $"\nThe Funding amount must be less than {StringUtilities.FormatAsCurrency(MAX_TRANSFER_AMOUNT.ToString())}.";
            }

            return returnValue;
		}
	}
}