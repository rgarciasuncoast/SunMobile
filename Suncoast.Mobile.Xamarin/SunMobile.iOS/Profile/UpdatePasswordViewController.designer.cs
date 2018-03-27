// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Profile
{
    [Register ("UpdatePasswordViewController")]
    partial class UpdatePasswordViewController
    {
        [Outlet]
        UIKit.UILabel labelInvalidConfirmPassword { get; set; }


        [Outlet]
        UIKit.UILabel labelInvalidPassword { get; set; }


        [Outlet]
        UIKit.UITextView textViewPasswordInstructions { get; set; }


        [Outlet]
        UIKit.UITextField txtConfirmPassword { get; set; }


        [Outlet]
        UIKit.UITextField txtPassword { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelInvalidConfirmPassword != null) {
                labelInvalidConfirmPassword.Dispose ();
                labelInvalidConfirmPassword = null;
            }

            if (labelInvalidPassword != null) {
                labelInvalidPassword.Dispose ();
                labelInvalidPassword = null;
            }

            if (textViewPasswordInstructions != null) {
                textViewPasswordInstructions.Dispose ();
                textViewPasswordInstructions = null;
            }

            if (txtConfirmPassword != null) {
                txtConfirmPassword.Dispose ();
                txtConfirmPassword = null;
            }

            if (txtPassword != null) {
                txtPassword.Dispose ();
                txtPassword = null;
            }
        }
    }
}