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
    [Register ("SubAccountsConfirmationViewController")]
    partial class SubAccountsConfirmationViewController
    {
        [Outlet]
        UIKit.UITextView labelConfirmationDescription { get; set; }


        [Outlet]
        UIKit.UILabel lblMemberFullName { get; set; }


        [Outlet]
        UIKit.UITextView lblSignatureConfirmationText { get; set; }


        [Outlet]
        UIKit.UILabel lblSignatureHeading { get; set; }


        [Outlet]
        UIKit.UITextField txtSignature { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelConfirmationDescription != null) {
                labelConfirmationDescription.Dispose ();
                labelConfirmationDescription = null;
            }

            if (lblMemberFullName != null) {
                lblMemberFullName.Dispose ();
                lblMemberFullName = null;
            }

            if (lblSignatureConfirmationText != null) {
                lblSignatureConfirmationText.Dispose ();
                lblSignatureConfirmationText = null;
            }

            if (lblSignatureHeading != null) {
                lblSignatureHeading.Dispose ();
                lblSignatureHeading = null;
            }

            if (txtSignature != null) {
                txtSignature.Dispose ();
                txtSignature = null;
            }
        }
    }
}