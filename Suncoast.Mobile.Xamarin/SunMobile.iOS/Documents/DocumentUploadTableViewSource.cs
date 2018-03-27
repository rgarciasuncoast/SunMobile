using System;
using System.Collections.Generic;
using Foundation;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public class DocumentUploadTableViewSource : UITableViewSource
	{
		public event Action<int> RemoveSelected = delegate { };
		List<ListViewItem> _listViewItems = new List<ListViewItem>();

		public DocumentUploadTableViewSource(List<ListViewItem> model)
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
			var item = _listViewItems[indexPath.Row];

			if (item.Item2Text.Contains("Error"))
			{
				return 72f;
			}
			else
			{
				return 44f;
			}
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("cellMain");

			var item = _listViewItems[indexPath.Row];

			var lblText1 = (UILabel)cell.ViewWithTag(100);
			lblText1.Text = item.Item1Text;
			var lblText2 = (UILabel)cell.ViewWithTag(200);
			lblText2.Text = item.Item2Text;
			var lblText3 = (UILabel)cell.ViewWithTag(250);
			lblText3.Text = string.Empty;


			if (lblText2.Text.Contains("Error"))
			{
				lblText2.TextColor = UIColor.Red;
				lblText3.Text = "File failed security scan, please upload new file.";
				lblText3.TextColor = UIColor.Red;
			}

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