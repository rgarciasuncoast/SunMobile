// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Accounts
{
    [Register ("CheckImageViewController")]
    partial class CheckImageViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnPrint { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnShare { get; set; }


        [Outlet]
        UIKit.UIImageView imageView { get; set; }


        [Outlet]
        UIKit.UIScrollView scrollView { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segmentControl { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPrint != null) {
                btnPrint.Dispose ();
                btnPrint = null;
            }

            if (btnShare != null) {
                btnShare.Dispose ();
                btnShare = null;
            }

            if (imageView != null) {
                imageView.Dispose ();
                imageView = null;
            }

            if (scrollView != null) {
                scrollView.Dispose ();
                scrollView = null;
            }

            if (segmentControl != null) {
                segmentControl.Dispose ();
                segmentControl = null;
            }
        }
    }
}