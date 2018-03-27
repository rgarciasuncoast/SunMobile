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
    [Register ("ProfileTableViewController")]
    partial class ProfileTableViewController
    {
        [Outlet]
        UIKit.UILabel lblProfileContactInfo { get; set; }


        [Outlet]
        UIKit.UILabel lblProfileElectronicDocumentEnrollment { get; set; }


        [Outlet]
        UIKit.UILabel lblProfileManageAlertSettings { get; set; }


        [Outlet]
        UIKit.UILabel lblProfileUpdatePassword { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblProfileContactInfo != null) {
                lblProfileContactInfo.Dispose ();
                lblProfileContactInfo = null;
            }

            if (lblProfileElectronicDocumentEnrollment != null) {
                lblProfileElectronicDocumentEnrollment.Dispose ();
                lblProfileElectronicDocumentEnrollment = null;
            }

            if (lblProfileManageAlertSettings != null) {
                lblProfileManageAlertSettings.Dispose ();
                lblProfileManageAlertSettings = null;
            }

            if (lblProfileUpdatePassword != null) {
                lblProfileUpdatePassword.Dispose ();
                lblProfileUpdatePassword = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }

            if (tableViewMenu != null) {
                tableViewMenu.Dispose ();
                tableViewMenu = null;
            }
        }
    }
}