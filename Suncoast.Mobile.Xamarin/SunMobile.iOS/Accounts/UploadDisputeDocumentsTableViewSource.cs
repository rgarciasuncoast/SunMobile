using System;
using System.Collections.Generic;
using Foundation;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public class UploadDisputeDocumentsTableViewSource : UITableViewSource
	{
		public event Action<int> RemoveSelected = delegate { };
		List<ListViewItem> _listViewItems = new List<ListViewItem>();

		public UploadDisputeDocumentsTableViewSource (List<ListViewItem> model)
		{
			_listViewItems = model;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _listViewItems.Count;
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
			var cell = tableView.DequeueReusableCell("cellMain");

			var item = _listViewItems[indexPath.Row];

			var lblText1 = (UILabel)cell.ViewWithTag(100);
			lblText1.Text = item.Item1Text;
			var lblText2 = (UILabel)cell.ViewWithTag(200);
			lblText2.Text = item.Item2Text;
			var btnDelete = (UIButton)cell.ViewWithTag(300);

			if (btnDelete != null)
			{
				btnDelete.Tag = indexPath.Row;

				btnDelete.TouchUpInside += (sender, e) =>
				{
					RemoveSelected((int)((UIButton)sender).Tag);
				};
			}

			return cell;
		}
	}
}