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
    [Register ("AdaptiveAuthenticationViewController")]
    partial class AdaptiveAuthenticationViewController
    {
        [Outlet]
        UIKit.UIButton btnFeedbackNoThanks { get; set; }


        [Outlet]
        UIKit.UIButton btnFeedbackRemindMeLater { get; set; }


        [Outlet]
        UIKit.UIButton btnFeedbackSignup { get; set; }


        [Outlet]
        UIKit.UIButton btnLogin { get; set; }


        [Outlet]
        UIKit.UIImageView imageUseBiometrics { get; set; }


        [Outlet]
        UIKit.UILabel labelRememberMember { get; set; }


        [Outlet]
        UIKit.UILabel labelTouchId { get; set; }


        [Outlet]
        UIKit.UILabel lblEnableTouchId { get; set; }


        [Outlet]
        UIKit.UILabel lblRememberMemberId { get; set; }


        [Outlet]
        UIKit.UISwitch switchRememberMemberId { get; set; }


        [Outlet]
        UIKit.UISwitch switchTouchId { get; set; }


        [Outlet]
        UIKit.UITextField txtMemberId { get; set; }


        [Outlet]
        UIKit.UITextField txtPin { get; set; }


        [Outlet]
        UIKit.UIView viewFeedback { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnLogin != null) {
                btnLogin.Dispose ();
                btnLogin = null;
            }

            if (imageUseBiometrics != null) {
                imageUseBiometrics.Dispose ();
                imageUseBiometrics = null;
            }

            if (labelRememberMember != null) {
                labelRememberMember.Dispose ();
                labelRememberMember = null;
            }

            if (labelTouchId != null) {
                labelTouchId.Dispose ();
                labelTouchId = null;
            }

            if (lblRememberMemberId != null) {
                lblRememberMemberId.Dispose ();
                lblRememberMemberId = null;
            }

            if (switchRememberMemberId != null) {
                switchRememberMemberId.Dispose ();
                switchRememberMemberId = null;
            }

            if (switchTouchId != null) {
                switchTouchId.Dispose ();
                switchTouchId = null;
            }

            if (txtMemberId != null) {
                txtMemberId.Dispose ();
                txtMemberId = null;
            }

            if (txtPin != null) {
                txtPin.Dispose ();
                txtPin = null;
            }
        }
    }
}