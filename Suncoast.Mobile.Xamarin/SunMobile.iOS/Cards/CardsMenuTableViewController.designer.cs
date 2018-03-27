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
    [Register ("CardsMenuTableViewController")]
    partial class CardsMenuTableViewController
    {
        [Outlet]
        UIKit.UILabel lblTravelNotifications { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMenu { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblTravelNotifications != null) {
                lblTravelNotifications.Dispose ();
                lblTravelNotifications = null;
            }

            if (tableViewMenu != null) {
                tableViewMenu.Dispose ();
                tableViewMenu = null;
            }
        }
    }
}