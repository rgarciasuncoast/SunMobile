using System;
using System.Collections.Generic;
using Foundation;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Common
{
	public class StringListTableViewSource : UITableViewSource
	{
		public event Action<List<string>> ItemsSelected = delegate{};
		private List<ListViewItem> _tableViewSource;
		private List<string> _selectedItems;

		public StringListTableViewSource(List<string> model, List<string> selectedItems = null)
		{
			_tableViewSource = new List<ListViewItem>();
            _selectedItems = new List<string>();

			foreach (string s in model)
			{
				var listViewItem = new ListViewItem();
				listViewItem.HeaderText = s;

                if (selectedItems != null)
                {
                    listViewItem.IsChecked = selectedItems.Contains(s);

                    if (listViewItem.IsChecked)
                    {
                        _selectedItems.Add(listViewItem.HeaderText);
                    }
                }

				_tableViewSource.Add(listViewItem);
			}			
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _tableViewSource.Count;
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
			var item = _tableViewSource[indexPath.Row];
			var cell = tableView.DequeueReusableCell("cellMain");
			var lblHeader = (UILabel)cell.ViewWithTag(100);
			lblHeader.Text = item.HeaderText;

			cell.Accessory = item.IsChecked ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var listViewItem = _tableViewSource[indexPath.Row];

			if (!listViewItem.IsChecked)
			{
				tableView.CellAt(indexPath).Accessory = UITableViewCellAccessory.Checkmark;
				_selectedItems.Add(listViewItem.HeaderText);
			}
			else
			{
				tableView.CellAt(indexPath).Accessory = UITableViewCellAccessory.None;
				_selectedItems.Remove(listViewItem.HeaderText);
			}

			_tableViewSource[indexPath.Row].IsChecked = !_tableViewSource[indexPath.Row].IsChecked;

			ItemsSelected(_selectedItems);

			tableView.DeselectRow(indexPath, true);
		}
	}
}