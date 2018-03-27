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
    [Register ("MessageViewController")]
    partial class MessageViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnCompose { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnDocumentCenter { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnFixedSpace1 { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnFixedSpace2 { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnReply { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnTrash { get; set; }


        [Outlet]
        UIKit.UILabel lblDateReceived { get; set; }


        [Outlet]
        UIKit.UILabel lblFrom { get; set; }


        [Outlet]
        UIKit.UILabel lblMessage { get; set; }


        [Outlet]
        UIKit.UILabel lblSubject { get; set; }


        [Outlet]
        UIKit.UIScrollView scrollViewMain { get; set; }


        [Outlet]
        UIKit.UIToolbar toolbarMain { get; set; }


        [Outlet]
        UIKit.UITextView txtViewMessage { get; set; }


        [Outlet]
        UIKit.UIWebView webViewMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCompose != null) {
                btnCompose.Dispose ();
                btnCompose = null;
            }

            if (btnDocumentCenter != null) {
                btnDocumentCenter.Dispose ();
                btnDocumentCenter = null;
            }

            if (btnFixedSpace1 != null) {
                btnFixedSpace1.Dispose ();
                btnFixedSpace1 = null;
            }

            if (btnFixedSpace2 != null) {
                btnFixedSpace2.Dispose ();
                btnFixedSpace2 = null;
            }

            if (btnReply != null) {
                btnReply.Dispose ();
                btnReply = null;
            }

            if (btnTrash != null) {
                btnTrash.Dispose ();
                btnTrash = null;
            }

            if (lblDateReceived != null) {
                lblDateReceived.Dispose ();
                lblDateReceived = null;
            }

            if (lblSubject != null) {
                lblSubject.Dispose ();
                lblSubject = null;
            }

            if (toolbarMain != null) {
                toolbarMain.Dispose ();
                toolbarMain = null;
            }

            if (webViewMessage != null) {
                webViewMessage.Dispose ();
                webViewMessage = null;
            }
        }
    }
}