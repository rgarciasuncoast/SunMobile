using System;
using Foundation;
using UIKit;
using SunMobile.Shared.Logging;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Views;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Culture;

namespace SunMobile.iOS.Accounts
{
	public class TransactionsTableViewSource : UITableViewSource
	{
		private readonly TextViewTableSource Model = null;
		private string _memberId;
		public event Action<ListViewItem> ItemSelected = delegate { };
		public event Action<ListViewItem> ViewCheckSelected = delegate { };
		public event Action<ListViewItem> DisputeSelected = delegate { };
		public event Action<int> ScrolledToBottom = delegate { };

		public TransactionsTableViewSource(TextViewTableSource model, string memberId)
		{
			Model = model;
			_memberId = memberId;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Model.Items.Count;
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

			var item = Model.Items[indexPath.Row];

			var cell = tableView.DequeueReusableCell("cellMain");

			var imageCheck = (UIImageView)cell.ViewWithTag(300);
			var lblHeader = (UILabel)cell.ViewWithTag(100);
			var lblText1 = (UILabel)cell.ViewWithTag(200);
			var lblValue1 = (UILabel)cell.ViewWithTag(400);
			var lblValue2 = (UILabel)cell.ViewWithTag(500);

			imageCheck.Image = UIImage.FromBundle(item.ImageName);

			// I am manually laying these controls out because if I right anchor them, they will be uneven because some have disclosures and some do not.
			lblValue1.Frame = new CoreGraphics.CGRect(tableView.Bounds.Width - lblValue1.Frame.Width - disclosureWidth, lblValue1.Frame.Y, lblValue1.Frame.Width, lblValue1.Frame.Height);
			lblValue2.Frame = new CoreGraphics.CGRect(tableView.Bounds.Width - lblValue2.Frame.Width - disclosureWidth, lblValue2.Frame.Y, lblValue2.Frame.Width, lblValue2.Frame.Height);
			imageCheck.Frame = new CoreGraphics.CGRect(tableView.Bounds.Width - imageCheck.Frame.Width - lblValue1.Frame.Width - disclosureWidth, imageCheck.Frame.Y, imageCheck.Frame.Width, imageCheck.Frame.Height);
			lblText1.Frame = new CoreGraphics.CGRect(lblText1.Frame.X, lblText1.Frame.Y, imageCheck.Frame.X - lblText1.Frame.X - space, lblText1.Frame.Height);

			lblHeader.Text = item.HeaderText;
			lblText1.Text = item.Item1Text;
			lblText1.SizeToFit();
			lblValue1.Text = item.Value1Text;
			lblValue2.Text = item.Value2Text;

			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var amount = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(item.Value1Text));

			decimal result;
			decimal.TryParse(amount, out result);
			lblValue1.TextColor = result < 0 ? UIColor.Red : UIColor.FromRGB(0x1b, 0x50, 0x6e);

            // Pending items
            cell.BackgroundColor = item.Bool1Value ? UIColor.FromRGB(0xf1, 0xfa, 0xff) : UIColor.White;
            lblHeader.TextColor = item.Bool1Value ? UIColor.FromRGB(0x1b, 0x50, 0x6e) : UIColor.Black;

            if (item.Bool1Value)
            {
                lblHeader.Text = "Pending";
                CultureTextProvider.SetMobileResourceText(lblHeader, "D335A6E9-8ADB-4C29-912D-625090EAD031", "4FCFEB18-9278-4B44-A75E-651F1D68258E", "Pending");
            }

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			try
			{
				if (indexPath.Row >= 0 && indexPath.Row <= Model.Items.Count)
				{
					ItemSelected(Model.Items[indexPath.Row]);
					tableView.DeselectRow(indexPath, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsTableViewSource:RowSelected");
			}
		}

		public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			if (indexPath.Row >= Model.Items.Count - 5)  // Load new items when we are 5 from the end of the list.
			{
				ScrolledToBottom(Model.Items.Count);
			}
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			var returnValue = false;

			try
			{
				if (indexPath.Row >= 0 && indexPath.Row <= Model.Items.Count)
				{
					var item = Model.Items[indexPath.Row];
					var transaction = (Transaction)item.Data;
					var accountMethods = new AccountMethods();
					var disputeInfoResponse = accountMethods.GetDisputeInfo(transaction, _memberId, item.Item5Text == "CreditCard");

					if (item.MoreIconVisible || disputeInfoResponse.AllowDispute)
					{
						returnValue = true;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsTableViewSource:CanEditRow");
			}

			return returnValue;
		}

		public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewRowAction[] rowActions = null;

			try
			{
				if (indexPath.Row >= 0 && indexPath.Row <= Model.Items.Count)
				{
					var item = Model.Items[indexPath.Row];
					var transaction = (Transaction)item.Data;
					var accountMethods = new AccountMethods();
					var disputeInfoResponse = accountMethods.GetDisputeInfo(transaction, _memberId, item.Item5Text == "CreditCard");

					var viewCheckText = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "CB26E0E2-30CC-4225-9936-32ECCA4D67A0", "View Check");
					var disputeText = CultureTextProvider.GetMobileResourceText("AB401808-AC62-4A09-B4E9-C8FE740CD699", "6E08242D-1C53-426F-98F3-957E59BF4F54", "Dispute");

					if (item.MoreIconVisible && disputeInfoResponse.AllowDispute)
					{
						var viewCheckButton = UITableViewRowAction.Create(UITableViewRowActionStyle.Normal, viewCheckText, delegate { ViewCheck(item); });
						var disputeButton = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, disputeText, delegate { Dispute(item); });
						rowActions = new UITableViewRowAction[] { disputeButton, viewCheckButton };
					}

					if (item.MoreIconVisible && !disputeInfoResponse.AllowDispute)
					{
						var viewCheckButton = UITableViewRowAction.Create(UITableViewRowActionStyle.Normal, viewCheckText, delegate { ViewCheck(item); });
						rowActions = new UITableViewRowAction[] { viewCheckButton };
					}

					if (!item.MoreIconVisible && disputeInfoResponse.AllowDispute)
					{
						var disputeButton = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, disputeText, delegate { Dispute(item); });
						rowActions = new UITableViewRowAction[] { disputeButton };
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsTableViewSource:EditActionsForRow");
			}

			return rowActions;
		}

		private void Dispute(ListViewItem item)
		{
			if (item != null)
			{
				DisputeSelected(item);
			}
		}

		private void ViewCheck(ListViewItem item)
		{
			if (item != null && item.MoreIconVisible)
			{
				ViewCheckSelected(item);
			}
		}
	}
}