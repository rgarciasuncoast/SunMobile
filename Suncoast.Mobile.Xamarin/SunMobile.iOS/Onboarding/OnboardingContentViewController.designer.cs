// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Onboarding
{
    [Register ("OnboardingContentViewController")]
    partial class OnboardingContentViewController
    {
        [Outlet]
        UIKit.UIButton btnSkip { get; set; }


        [Outlet]
        UIKit.UIImageView imageMain { get; set; }


        [Outlet]
        UIKit.UILabel lblDescription { get; set; }


        [Outlet]
        UIKit.UILabel lblTitle { get; set; }


        [Outlet]
        UIKit.UIView viewMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnSkip != null) {
                btnSkip.Dispose ();
                btnSkip = null;
            }

            if (imageMain != null) {
                imageMain.Dispose ();
                imageMain = null;
            }

            if (lblDescription != null) {
                lblDescription.Dispose ();
                lblDescription = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (viewMain != null) {
                viewMain.Dispose ();
                viewMain = null;
            }
        }
    }
}