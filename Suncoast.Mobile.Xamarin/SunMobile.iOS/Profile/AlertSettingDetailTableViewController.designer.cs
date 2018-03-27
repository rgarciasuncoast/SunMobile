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
    [Register ("AlertSettingDetailTableViewController")]
    partial class AlertSettingDetailTableViewController
    {
        [Outlet]
        UIKit.UILabel lblDescription { get; set; }


        [Outlet]
        UIKit.UISwitch switchEnabled { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMain { get; set; }


        [Outlet]
        UIKit.UITextField txtAmount { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblDescription != null) {
                lblDescription.Dispose ();
                lblDescription = null;
            }

            if (switchEnabled != null) {
                switchEnabled.Dispose ();
                switchEnabled = null;
            }

            if (tableViewMain != null) {
                tableViewMain.Dispose ();
                tableViewMain = null;
            }

            if (txtAmount != null) {
                txtAmount.Dispose ();
                txtAmount = null;
            }
        }
    }
}