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
    [Register ("PayoffViewController")]
    partial class PayoffViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnPrint { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnShare { get; set; }


        [Outlet]
        UIKit.UILabel lblDate { get; set; }


        [Outlet]
        UIKit.UITableView tablePayoffDate { get; set; }


        [Outlet]
        UIKit.UIWebView webView { get; set; }

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

            if (tablePayoffDate != null) {
                tablePayoffDate.Dispose ();
                tablePayoffDate = null;
            }

            if (webView != null) {
                webView.Dispose ();
                webView = null;
            }
        }
    }
}