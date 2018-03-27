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
    [Register ("SubAccountsCardViewController")]
    partial class SubAccountsCardViewController
    {
        [Outlet]
        UIKit.UITextView lblCardDescription { get; set; }

        [Outlet]
        UIKit.UILabel lblDebitCard { get; set; }

        [Outlet]
        UIKit.UILabel lblSelectCardImage { get; set; }

        [Outlet]
        UIKit.UISwitch switchDebitCard { get; set; }

        [Outlet]
        UIKit.UIView viewPlaceHolder { get; set; }

        [Outlet]
        UIKit.UIView viewSeparatorBottom { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblCardDescription != null) {
                lblCardDescription.Dispose ();
                lblCardDescription = null;
            }

            if (lblDebitCard != null) {
                lblDebitCard.Dispose ();
                lblDebitCard = null;
            }

            if (lblSelectCardImage != null) {
                lblSelectCardImage.Dispose ();
                lblSelectCardImage = null;
            }

            if (switchDebitCard != null) {
                switchDebitCard.Dispose ();
                switchDebitCard = null;
            }

            if (viewPlaceHolder != null) {
                viewPlaceHolder.Dispose ();
                viewPlaceHolder = null;
            }

            if (viewSeparatorBottom != null) {
                viewSeparatorBottom.Dispose ();
                viewSeparatorBottom = null;
            }
        }
    }
}