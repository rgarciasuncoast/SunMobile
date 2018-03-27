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
    [Register ("PromotionViewController")]
    partial class PromotionViewController
    {
        [Outlet]
        UIKit.UIButton btnNo { get; set; }


        [Outlet]
        UIKit.UIButton btnYes { get; set; }


        [Outlet]
        UIKit.UIWebView webViewMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNo != null) {
                btnNo.Dispose ();
                btnNo = null;
            }

            if (btnYes != null) {
                btnYes.Dispose ();
                btnYes = null;
            }

            if (webViewMain != null) {
                webViewMain.Dispose ();
                webViewMain = null;
            }
        }
    }
}