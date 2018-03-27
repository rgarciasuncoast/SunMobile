// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Common
{
    [Register ("CustomAlertViewController")]
    partial class CustomAlertViewController
    {
        [Outlet]
        UIKit.UIButton btnNegative { get; set; }


        [Outlet]
        UIKit.UIButton btnPositive { get; set; }


        [Outlet]
        UIKit.UISwitch switchConfirm { get; set; }


        [Outlet]
        UIKit.UILabel txtConfirm { get; set; }


        [Outlet]
        UIKit.UITextView txtMessage { get; set; }


        [Outlet]
        UIKit.UILabel txtTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNegative != null) {
                btnNegative.Dispose ();
                btnNegative = null;
            }

            if (btnPositive != null) {
                btnPositive.Dispose ();
                btnPositive = null;
            }

            if (switchConfirm != null) {
                switchConfirm.Dispose ();
                switchConfirm = null;
            }

            if (txtConfirm != null) {
                txtConfirm.Dispose ();
                txtConfirm = null;
            }

            if (txtMessage != null) {
                txtMessage.Dispose ();
                txtMessage = null;
            }

            if (txtTitle != null) {
                txtTitle.Dispose ();
                txtTitle = null;
            }
        }
    }
}