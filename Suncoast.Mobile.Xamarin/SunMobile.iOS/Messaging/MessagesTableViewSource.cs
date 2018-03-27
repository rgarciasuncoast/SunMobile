using System;
using System.Collections.Generic;
using Foundation;
using SunMobile.Shared.Data;
using UIKit;

namespace SunMobile.iOS.Messaging
{
	public class MessagesTableViewSource : UITableViewSource
	{
		private readonly List<MessageViewModel> _model = null;
		public event Action<MessageViewModel> ItemSelected = delegate{};
		public event Action<MessageViewModel> ItemDeleted = delegate{};

		public MessagesTableViewSource(List<MessageViewModel> model)
		{
			_model = model;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _model.Count;
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
			var message = _model[indexPath.Row];

			var cell = tableView.DequeueReusableCell("cellMain");

			var lblSubject = (UILabel)cell.ViewWithTag(100);
			var lblBody = (UILabel)cell.ViewWithTag(300);
			var lblFriendlyDate = (UILabel)cell.ViewWithTag(400);
			var unreadBlueDot = cell.ViewWithTag(500);

			lblSubject.Text = message.Subject;
			lblBody.Text = message.BodyStrippedOfHtml;
			lblFriendlyDate.Text = message.FriendlyDate;
			unreadBlueDot.Hidden = message.IsRead;

			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(_model[indexPath.Row]);

			tableView.DeselectRow(indexPath, true);
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			switch (editingStyle) 
			{
				case UITableViewCellEditingStyle.Delete:
					// Remove from the database.
					ItemDeleted(_model[indexPath.Row]);
					// Remove the item from the underlying data source
					_model.RemoveAt(indexPath.Row);
					// Delete the row from the table
					tableView.DeleteRows(new [] { indexPath }, UITableViewRowAnimation.Fade);
					break;
				case UITableViewCellEditingStyle.None:
					Console.WriteLine("CommitEditingStyle:None called");
					break;
			}
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
		{
			return "Trash";
		}
	}
}