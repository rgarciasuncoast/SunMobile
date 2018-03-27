using System;
using Foundation;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using UIKit;

namespace SunMobile.iOS.LoanCenter
{
    public partial class LoanCenterMenuTableViewController : BaseTableViewController
    {
        public LoanCenterMenuTableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hides the remaining rows.
            tableViewMenu.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            CommonMethods.AddBottomToolbar(this);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            try
            {
                var loanCenterViewController = AppDelegate.StoryBoard.InstantiateViewController("LoanCenterViewController") as LoanCenterViewController;

                switch (indexPath.Row)
                {
                    case 0:
                        loanCenterViewController.LoanType = Shared.Data.LoanCenterTypes.ApplyForLoan;     
                        break;
                    case 1:
                        loanCenterViewController.LoanType = Shared.Data.LoanCenterTypes.CarLoan;
                        break;
                    case 2:
                        loanCenterViewController.LoanType = Shared.Data.LoanCenterTypes.HomeLoan;
                        break;                    
                }

                NavigationController.PushViewController(loanCenterViewController, true);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "LoanCenterMenuTableViewController:RowSelected");
            }
        }
    }
}