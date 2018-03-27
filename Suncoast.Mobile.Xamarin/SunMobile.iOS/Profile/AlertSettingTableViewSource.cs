using System;
using System.Collections.Generic;
using Foundation;
using SunMobile.Shared.Data;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public class AlertSettingTableViewSource : UITableViewSource
	{
		public event Action<ListViewItem> ItemSelected = delegate{};
		public event Action<List<AlertSetting>> ItemsChanged = delegate{};
		private readonly List<ListViewItem> _model;
		private List<AlertSetting> _itemsChanged;

		public AlertSettingTableViewSource(List<ListViewItem> model)
		{
			_model = model;
			_itemsChanged = new List<AlertSetting>();
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

			var cell = tableView.DequeueReusableCell("cellMain");

			var lblDescription = (UILabel)cell.ViewWithTag(100);
			var lblEnabled = (UILabel)cell.ViewWithTag(200);
			var switchEnabled = (UISwitch)cell.ViewWithTag(300);

			if (lblDescription != null)
			{
				lblDescription.Text = item.Item1Text;
			}

			if (lblEnabled != null)
			{
				lblEnabled.Text = item.Item2Text;
			}

			if (switchEnabled != null)
			{
				switchEnabled.On = item.IsChecked;
			}

			cell.Accessory = item.MoreIconVisible ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;

			if (item.MoreIconVisible)
			{
				if (switchEnabled != null)
				{
					switchEnabled.RemoveFromSuperview();
				}
			}
			else
			{
				if (lblEnabled != null)
				{
					lblEnabled.RemoveFromSuperview();
				}
			}

			if (switchEnabled != null)
			{
				switchEnabled.ValueChanged += (sender, e) =>
				{
					var alertSetting = new AlertSetting
					{
						Description = item.Item3Text,
						Value = switchEnabled.On
					};

					int itemIndex = -1;

					for (int i = 0; i < _itemsChanged.Count; i++)
					{
						if (_itemsChanged[i].Description == alertSetting.Description)
						{
							itemIndex = i;
							break;
						}
					}

					if (switchEnabled.On != item.IsChecked)
					{
						if (itemIndex < 0)
						{
							_itemsChanged.Add(alertSetting);
							ItemsChanged(_itemsChanged);
						}
					}
					else
					{
						if (itemIndex >= 0)
						{
							_itemsChanged.RemoveAt(itemIndex);
							ItemsChanged(_itemsChanged);						
						}
					}
				};
			}

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (_model[indexPath.Row].MoreIconVisible)
			{
				ItemSelected(_model[indexPath.Row]);
			}

			tableView.DeselectRow(indexPath, true);
		}
	}
}