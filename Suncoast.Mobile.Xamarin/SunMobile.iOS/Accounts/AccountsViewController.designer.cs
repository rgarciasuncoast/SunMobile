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
    [Register ("AccountsViewController")]
    partial class AccountsViewController
    {
        [Outlet]
        UIKit.UIButton btnCloseSlideout { get; set; }


        [Outlet]
        UIKit.UIButton btnDismissRocketChecking { get; set; }


        [Outlet]
        UIKit.UIButton btnOpenRocketChecking { get; set; }


        [Outlet]
        UIKit.UIButton btnRocketChecking { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segmentPrimaryJoints { get; set; }


        [Outlet]
        UIKit.UITableView tableViewAccounts { get; set; }


        [Outlet]
        UIKit.UIView viewSlideout { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCloseSlideout != null) {
                btnCloseSlideout.Dispose ();
                btnCloseSlideout = null;
            }

            if (btnDismissRocketChecking != null) {
                btnDismissRocketChecking.Dispose ();
                btnDismissRocketChecking = null;
            }

            if (btnOpenRocketChecking != null) {
                btnOpenRocketChecking.Dispose ();
                btnOpenRocketChecking = null;
            }

            if (btnRocketChecking != null) {
                btnRocketChecking.Dispose ();
                btnRocketChecking = null;
            }

            if (segmentPrimaryJoints != null) {
                segmentPrimaryJoints.Dispose ();
                segmentPrimaryJoints = null;
            }

            if (tableViewAccounts != null) {
                tableViewAccounts.Dispose ();
                tableViewAccounts = null;
            }

            if (viewSlideout != null) {
                viewSlideout.Dispose ();
                viewSlideout = null;
            }
        }
    }
}