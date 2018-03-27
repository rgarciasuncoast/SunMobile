using System;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications.AlertSettings.AccountSpecificAlerts;
using SunBlock.DataTransferObjects.Notifications.AlertSettings;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public class ManageAlertsTableViewSource : UITableViewSource
	{
		private readonly List<IAccountSpecificAlertSettings> _model = null;
		private bool _globalEnabled;
		public event Action<int> ItemSelected = delegate {};
		public event Action<IAccountSpecificAlertSettings> AccountSelected = delegate{};

		public ManageAlertsTableViewSource(List<IAccountSpecificAlertSettings> model, bool globalEnabled)
		{
			_model = model;
			_globalEnabled = globalEnabled;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			nint returnValue;

			returnValue = section == 0 ? 3 : _model.Count;

			return returnValue;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 2;
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return section == 1 ? "Account Specific Alerts" : string.Empty;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = null;

			if (indexPath.Section == 0)
			{
				switch (indexPath.Row)
				{
					case 0:
						cell = tableView.DequeueReusableCell("cellSecurityAlertsEnabled");

						break;
					case 1:
						cell = tableView.DequeueReusableCell("cellEDocumentsAlertsEnabled");
						break;
					case 2:
						cell = tableView.DequeueReusableCell("cellAlertsEnabled");
						break;				
				}
			}
			else
			{
				var item = (AccountSpecificAlertModel)_model[indexPath.Row];

				var views = NSBundle.MainBundle.LoadNib("AlertsTableViewCell", tableView, null); 
				cell = Runtime.GetNSObject(views.ValueAt(0)) as AlertsTableViewCell; 

				var lblAccountName = (UILabel)cell.ViewWithTag(100);
				var lblAccountAlertsEnabled = (UILabel)cell.ViewWithTag(200);

				lblAccountName.Text = item.DisplayText;

				lblAccountAlertsEnabled.Text = ((item.AvailableBalaceThresholdAlertSettings != null && item.AvailableBalaceThresholdAlertSettings.Enabled) ||
					(item.DirectDepositAlertSettings != null && item.DirectDepositAlertSettings.Enabled) ||
					(item.NsfAlertSettings != null && item.NsfAlertSettings.Enabled) ||
					item.PaymentReminderAlertSettings != null && item.PaymentReminderAlertSettings.Enabled) && _globalEnabled ? "On" : "Off";

				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				cell.UserInteractionEnabled = _globalEnabled;
			}

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 0)
			{
				ItemSelected(indexPath.Row);
			}
			else
			{
				AccountSelected(_model[indexPath.Row]);
				tableView.DeselectRow(indexPath, true);
			}
		}
	}
}