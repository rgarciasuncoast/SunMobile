// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    [Register ("SubAccountsAgreementViewController")]
    partial class SubAccountsAgreementViewController
    {
        [Outlet]
        UIKit.UILabel lblAgree { get; set; }


        [Outlet]
        UIKit.UILabel lblEnrollInEstatements { get; set; }


        [Outlet]
        UIKit.UITableView mainTableView { get; set; }


        [Outlet]
        UIKit.UISwitch switchAgree { get; set; }


        [Outlet]
        UIKit.UISwitch switchEnrollInEStatements { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblAgree != null) {
                lblAgree.Dispose ();
                lblAgree = null;
            }

            if (lblEnrollInEstatements != null) {
                lblEnrollInEstatements.Dispose ();
                lblEnrollInEstatements = null;
            }

            if (mainTableView != null) {
                mainTableView.Dispose ();
                mainTableView = null;
            }

            if (switchAgree != null) {
                switchAgree.Dispose ();
                switchAgree = null;
            }

            if (switchEnrollInEStatements != null) {
                switchEnrollInEStatements.Dispose ();
                switchEnrollInEStatements = null;
            }
        }
    }
}