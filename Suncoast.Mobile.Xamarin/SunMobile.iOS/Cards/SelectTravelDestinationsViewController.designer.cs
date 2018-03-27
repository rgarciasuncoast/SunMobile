// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Cards
{
    [Register ("SelectTravelDestinationsViewController")]
    partial class SelectTravelDestinationsViewController
    {
        [Outlet]
        UIKit.UIButton btnRestrictionInformation { get; set; }


        [Outlet]
        UIKit.UILabel lblDisclaimer { get; set; }


        [Outlet]
        UIKit.UITableView tableViewCountries { get; set; }


        [Outlet]
        UIKit.UITableView tableViewList { get; set; }


        [Outlet]
        UIKit.UITableView tableViewStates { get; set; }


        [Outlet]
        UIKit.UITextField txtListType { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnRestrictionInformation != null) {
                btnRestrictionInformation.Dispose ();
                btnRestrictionInformation = null;
            }

            if (lblDisclaimer != null) {
                lblDisclaimer.Dispose ();
                lblDisclaimer = null;
            }

            if (tableViewCountries != null) {
                tableViewCountries.Dispose ();
                tableViewCountries = null;
            }

            if (tableViewStates != null) {
                tableViewStates.Dispose ();
                tableViewStates = null;
            }

            if (txtListType != null) {
                txtListType.Dispose ();
                txtListType = null;
            }
        }
    }
}