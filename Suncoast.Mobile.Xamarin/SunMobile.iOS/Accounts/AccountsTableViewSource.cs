using System;
using Foundation;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.Shared.Culture;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public class AccountsTableViewSource : UITableViewSource
	{
	    private readonly GroupedTextViewTableSource _textViewTableSource = null;
	    public event Action<ListViewItem> ItemSelected = delegate{};	
        private bool _shouldDeselectRow;

		public AccountsTableViewSource(AccountListTextViewModel model, bool includeJoints, bool shouldDeselectRow = true, bool includeRocketAccounts = false)
	    {
			_textViewTableSource = ViewUtilities.ConvertTextViewModelToGroupedTextViewTableSource(model, includeJoints, includeRocketAccounts);
            _shouldDeselectRow = shouldDeselectRow;
	    }

	    public override nint RowsInSection(UITableView tableview, nint section)
	    {
			return _textViewTableSource.Sections == null ? 0 : _textViewTableSource.Sections[_textViewTableSource.SectionTitles[Convert.ToInt32(section)]].ItemCount;
	    }

	    public override nint NumberOfSections(UITableView tableView)
	    {
	        return _textViewTableSource.SectionTitles == null ? 0 : _textViewTableSource.SectionTitles.Count;
	    }

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			return 28;
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			var view = new UIView(new CoreGraphics.CGRect(0, 0, 320, 28));
			var label = new UILabel();
			label.BackgroundColor = UIColor.Clear;
			label.Opaque = false;
			label.TextColor = AppStyles.TitleBarItemTintColor;
			label.Font = UIFont.FromName("Helvetica", 16f);
			label.Frame = new CoreGraphics.CGRect(15, 2, 290, 24);
			label.Text = _textViewTableSource.SectionTitles[Convert.ToInt32(section)];

			switch (label.Text) 
			{
				case "Deposits":
					view.BackgroundColor = AppStyles.DepositsHeaderBackgroundColor;
					break;
				case "Loans":
					view.BackgroundColor = AppStyles.LoansHeaderBackgroundColor;
					break;
				case "Credit Cards":
					view.BackgroundColor = AppStyles.CreditCardsHeaderBackgroundColor;
					break;
				default:
					view.BackgroundColor = AppStyles.BarTintColor;
					break;					
			}

			if (label.Text == "Deposits")
			{
				label.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits");
			}
			else if (label.Text == "Loans")
			{
				label.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "8F7B796C-9009-49B3-BC91-9F14CADAA936", "Loans");
			}
			else if (label.Text == "Credit Cards")
			{
				label.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "944713ED-0D8C-4997-8FAD-4B8D45BB10F0", "Credit Cards");
			}

			view.AddSubview(label);

			return view;
		}

	    public override string TitleForHeader(UITableView tableView, nint section)
	    {
			return _textViewTableSource.SectionTitles[Convert.ToInt32(section)];
	    }

	    public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
	    {
			var item = _textViewTableSource.Sections[_textViewTableSource.SectionTitles[indexPath.Section]].ListViewItems[indexPath.Row];

			if (((Account)item.Data).AccountType == "RocketAccount")
			{
				return 60;
			}
			else
			{
				return item.Value3Text == string.Empty ? 79 : 112;
			}
	    }

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = _textViewTableSource.Sections[_textViewTableSource.SectionTitles[indexPath.Section]].ListViewItems[indexPath.Row];
			var account = (Account)item.Data;

			var cell = tableView.DequeueReusableCell("cellMain");

			var lblHeader = (UILabel)cell.ViewWithTag(100);
			//lblHeader.Font = UIFont.PreferredSubheadline;
			var lblHeader2 = (UILabel)cell.ViewWithTag(200);
			//lblHeader2.Font = UIFont.PreferredSubheadline;
			var lblText1 = (UILabel)cell.ViewWithTag(300); //
			//lblText1.Font = UIFont.PreferredCaption1;
			var lblValue1 = (UILabel)cell.ViewWithTag(400);
			//lblValue1.Font = UIFont.PreferredCaption1;
			var lblText2 = (UILabel)cell.ViewWithTag(500); //
			//lblText2.Font = UIFont.PreferredCaption1;
			var lblValue2 = (UILabel)cell.ViewWithTag(600);
			//lblValue2.Font = UIFont.PreferredCaption1;
			var lblText3 = (UILabel)cell.ViewWithTag(700); //
			//lblText3.Font = UIFont.PreferredCaption1;
			var lblValue3 = (UILabel)cell.ViewWithTag(800);
			//lblValue3.Font = UIFont.PreferredCaption1;
			var lblText4 = (UILabel)cell.ViewWithTag(900); //
			//lblText4.Font = UIFont.PreferredCaption1;
			var lblValue4 = (UILabel)cell.ViewWithTag(1000);
			//lblValue4.Font = UIFont.PreferredCaption1;			

			lblHeader.Text = item.HeaderText;
			lblHeader2.Text = item.Header2Text;
			lblText1.Text = item.Item1Text;
			lblText2.Text = item.Item2Text;
			lblText3.Text = item.Item3Text;
			lblText4.Text = item.Item4Text;			
			lblValue1.Text = item.Value1Text;
			lblValue2.Text = item.Value2Text;
			lblValue3.Text = item.Value3Text;
			lblValue4.Text = item.Value4Text;

            // Accessibility
            if (!string.IsNullOrEmpty(lblText1.Text))
            {
                lblText1.AccessibilityLabel = lblText1.Text + lblValue1.Text;
                lblValue1.AccessibilityLabel = string.Empty;
            }
            if(!string.IsNullOrEmpty(lblText2.Text))
            {
                lblText2.AccessibilityLabel = lblText2.Text + lblValue2.Text;
                lblValue2.AccessibilityLabel = string.Empty;
            }
            if (!string.IsNullOrEmpty(lblText3.Text))
            {
                lblText3.AccessibilityLabel = lblText3.Text + lblValue3.Text;
                lblValue3.AccessibilityLabel = string.Empty;
            }
            if (!string.IsNullOrEmpty(lblText4.Text))
            {
                lblText4.AccessibilityLabel = lblText4.Text + lblValue4.Text;
                lblValue4.AccessibilityLabel = string.Empty;
            }

			switch (account.OwnershipType) 
			{
				case "Primary":
					cell.BackgroundColor = AppStyles.RegularAccountsBackgroundColor;
					break;
				case "Joint":
					cell.BackgroundColor = AppStyles.JointAccountsBackgroundColor;
					break;
				case "Secondary":
					cell.BackgroundColor = AppStyles.SecondaryAccountsBackgroundColor;
					break;				
				default:
					cell.BackgroundColor = AppStyles.RegularAccountsBackgroundColor;
					break;					
			}

			cell.Accessory = item.MoreIconVisible ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;

			var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(item.Value1Text));

			decimal result;
			decimal.TryParse(amount, out result);
			lblValue1.TextColor = result < 0 ? UIColor.Red : UIColor.Black;

			amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(item.Value2Text));

			decimal.TryParse(amount, out result);
			lblValue2.TextColor = result < 0 ? UIColor.Red : UIColor.Black;         

			return cell;
		}

	    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
	    {
	        ItemSelected(_textViewTableSource.Sections[_textViewTableSource.SectionTitles[indexPath.Section]].ListViewItems[indexPath.Row]);

            if (_shouldDeselectRow)
            {
                tableView.DeselectRow(indexPath, true);
            }
	    }
	}
}