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
    [Register ("ManageAlertsTableViewController")]
    partial class ManageAlertsTableViewController
    {
        [Outlet]
        UIKit.UILabel lblManageAlertSettingsAccountSpecificAlerts { get; set; }


        [Outlet]
        UIKit.UILabel lblManageAlertSettingsElectronicDocumentAlertPrefs { get; set; }


        [Outlet]
        UIKit.UILabel lblManageAlertSettingsSecurityAlerts { get; set; }


        [Outlet]
        Foundation.NSObject sectionManageAlertSettingsAccountSpecificAlerts { get; set; }


        [Outlet]
        UIKit.UISwitch switchAlerts { get; set; }


        [Outlet]
        UIKit.UISwitch switchEDocumentAlerts { get; set; }


        [Outlet]
        UIKit.UISwitch switchSecurityAlerts { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMenu { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblManageAlertSettingsAccountSpecificAlerts != null) {
                lblManageAlertSettingsAccountSpecificAlerts.Dispose ();
                lblManageAlertSettingsAccountSpecificAlerts = null;
            }

            if (lblManageAlertSettingsElectronicDocumentAlertPrefs != null) {
                lblManageAlertSettingsElectronicDocumentAlertPrefs.Dispose ();
                lblManageAlertSettingsElectronicDocumentAlertPrefs = null;
            }

            if (lblManageAlertSettingsSecurityAlerts != null) {
                lblManageAlertSettingsSecurityAlerts.Dispose ();
                lblManageAlertSettingsSecurityAlerts = null;
            }

            if (switchAlerts != null) {
                switchAlerts.Dispose ();
                switchAlerts = null;
            }

            if (switchSecurityAlerts != null) {
                switchSecurityAlerts.Dispose ();
                switchSecurityAlerts = null;
            }

            if (tableViewMenu != null) {
                tableViewMenu.Dispose ();
                tableViewMenu = null;
            }
        }
    }
}