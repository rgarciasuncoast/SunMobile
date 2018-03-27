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
    [Register ("SelectAccountViewController")]
    partial class SelectAccountViewController
    {
        [Outlet]
        UIKit.UISegmentedControl segmentAccountTypes { get; set; }


        [Outlet]
        UIKit.UITableView tableViewAccounts { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (segmentAccountTypes != null) {
                segmentAccountTypes.Dispose ();
                segmentAccountTypes = null;
            }

            if (tableViewAccounts != null) {
                tableViewAccounts.Dispose ();
                tableViewAccounts = null;
            }
        }
    }
}