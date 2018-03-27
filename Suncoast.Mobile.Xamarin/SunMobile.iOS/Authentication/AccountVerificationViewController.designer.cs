// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Authentication
{
    [Register ("AccountVerificationViewController")]
    partial class AccountVerificationViewController
    {
        [Outlet]
        UIKit.UIButton btnContinue { get; set; }


        [Outlet]
        UIKit.UIButton btnSendCode { get; set; }


        [Outlet]
        UIKit.UILabel lblHeader { get; set; }


        [Outlet]
        UIKit.UITextField txtAnswer { get; set; }


        [Outlet]
        UIKit.UITextField txtVerificationType { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnContinue != null) {
                btnContinue.Dispose ();
                btnContinue = null;
            }

            if (btnSendCode != null) {
                btnSendCode.Dispose ();
                btnSendCode = null;
            }

            if (lblHeader != null) {
                lblHeader.Dispose ();
                lblHeader = null;
            }

            if (txtAnswer != null) {
                txtAnswer.Dispose ();
                txtAnswer = null;
            }

            if (txtVerificationType != null) {
                txtVerificationType.Dispose ();
                txtVerificationType = null;
            }
        }
    }
}