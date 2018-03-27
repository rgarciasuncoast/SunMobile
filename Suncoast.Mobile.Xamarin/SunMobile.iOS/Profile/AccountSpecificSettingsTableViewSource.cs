using System;
using System.Collections.Generic;
using Foundation;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public class AccountSpecificSettingsTableViewSource : UITableViewSource
	{
		public event Action<ListViewItem> ItemSelected = delegate{};		
        public event Action<nint> ShowDetails = delegate{};        
        private readonly List<ListViewItem> _model;		

        public AccountSpecificSettingsTableViewSource(List<ListViewItem> model)
		{
			_model = model;

            // TODO: Replace this with real logic
            _model = new List<ListViewItem>();
            var item1 = new ListViewItem { Item1Text = "Overdraft Protection " };
            _model.Add(item1);
            var item2 = new ListViewItem { Item1Text = "Courtesy Pay" };
            _model.Add(item2);
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
			var switchEnabled = (UISwitch)cell.ViewWithTag(200);

			if (lblDescription != null)
			{
				lblDescription.Text = item.Item1Text;
			}			

			return cell;
		}

        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            ShowDetails(indexPath.Row);
        }

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(_model[indexPath.Row]);
			tableView.DeselectRow(indexPath, true);
		}
	}
}