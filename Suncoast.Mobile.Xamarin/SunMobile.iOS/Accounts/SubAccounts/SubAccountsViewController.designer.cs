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
    [Register ("SubAccountsViewController")]
    partial class SubAccountsViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnNext { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnPrevious { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnShare { get; set; }


        [Outlet]
        UIKit.UIImageView imageProgress { get; set; }


        [Outlet]
        UIKit.UILabel lblHeader { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNext != null) {
                btnNext.Dispose ();
                btnNext = null;
            }

            if (btnPrevious != null) {
                btnPrevious.Dispose ();
                btnPrevious = null;
            }

            if (imageProgress != null) {
                imageProgress.Dispose ();
                imageProgress = null;
            }

            if (lblHeader != null) {
                lblHeader.Dispose ();
                lblHeader = null;
            }
        }
    }
}