// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.BillPay
{
    [Register ("BillPayMenuTableViewController")]
    partial class BillPayMenuTableViewController
    {
        [Outlet]
        UIKit.UITableView tableViewMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblManagePayees { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblPendingPayments { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSchedulePayment { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblViewHistory { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblManagePayees != null) {
                lblManagePayees.Dispose ();
                lblManagePayees = null;
            }

            if (lblPendingPayments != null) {
                lblPendingPayments.Dispose ();
                lblPendingPayments = null;
            }

            if (lblSchedulePayment != null) {
                lblSchedulePayment.Dispose ();
                lblSchedulePayment = null;
            }

            if (lblViewHistory != null) {
                lblViewHistory.Dispose ();
                lblViewHistory = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }

            if (tableViewMenu != null) {
                tableViewMenu.Dispose ();
                tableViewMenu = null;
            }
        }
    }
}