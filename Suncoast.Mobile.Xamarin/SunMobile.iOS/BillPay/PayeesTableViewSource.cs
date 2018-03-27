using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.BillPay.V2;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.BillPay
{
    /*
	public class PayeesTableViewSource : UITableViewSource
	{
		private readonly TextViewTableSource _model = null;
		public event Action<ListViewItem> ItemSelected = delegate{};

		public PayeesTableViewSource(List<Payee> model)
		{
			_model = ViewUtilities.ConvertPayeeListTextViewTableSource(model);		
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _model.Items.Count;
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
			var item = _model.Items[indexPath.Row];

			var cell = tableView.DequeueReusableCell("cellMain");

			var lblPayee = (UILabel)cell.ViewWithTag(100);
			var lblNickName = (UILabel)cell.ViewWithTag(200);
			var lblAccount = (UILabel)cell.ViewWithTag(300);

			lblPayee.Text = item.HeaderText;
			lblNickName.Text = item.Header2Text;
			lblAccount.Text = item.Value1Text;

			cell.Accessory = item.MoreIconVisible ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(_model.Items[indexPath.Row]);

			tableView.DeselectRow(indexPath, true);
		}
	}
	*/

	public class PayeesV2TableViewSource : UITableViewSource
	{
		private readonly TextViewTableSource _textViewTableSource = null;
		public event Action<ListViewItem> ItemSelected = delegate{};

		public PayeesV2TableViewSource(List<Payee> model, bool isActive)
		{
			_textViewTableSource = ViewUtilities.ConvertPayeeV2ListTextViewTableSource(model, isActive);		
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

			var lblPayee = (UILabel)cell.ViewWithTag(100);
			var lblNickName = (UILabel)cell.ViewWithTag(200);
			var lblAccount = (UILabel)cell.ViewWithTag(300);

			lblPayee.Text = item.HeaderText;
			lblNickName.Text = item.Header2Text;
			lblAccount.Text = item.Value1Text;

			cell.Accessory = item.MoreIconVisible ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(_textViewTableSource.Items[indexPath.Row]);

			tableView.DeselectRow(indexPath, true);
		}
	}
}