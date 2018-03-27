using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using UIKit;

namespace SunMobile.iOS.Transfers
{
    public class TransferFavoritesTableViewSource : UITableViewSource
    {
        public event Action<TransferFavorite> ItemSelected = delegate {};
        public event Action ItemsChanged = delegate {};
        private readonly List<TransferFavorite> _model;

        public TransferFavoritesTableViewSource(List<TransferFavorite> model)
        {
            _model = model;
        }

        public List<TransferFavorite> GetModel()
        {
            return _model;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _model.Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _model[indexPath.Row];

            var cell = tableView.DequeueReusableCell("cellMain", indexPath);

            var lblDescription = (UILabel)cell.ViewWithTag(100);

            if (lblDescription != null)
            {
                lblDescription.Text = item.FriendlyFavoriteName;
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _model[indexPath.Row];
            ItemSelected(item);
            tableView.DeselectRow(indexPath, true);
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var item = _model[sourceIndexPath.Row];
            var deleteAt = sourceIndexPath.Row;
            var insertAt = destinationIndexPath.Row;

            // are we inserting 
            if (destinationIndexPath.Row < sourceIndexPath.Row)
            {
                // add one to where we delete, because we're increasing the index by inserting
                deleteAt += 1;
            }
            else
            {
                // add one to where we insert, because we haven't deleted the original yet
                insertAt += 1;
            }

            _model.Insert(insertAt, item);
            _model.RemoveAt(deleteAt);
            ItemsChanged();
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    // remove the item from the underlying data source
                    _model.RemoveAt(indexPath.Row);
                    // delete the row from the table
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    ItemsChanged();
                    break;
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }
    }
}