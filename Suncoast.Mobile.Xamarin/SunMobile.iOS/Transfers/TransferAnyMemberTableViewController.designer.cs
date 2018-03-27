// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Transfers
{
    [Register ("TransferAnyMemberTableViewController")]
    partial class TransferAnyMemberTableViewController
    {
        [Outlet]
        UIKit.UISegmentedControl accountTypeSegmentControl { get; set; }



        [Outlet]
        UIKit.UITableViewCell lastNameCell { get; set; }



        [Outlet]
        UIKit.UILabel lblSuffix { get; set; }



        [Outlet]
        UIKit.UITableView tableView { get; set; }



        [Outlet]
        UIKit.UITextField txtAccount { get; set; }



        [Outlet]
        UIKit.UITextField txtLastName { get; set; }



        [Outlet]
        UIKit.UITextField txtSuffix { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAccount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLastName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (accountTypeSegmentControl != null) {
                accountTypeSegmentControl.Dispose ();
                accountTypeSegmentControl = null;
            }

            if (lastNameCell != null) {
                lastNameCell.Dispose ();
                lastNameCell = null;
            }

            if (lblAccount != null) {
                lblAccount.Dispose ();
                lblAccount = null;
            }

            if (lblLastName != null) {
                lblLastName.Dispose ();
                lblLastName = null;
            }

            if (lblSuffix != null) {
                lblSuffix.Dispose ();
                lblSuffix = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }

            if (txtAccount != null) {
                txtAccount.Dispose ();
                txtAccount = null;
            }

            if (txtLastName != null) {
                txtLastName.Dispose ();
                txtLastName = null;
            }

            if (txtSuffix != null) {
                txtSuffix.Dispose ();
                txtSuffix = null;
            }
        }
    }
}