// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Deposits
{
    [Register ("CameraViewController")]
    partial class CameraViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnCancel { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnRetake { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnShutter { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnUse { get; set; }


        [Outlet]
        UIKit.UIImageView imageView { get; set; }


        [Outlet]
        UIKit.UILabel lblHelpText { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem spacerLeft { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem spacerRight { get; set; }


        [Outlet]
        UIKit.UIToolbar toolbar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCancel != null) {
                btnCancel.Dispose ();
                btnCancel = null;
            }

            if (btnRetake != null) {
                btnRetake.Dispose ();
                btnRetake = null;
            }

            if (btnShutter != null) {
                btnShutter.Dispose ();
                btnShutter = null;
            }

            if (btnUse != null) {
                btnUse.Dispose ();
                btnUse = null;
            }

            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }

            if (lblHelpText != null) {
                lblHelpText.Dispose ();
                lblHelpText = null;
            }

            if (spacerLeft != null) {
                spacerLeft.Dispose ();
                spacerLeft = null;
            }

            if (spacerRight != null) {
                spacerRight.Dispose ();
                spacerRight = null;
            }

            if (toolbar != null) {
                toolbar.Dispose ();
                toolbar = null;
            }
        }
    }
}