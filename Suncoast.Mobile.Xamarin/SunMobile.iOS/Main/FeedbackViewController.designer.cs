// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Main
{
    [Register ("FeedbackViewController")]
    partial class FeedbackViewController
    {
        [Outlet]
        UIKit.UIButton btnNoThanks { get; set; }


        [Outlet]
        UIKit.UIButton btnRemindMeLater { get; set; }


        [Outlet]
        UIKit.UIButton btnSignMeUp { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNoThanks != null) {
                btnNoThanks.Dispose ();
                btnNoThanks = null;
            }

            if (btnRemindMeLater != null) {
                btnRemindMeLater.Dispose ();
                btnRemindMeLater = null;
            }

            if (btnSignMeUp != null) {
                btnSignMeUp.Dispose ();
                btnSignMeUp = null;
            }
        }
    }
}