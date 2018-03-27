// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Profile
{
    [Register ("AccountSpecificSettingsTableViewController")]
    partial class AccountSpecificSettingsTableViewController
    {
        [Outlet]
        UIKit.UITableView tableViewMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tableViewMain != null) {
                tableViewMain.Dispose ();
                tableViewMain = null;
            }
        }
    }
}