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
    [Register ("TransactionsViewController")]
    partial class TransactionsViewController
    {
        [Outlet]
        UIKit.UIButton btnExpand { get; set; }


        [Outlet]
        UIKit.UIButton btnRequestPayoff { get; set; }


        [Outlet]
        UIKit.UIImageView imgExpand { get; set; }


        [Outlet]
        UIKit.UILabel lblAccountNumber { get; set; }


        [Outlet]
        UIKit.UILabel lblAvailableBalance { get; set; }


        [Outlet]
        UIKit.UILabel lblBalance { get; set; }


        [Outlet]
        UIKit.UILabel lblHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblMinimumPayment { get; set; }


        [Outlet]
        UIKit.UILabel lblNextDueDate { get; set; }


        [Outlet]
        UIKit.UILabel lblRate { get; set; }


        [Outlet]
        UIKit.UITableView transactionTableView { get; set; }


        [Outlet]
        UIKit.UILabel txtAccountNumber { get; set; }


        [Outlet]
        UIKit.UILabel txtAccountNumberForACH { get; set; }


        [Outlet]
        UIKit.UILabel txtAvailableBalance { get; set; }


        [Outlet]
        UIKit.UILabel txtBalance { get; set; }


        [Outlet]
        UIKit.UILabel txtMinimumPayment { get; set; }


        [Outlet]
        UIKit.UILabel txtNextDueDate { get; set; }


        [Outlet]
        UIKit.UILabel txtRate { get; set; }


        [Outlet]
        UIKit.UIView viewPayoffInfo { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnRequestPayoff != null) {
                btnRequestPayoff.Dispose ();
                btnRequestPayoff = null;
            }

            if (imgExpand != null) {
                imgExpand.Dispose ();
                imgExpand = null;
            }

            if (lblAccountNumber != null) {
                lblAccountNumber.Dispose ();
                lblAccountNumber = null;
            }

            if (lblAvailableBalance != null) {
                lblAvailableBalance.Dispose ();
                lblAvailableBalance = null;
            }

            if (lblBalance != null) {
                lblBalance.Dispose ();
                lblBalance = null;
            }

            if (lblHeader != null) {
                lblHeader.Dispose ();
                lblHeader = null;
            }

            if (lblMinimumPayment != null) {
                lblMinimumPayment.Dispose ();
                lblMinimumPayment = null;
            }

            if (lblNextDueDate != null) {
                lblNextDueDate.Dispose ();
                lblNextDueDate = null;
            }

            if (lblRate != null) {
                lblRate.Dispose ();
                lblRate = null;
            }

            if (transactionTableView != null) {
                transactionTableView.Dispose ();
                transactionTableView = null;
            }

            if (txtAccountNumber != null) {
                txtAccountNumber.Dispose ();
                txtAccountNumber = null;
            }

            if (txtAccountNumberForACH != null) {
                txtAccountNumberForACH.Dispose ();
                txtAccountNumberForACH = null;
            }

            if (txtAvailableBalance != null) {
                txtAvailableBalance.Dispose ();
                txtAvailableBalance = null;
            }

            if (txtBalance != null) {
                txtBalance.Dispose ();
                txtBalance = null;
            }

            if (txtMinimumPayment != null) {
                txtMinimumPayment.Dispose ();
                txtMinimumPayment = null;
            }

            if (txtNextDueDate != null) {
                txtNextDueDate.Dispose ();
                txtNextDueDate = null;
            }

            if (txtRate != null) {
                txtRate.Dispose ();
                txtRate = null;
            }

            if (viewPayoffInfo != null) {
                viewPayoffInfo.Dispose ();
                viewPayoffInfo = null;
            }
        }
    }
}