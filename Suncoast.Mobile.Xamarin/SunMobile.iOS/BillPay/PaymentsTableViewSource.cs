using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.BillPay
{
	public class PaymentsTableViewSource : UITableViewSource
	{
		private readonly TextViewTableSource _textViewTableSource = null;
		public event Action<ListViewItem> ItemSelected = delegate{};

		public PaymentsTableViewSource(List<Payment> model, bool isPending)
		{
            _textViewTableSource = ViewUtilities.ConvertPaymentListTextViewTableSource(model, isPending);		
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
			var item = _textViewTableSource.Items[indexPath.Row];

			var cell = tableView.DequeueReusableCell("cellMain");

			var lblDate = (UILabel)cell.ViewWithTag(100);
			var lblPayee = (UILabel)cell.ViewWithTag(200);			
			var lblAmount = (UILabel)cell.ViewWithTag(400);			
            var lblStatus = (UILabel)cell.ViewWithTag(300);

			lblDate.Text = item.Value2Text;
			lblPayee.Text = item.HeaderText;			
			lblAmount.Text = item.Value1Text;
            lblStatus.Text = item.Value3Text;			

			cell.Accessory = item.MoreIconVisible ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;

			var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(item.Value1Text));

			decimal result;
			decimal.TryParse(amount, out result);
			lblAmount.TextColor = result < 0 ? UIColor.Red : UIColor.Black;

            lblStatus.TextColor = item.Value4Text == "103" ? UIColor.Red : UIColor.Black;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(_textViewTableSource.Items[indexPath.Row]);

			tableView.DeselectRow(indexPath, true);
		}
	}
}