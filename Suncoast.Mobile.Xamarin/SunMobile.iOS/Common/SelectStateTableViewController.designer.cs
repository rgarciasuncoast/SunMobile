// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Common
{
    [Register ("SelectStateTableViewController")]
    partial class SelectStateTableViewController
    {
        [Outlet]
        UIKit.UITableView mainTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (mainTableView != null) {
                mainTableView.Dispose ();
                mainTableView = null;
            }
        }
    }
}