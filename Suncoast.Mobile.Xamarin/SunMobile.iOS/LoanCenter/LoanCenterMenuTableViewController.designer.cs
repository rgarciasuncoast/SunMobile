// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.LoanCenter
{
    [Register ("LoanCenterMenuTableViewController")]
    partial class LoanCenterMenuTableViewController
    {
        [Outlet]
        UIKit.UITableView tableViewMenu { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tableViewMenu != null) {
                tableViewMenu.Dispose ();
                tableViewMenu = null;
            }
        }
    }
}