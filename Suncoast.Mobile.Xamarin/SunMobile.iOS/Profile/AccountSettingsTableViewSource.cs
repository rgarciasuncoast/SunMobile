using System;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Profile
{
    public class AccountSettingsTableViewSource : UITableViewSource
    {
        private readonly List<Account> _model = null;
        public event Action<nint> ShowDetails = delegate{};        
        public event Action<int> ItemSelected = delegate {};
        public event Action<Account> AccountSelected = delegate {};

        public AccountSettingsTableViewSource(AccountListTextViewModel accountList)
        {
            _model = ViewUtilities.ConvertTextViewModelToAccountList(accountList, false);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            nint returnValue;

            returnValue = section == 0 ? 1 : _model.Count;

            return returnValue;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return section == 0 ? "General Settings" : "Account Specific Settings";
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = null;

            if (indexPath.Section == 0)
            {
                switch (indexPath.Row)
                {
                    case 0:
                        cell = tableView.DequeueReusableCell("cellEStatementEnrollment");
                        break;                    
                }
            }
            else
            {
                var item = _model[indexPath.Row];

                var views = NSBundle.MainBundle.LoadNib("AccountSettingsTableViewCell", tableView, null);
                cell = Runtime.GetNSObject(views.ValueAt(0)) as AccountSettingsTableViewCell;

                var lblAccountName = (UILabel)cell.ViewWithTag(100);
                var lblEnabled = (UILabel)cell.ViewWithTag(200);

                lblAccountName.Text = item.Description;
                lblEnabled.Text = string.Empty;
            }

            return cell;
        }

        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            ShowDetails(indexPath.Row);
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