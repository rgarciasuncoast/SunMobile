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
    [Register ("MessagesViewController")]
    partial class MessagesViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnCompose { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMain { get; set; }


        [Outlet]
        UIKit.UIToolbar toolbarMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCompose != null) {
                btnCompose.Dispose ();
                btnCompose = null;
            }

            if (tableViewMain != null) {
                tableViewMain.Dispose ();
                tableViewMain = null;
            }

            if (toolbarMain != null) {
                toolbarMain.Dispose ();
                toolbarMain = null;
            }
        }
    }
}