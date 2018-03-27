using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public class EStatementsTableViewSource : UITableViewSource
	{
		private List<ListViewItem> _model;
		public event Action<ListViewItem> ItemSelected = delegate { };
		private bool _enrolled;

		public EStatementsTableViewSource(List<ImageDocument> documents, bool enrolled)
		{
			_enrolled = enrolled;
			_model = ViewUtilities.ConvertListImageDocumentToListViews(documents);
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _enrolled ? _model.Count : 1;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			if (_enrolled)
			{
				var item = _model[indexPath.Row];

				var cell = tableView.DequeueReusableCell("cellMain");
				cell.SelectionStyle = UITableViewCellSelectionStyle.Default;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

				var lblDate = (UILabel)cell.ViewWithTag(100);
				var lblDescription = (UILabel)cell.ViewWithTag(200);

				lblDate.Text = item.Item1Text;
				lblDescription.Text = item.Item2Text;
				lblDescription.Font = lblDescription.Font.WithSize(13f);

				return cell;
			}
			else
			{
				var cell = tableView.DequeueReusableCell("cellMain");
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.Accessory = UITableViewCellAccessory.None;

				var lblDate = (UILabel)cell.ViewWithTag(100);
				var lblDescription = (UILabel)cell.ViewWithTag(200);

				lblDate.Text = string.Empty;
				lblDescription.Text = "You are not currently enrolled.";
				lblDescription.Font = lblDescription.Font.WithSize(10f);

				return cell;
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (_enrolled)
			{
				ItemSelected(_model[indexPath.Row]);

				tableView.DeselectRow(indexPath, true);
			}
		}
	}
}