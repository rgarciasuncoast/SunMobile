using System;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Transfers
{
	public partial class TransferAnyMemberTableViewController : BaseTableViewController
	{
		private string suffixText;
		private string last4Text;

		public event Action<AnyMemberInfo> AnyMemberSelected = delegate { };

		public TransferAnyMemberTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();			

			// Hides the remaining rows.
			tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			var doneText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "D8E6C34F-BA33-4439-AA67-EE6A729A3259", "Submit");

			var rightButton = new UIBarButtonItem(doneText, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => Submit();

			txtAccount.EditingChanged += (sender, e) => Validate();
			txtAccount.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 10;
			};

			txtSuffix.EditingChanged += (sender, e) => Validate();
			txtSuffix.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 4;
			};

			txtLastName.EditingChanged += (sender, e) => Validate();
			txtLastName.ShouldReturn += TextFieldShouldReturn;
			txtLastName.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 40;
			};

			accountTypeSegmentControl.ValueChanged += AccountTypeChanged;

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
						accountTypeSegmentControl.SelectedSegment = 1;
						break;
					case "CreditCards":
						accountTypeSegmentControl.SelectedSegment = 2;
						break;
					default:
						accountTypeSegmentControl.SelectedSegment = 0;
						break;
				}

				Validate();
			}

			txtAccount.BecomeFirstResponder();
		}

		public override void SetCultureConfiguration()
		{
			last4Text = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "734B5E85-CE70-47AD-BC2E-57E489C5E815", "Last 4 of Credit Card");
            suffixText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "A72AB6BD-9BCC-4F94-9188-E9C10622661A", "Suffix");
			CultureTextProvider.SetMobileResourceText(lblSuffix, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "A72AB6BD-9BCC-4F94-9188-E9C10622661A", "Suffix");
			CultureTextProvider.SetMobileResourceText(lblAccount, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "D7FBE45D-4051-49D4-8611-FC9F20371E7A", "Member Number");
			CultureTextProvider.SetMobileResourceText(lblLastName, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "9E616841-EA09-437F-9DB3-2FC2245255B9", "Last Name");
			var shareText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "658949D5-CD88-4783-9DAD-F33D016AA845", "Share");
			var loanText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "E7592204-E593-46C6-8AE4-BEF472381553", "Loan");
			var ccText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "5FF200AE-12F2-4FFA-95D5-89794C43894F", "Credit Card");
			accountTypeSegmentControl.SetTitle(shareText, 0);
			accountTypeSegmentControl.SetTitle(loanText, 1);
			accountTypeSegmentControl.SetTitle(ccText, 2);
		}

		private bool TextFieldShouldReturn(UITextField textfield)
		{
			if (textfield == txtAccount)
			{
				txtSuffix.BecomeFirstResponder();
			}

			if (textfield == txtSuffix)
			{
				txtLastName.BecomeFirstResponder();
			}

			if (textfield == txtLastName)
			{
				Submit();
			}

			return false;
		}

		private void AccountTypeChanged(object sender, EventArgs e)
		{
			var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;

			lblSuffix.Text = selectedSegmentId == 2 ? last4Text : suffixText;

			Validate();
		}

		private void ClearAll()
		{
			txtAccount.Text = string.Empty;
			txtSuffix.Text = string.Empty;
			txtLastName.Text = string.Empty;			
			accountTypeSegmentControl.SelectedSegment = 0;
		}

		private AnyMemberInfo PopulateAnyMemberInfo()
		{
			var anyMemberInfo = new AnyMemberInfo();

			anyMemberInfo.IsAnyMember = true;
			anyMemberInfo.Account = txtAccount.Text;

			switch (accountTypeSegmentControl.SelectedSegment)
			{
				case (0):
					anyMemberInfo.AccountType = "Shares";
					break;
				case (1):
					anyMemberInfo.AccountType = "Loans";
					break;
				case (2):
					anyMemberInfo.AccountType = "CreditCards";
					break;
			}

			anyMemberInfo.IsJoint = false;
			anyMemberInfo.LastName = txtLastName.Text;
			anyMemberInfo.Suffix = txtSuffix.Text;

			return anyMemberInfo;
		}

		private void Validate()
		{
			var methods = new AccountMethods();
			NavigationItem.RightBarButtonItem.Enabled = methods.ValidateAnyMemberTransfer(PopulateAnyMemberInfo());
		}

		private void Submit()
		{
			var anyMemberInfo = PopulateAnyMemberInfo();
			RetainedSettings.Instance.AnyMemberInfo = anyMemberInfo;
			NavigationController.PopViewController(false);
			AnyMemberSelected(anyMemberInfo);
		}
	}
}