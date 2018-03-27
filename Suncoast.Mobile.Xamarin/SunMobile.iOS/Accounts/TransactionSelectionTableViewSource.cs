using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.Logging;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public class TransactionSelectionTableViewSource : UITableViewSource
	{
		public List<Transaction> SelectedItems { get; set; }
		public event Action<bool, Transaction> AddRemove = delegate { };
		public Func<Transaction, bool> IsInList;
		private readonly TextViewTableSource _textViewTableSource = null;
		public event Action<ListViewItem> ItemSelected = delegate { };

		public TransactionSelectionTableViewSource(TextViewTableSource model)
		{
			_textViewTableSource = model;
			SelectedItems = new List<Transaction>();
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
			const int disclosureWidth = 35;
			const int space = 4;

			var item = _textViewTableSource.Items[indexPath.Row];

			var cell = tableView.DequeueReusableCell("cellMain");

			var imageCheck = (UIImageView)cell.ViewWithTag(300);
			var lblHeader = (UILabel)cell.ViewWithTag(100);
			var lblText1 = (UILabel)cell.ViewWithTag(200);
			var lblValue1 = (UILabel)cell.ViewWithTag(400);

			imageCheck.Image = UIImage.FromBundle(string.Empty);

			// I am manually laying these controls out because if I right anchor them, they will be uneven because some have disclosures and some do not.
			lblValue1.Frame = new CoreGraphics.CGRect(tableView.Bounds.Width - lblValue1.Frame.Width - disclosureWidth, lblValue1.Frame.Y, lblValue1.Frame.Width, lblValue1.Frame.Height);
			imageCheck.Frame = new CoreGraphics.CGRect(tableView.Bounds.Width - imageCheck.Frame.Width - lblValue1.Frame.Width - disclosureWidth, imageCheck.Frame.Y, imageCheck.Frame.Width, imageCheck.Frame.Height);
			lblText1.Frame = new CoreGraphics.CGRect(lblText1.Frame.X, lblText1.Frame.Y, imageCheck.Frame.X - lblText1.Frame.X - space, lblText1.Frame.Height);

			lblHeader.Text = item.HeaderText;
			lblText1.Text = item.Item1Text;
			lblValue1.Text = item.Value1Text;

			var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(item.Value1Text));

			decimal result;
			decimal.TryParse(amount, out result);
			lblValue1.TextColor = result < 0 ? UIColor.Red : UIColor.Black;

			if (IsInList((Transaction)item.Data))
			{
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			else
			{
				cell.Accessory = UITableViewCellAccessory.None;
			}

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			try
			{
				if (indexPath.Row >= 0 && indexPath.Row <= _textViewTableSource.Items.Count)
				{
					ItemSelected(_textViewTableSource.Items[indexPath.Row]);
					tableView.DeselectRow(indexPath, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionSelectionTableViewSource:RowSelected");
			}
		}
	}
}