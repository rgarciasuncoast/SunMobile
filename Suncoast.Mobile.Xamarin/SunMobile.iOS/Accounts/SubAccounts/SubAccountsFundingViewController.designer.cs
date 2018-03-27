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
    [Register ("SubAccountsFundingViewController")]
    partial class SubAccountsFundingViewController
    {
        [Outlet]
        UIKit.UILabel labelFunding { get; set; }


        [Outlet]
        UIKit.UILabel lblSource { get; set; }


        [Outlet]
        UIKit.UILabel lblSource2 { get; set; }


        [Outlet]
        UIKit.UILabel lblSourceText1 { get; set; }


        [Outlet]
        UIKit.UILabel lblSourceText2 { get; set; }


        [Outlet]
        UIKit.UILabel lblSourceValue1 { get; set; }


        [Outlet]
        UIKit.UILabel lblSourceValue2 { get; set; }


        [Outlet]
        UIKit.UITextField txtAmount { get; set; }


        [Outlet]
        UIKit.UIView viewAccount { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelFunding != null) {
                labelFunding.Dispose ();
                labelFunding = null;
            }

            if (lblSource != null) {
                lblSource.Dispose ();
                lblSource = null;
            }

            if (lblSource2 != null) {
                lblSource2.Dispose ();
                lblSource2 = null;
            }

            if (lblSourceText1 != null) {
                lblSourceText1.Dispose ();
                lblSourceText1 = null;
            }

            if (lblSourceText2 != null) {
                lblSourceText2.Dispose ();
                lblSourceText2 = null;
            }

            if (lblSourceValue1 != null) {
                lblSourceValue1.Dispose ();
                lblSourceValue1 = null;
            }

            if (lblSourceValue2 != null) {
                lblSourceValue2.Dispose ();
                lblSourceValue2 = null;
            }

            if (txtAmount != null) {
                txtAmount.Dispose ();
                txtAmount = null;
            }

            if (viewAccount != null) {
                viewAccount.Dispose ();
                viewAccount = null;
            }
        }
    }
}