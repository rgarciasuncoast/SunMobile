// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Locations
{
    [Register ("LocationsDetailViewController")]
    partial class LocationsDetailViewController
    {
        [Outlet]
        UIKit.UIButton btnCall { get; set; }


        [Outlet]
        UIKit.UIButton btnGetDirections { get; set; }


        [Outlet]
        UIKit.UILabel lblAddress1 { get; set; }


        [Outlet]
        UIKit.UILabel lblAddress2 { get; set; }


        [Outlet]
        UIKit.UILabel lblLocationName { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCall != null) {
                btnCall.Dispose ();
                btnCall = null;
            }

            if (btnGetDirections != null) {
                btnGetDirections.Dispose ();
                btnGetDirections = null;
            }

            if (lblAddress1 != null) {
                lblAddress1.Dispose ();
                lblAddress1 = null;
            }

            if (lblAddress2 != null) {
                lblAddress2.Dispose ();
                lblAddress2 = null;
            }

            if (lblLocationName != null) {
                lblLocationName.Dispose ();
                lblLocationName = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }
        }
    }
}