// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Deposits
{
    [Register ("DepositsTableViewController")]
    partial class DepositsTableViewController
    {
        [Outlet]
        UIKit.UIView amountView { get; set; }


        [Outlet]
        UIKit.UIView backOfCheckView { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellAccount { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellAmount { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellBackOfCheck { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellFrontOfCheck { get; set; }


        [Outlet]
        UIKit.UIImageView imgCheckBack { get; set; }


        [Outlet]
        UIKit.UIImageView imgCheckFront { get; set; }


        [Outlet]
        UIKit.UILabel lblAmount { get; set; }


        [Outlet]
        UIKit.UILabel lblAvailableBalance { get; set; }


        [Outlet]
        UIKit.UILabel lblBackOfCheck { get; set; }


        [Outlet]
        UIKit.UILabel lblBalance { get; set; }


        [Outlet]
        UIKit.UILabel lblDailyLimit { get; set; }


        [Outlet]
        UIKit.UILabel lblDepositTo { get; set; }


        [Outlet]
        UIKit.UILabel lblDepositTo2 { get; set; }


        [Outlet]
        UIKit.UILabel lblDepositToHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblFrontOfCheck { get; set; }


        [Outlet]
        UIKit.UILabel lblText1 { get; set; }


        [Outlet]
        UIKit.UILabel lblText2 { get; set; }


        [Outlet]
        UIKit.UILabel lblValue1 { get; set; }


        [Outlet]
        UIKit.UILabel lblValue2 { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Outlet]
        UIKit.UITextField txtAmount { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (amountView != null) {
                amountView.Dispose ();
                amountView = null;
            }

            if (cellAccount != null) {
                cellAccount.Dispose ();
                cellAccount = null;
            }

            if (cellAmount != null) {
                cellAmount.Dispose ();
                cellAmount = null;
            }

            if (cellBackOfCheck != null) {
                cellBackOfCheck.Dispose ();
                cellBackOfCheck = null;
            }

            if (cellFrontOfCheck != null) {
                cellFrontOfCheck.Dispose ();
                cellFrontOfCheck = null;
            }

            if (imgCheckBack != null) {
                imgCheckBack.Dispose ();
                imgCheckBack = null;
            }

            if (imgCheckFront != null) {
                imgCheckFront.Dispose ();
                imgCheckFront = null;
            }

            if (lblAmount != null) {
                lblAmount.Dispose ();
                lblAmount = null;
            }

            if (lblAvailableBalance != null) {
                lblAvailableBalance.Dispose ();
                lblAvailableBalance = null;
            }

            if (lblBackOfCheck != null) {
                lblBackOfCheck.Dispose ();
                lblBackOfCheck = null;
            }

            if (lblBalance != null) {
                lblBalance.Dispose ();
                lblBalance = null;
            }

            if (lblDailyLimit != null) {
                lblDailyLimit.Dispose ();
                lblDailyLimit = null;
            }

            if (lblDepositTo != null) {
                lblDepositTo.Dispose ();
                lblDepositTo = null;
            }

            if (lblDepositTo2 != null) {
                lblDepositTo2.Dispose ();
                lblDepositTo2 = null;
            }

            if (lblDepositToHeader != null) {
                lblDepositToHeader.Dispose ();
                lblDepositToHeader = null;
            }

            if (lblFrontOfCheck != null) {
                lblFrontOfCheck.Dispose ();
                lblFrontOfCheck = null;
            }

            if (lblText1 != null) {
                lblText1.Dispose ();
                lblText1 = null;
            }

            if (lblText2 != null) {
                lblText2.Dispose ();
                lblText2 = null;
            }

            if (lblValue1 != null) {
                lblValue1.Dispose ();
                lblValue1 = null;
            }

            if (lblValue2 != null) {
                lblValue2.Dispose ();
                lblValue2 = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }

            if (txtAmount != null) {
                txtAmount.Dispose ();
                txtAmount = null;
            }
        }
    }
}