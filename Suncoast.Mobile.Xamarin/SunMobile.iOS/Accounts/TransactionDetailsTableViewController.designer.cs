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
    [Register ("TransactionDetailsTableViewController")]
    partial class TransactionDetailsTableViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnDispute { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnViewCheck { get; set; }


        [Outlet]
        UIKit.UILabel lblAmountLabel { get; set; }


        [Outlet]
        UIKit.UILabel lblDateLabel { get; set; }


        [Outlet]
        UIKit.UILabel lblDescriptionLabel { get; set; }


        [Outlet]
        UIKit.UILabel lblTransactionAmount { get; set; }


        [Outlet]
        UIKit.UILabel lblTransactionDate { get; set; }


        [Outlet]
        UIKit.UILabel lblTransactionDescription { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnDispute != null) {
                btnDispute.Dispose ();
                btnDispute = null;
            }

            if (btnViewCheck != null) {
                btnViewCheck.Dispose ();
                btnViewCheck = null;
            }

            if (lblAmountLabel != null) {
                lblAmountLabel.Dispose ();
                lblAmountLabel = null;
            }

            if (lblDateLabel != null) {
                lblDateLabel.Dispose ();
                lblDateLabel = null;
            }

            if (lblDescriptionLabel != null) {
                lblDescriptionLabel.Dispose ();
                lblDescriptionLabel = null;
            }

            if (lblTransactionAmount != null) {
                lblTransactionAmount.Dispose ();
                lblTransactionAmount = null;
            }

            if (lblTransactionDate != null) {
                lblTransactionDate.Dispose ();
                lblTransactionDate = null;
            }

            if (lblTransactionDescription != null) {
                lblTransactionDescription.Dispose ();
                lblTransactionDescription = null;
            }
        }
    }
}