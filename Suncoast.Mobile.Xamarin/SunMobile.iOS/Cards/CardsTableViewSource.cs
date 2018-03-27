using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Cards
{
	public class CardsTableViewSource : UITableViewSource
	{
		public event Action<List<BankCard>> ItemsSelected = delegate { };
		private readonly TextViewTableSource _textViewTableSource = null;
		private List<BankCard> _selectedItems;
		private bool _singleSelection;

		public CardsTableViewSource(List<BankCard> model, bool singleSelection)
		{
			_textViewTableSource = ViewUtilities.ConvertCardListTextViewTableSource(model);
			_selectedItems = new List<BankCard>();
			_singleSelection = singleSelection;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _textViewTableSource.Items.Count;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return tableView.RowHeight;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var listViewItem = _textViewTableSource.Items[indexPath.Row];

			var cell = tableView.DequeueReusableCell("cellMain");

			var bankCard = (BankCard)listViewItem.Data;

			var header = string.Empty;

			switch (bankCard.CardType)
			{
				case CardTypes.CreditCard:
					header = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "17783198-589D-44DC-A3F8-40A0E32522B9", "Credit Card");
					break;
				case CardTypes.ProprietaryCard:
					header = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "F02EAA6F-CB04-491A-8252-440DBAF9F856", "ATM Card");
					break;
				case CardTypes.VisaDebitCard:
					header = "Visa " + CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "F74D32D8-EFCA-4798-BF46-05175CFACE02", "Check Card");
					break;
			}

			var header2 = bankCard.CardAccountNumber;

			if (!string.IsNullOrWhiteSpace(bankCard.CardAccountNumber) && bankCard.CardAccountNumber.Length >= 4)
			{
				header2 = "xxxx-xxxx-xxxx-" + bankCard.CardAccountNumber.Substring(bankCard.CardAccountNumber.Length - 4, 4);
			}

			var lblHeader = (UILabel)cell.ViewWithTag(100);
			var lblAccount = (UILabel)cell.ViewWithTag(200);

			lblHeader.Text = header;
			lblAccount.Text = header2;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var listViewItem = _textViewTableSource.Items[indexPath.Row];

			if (!_singleSelection)
			{
				if (!listViewItem.IsChecked)
				{
					tableView.CellAt(indexPath).Accessory = UITableViewCellAccessory.Checkmark;
					_selectedItems.Add((BankCard)listViewItem.Data);
				}
				else
				{
					tableView.CellAt(indexPath).Accessory = UITableViewCellAccessory.None;
					_selectedItems.Remove((BankCard)listViewItem.Data);
				}

				_textViewTableSource.Items[indexPath.Row].IsChecked = !_textViewTableSource.Items[indexPath.Row].IsChecked;
			}
			else
			{
				_selectedItems.Add((BankCard)listViewItem.Data);
			}

			ItemsSelected(_selectedItems);
			tableView.DeselectRow(indexPath, true);
		}
	}
}