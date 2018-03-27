// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Messaging
{
    [Register ("MessageComposeViewController")]
    partial class MessageComposeViewController
    {
        [Outlet]
        UIKit.UILabel lblComposeMessageSubject { get; set; }


        [Outlet]
        UIKit.UITextView txtBody { get; set; }


        [Outlet]
        UIKit.UITextField txtSubject { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblComposeMessageSubject != null) {
                lblComposeMessageSubject.Dispose ();
                lblComposeMessageSubject = null;
            }

            if (txtBody != null) {
                txtBody.Dispose ();
                txtBody = null;
            }

            if (txtSubject != null) {
                txtSubject.Dispose ();
                txtSubject = null;
            }
        }
    }
}