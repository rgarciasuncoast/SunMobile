using System;
using Foundation;
using SunMobile.Shared.Utilities.Dates;
using UIKit;

namespace SunMobile.iOS.Accounts
{
    public class PayoffTableViewSource : UITableViewSource
    {
        public event Action ItemSelected = delegate { };
        DateTime PayoffDate;

        public PayoffTableViewSource()
        {
        }

        public void SetPayoffDate(DateTime payoffDate)
        {
            PayoffDate = payoffDate;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 1;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 44f;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("cellMain");

            var lblPayoutDate = (UILabel)cell.ViewWithTag(100);
            var txtPayoutDate = (UILabel)cell.ViewWithTag(200);

            if(PayoffDate == DateTime.MinValue)
            {
                txtPayoutDate.Text = string.Empty;
            }
            else
            {
                txtPayoutDate.Text = string.Format("{0:MM/dd/yyyy}", PayoffDate.UtcToEastern());
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ItemSelected();
            tableView.DeselectRow(indexPath, true);
        }
    }
}