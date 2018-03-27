// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SunMobile.iOS.LoanCenter
{
    [Register ("LoanCenterViewController")]
    partial class LoanCenterViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView loanCenterWebView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (loanCenterWebView != null) {
                loanCenterWebView.Dispose ();
                loanCenterWebView = null;
            }
        }
    }
}