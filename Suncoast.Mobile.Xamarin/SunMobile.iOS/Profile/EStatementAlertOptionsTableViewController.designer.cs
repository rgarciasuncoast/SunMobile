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
    [Register ("EStatementAlertOptionsTableViewController")]
    partial class EStatementAlertOptionsTableViewController
    {
        [Outlet]
        UIKit.UILabel lblAlertType { get; set; }


        [Outlet]
        UIKit.UILabel lblEDocumentsAlertPrefNotify { get; set; }


        [Outlet]
        UIKit.UILabel lblEDocumentsAlertPrefSendAnAlert { get; set; }


        [Outlet]
        UIKit.UISwitch switchEStatementAlerts { get; set; }


        [Outlet]
        UIKit.UISwitch switchSendPushAlert { get; set; }


        [Outlet]
        UIKit.UITableView tableMain { get; set; }


        [Outlet]
        UIKit.UITextField txtAlertAddress { get; set; }


        [Outlet]
        UIKit.UITextField txtAlertMethod { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblAlertType != null) {
                lblAlertType.Dispose ();
                lblAlertType = null;
            }

            if (lblEDocumentsAlertPrefNotify != null) {
                lblEDocumentsAlertPrefNotify.Dispose ();
                lblEDocumentsAlertPrefNotify = null;
            }

            if (lblEDocumentsAlertPrefSendAnAlert != null) {
                lblEDocumentsAlertPrefSendAnAlert.Dispose ();
                lblEDocumentsAlertPrefSendAnAlert = null;
            }

            if (switchEStatementAlerts != null) {
                switchEStatementAlerts.Dispose ();
                switchEStatementAlerts = null;
            }

            if (switchSendPushAlert != null) {
                switchSendPushAlert.Dispose ();
                switchSendPushAlert = null;
            }

            if (tableMain != null) {
                tableMain.Dispose ();
                tableMain = null;
            }

            if (txtAlertAddress != null) {
                txtAlertAddress.Dispose ();
                txtAlertAddress = null;
            }

            if (txtAlertMethod != null) {
                txtAlertMethod.Dispose ();
                txtAlertMethod = null;
            }
        }
    }
}