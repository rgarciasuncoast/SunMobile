// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Accounts
{
    [Register ("TransactionSelectionViewController")]
    partial class TransactionSelectionViewController
    {
        [Outlet]
        UIKit.UIButton btnNext { get; set; }


        [Outlet]
        UIKit.UIButton btnPrevious { get; set; }


        [Outlet]
        UIKit.UILabel lblDate { get; set; }


        [Outlet]
        UIKit.UILabel lblHeader { get; set; }


        [Outlet]
        UIKit.UITableView TransactionSelectionTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNext != null) {
                btnNext.Dispose ();
                btnNext = null;
            }

            if (btnPrevious != null) {
                btnPrevious.Dispose ();
                btnPrevious = null;
            }

            if (lblDate != null) {
                lblDate.Dispose ();
                lblDate = null;
            }

            if (lblHeader != null) {
                lblHeader.Dispose ();
                lblHeader = null;
            }

            if (TransactionSelectionTableView != null) {
                TransactionSelectionTableView.Dispose ();
                TransactionSelectionTableView = null;
            }
        }
    }
}