using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS
{
	public class TaxDocumentsTableViewSource: UITableViewSource
	{
		private List<ListViewItem> _model;
		public event Action<ListViewItem> ItemSelected = delegate { };
		public event Action<string> InstructionItemSelected = delegate { };

		public TaxDocumentsTableViewSource(List<ImageDocument> documents)
		{
			_model = ViewUtilities.ConvertListImageDocumentToListViews(documents);
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

            var lblDate = (UILabel)cell.ViewWithTag(100);
            var lblDescription = (UILabel)cell.ViewWithTag(200);

            lblDate.Text = item.Item1Text;
            lblDescription.Text = item.Item2Text;

            return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(_model[indexPath.Row]);			
			tableView.DeselectRow(indexPath, true);
		}
	}
}