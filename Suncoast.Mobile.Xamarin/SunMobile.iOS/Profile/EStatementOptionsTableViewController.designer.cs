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
    [Register ("EStatementOptionsTableViewController")]
    partial class EStatementOptionsTableViewController
    {
        [Outlet]
        UIKit.UIButton btnViewStatementDisclosure { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellAccountEStatementEnrollment { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellENoticeAndEStatementEnrollment { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellENoticeEnrollment { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellStatementDisclosure { get; set; }


        [Outlet]
        UIKit.UISwitch switchAccountEStatementEnrollment { get; set; }


        [Outlet]
        UIKit.UISwitch switchENoticeAndEStatementEnrollment { get; set; }


        [Outlet]
        UIKit.UISwitch switchENoticeEnrollment { get; set; }


        [Outlet]
        UIKit.UITableView tableMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnViewStatementDisclosure != null) {
                btnViewStatementDisclosure.Dispose ();
                btnViewStatementDisclosure = null;
            }

            if (cellAccountEStatementEnrollment != null) {
                cellAccountEStatementEnrollment.Dispose ();
                cellAccountEStatementEnrollment = null;
            }

            if (cellENoticeAndEStatementEnrollment != null) {
                cellENoticeAndEStatementEnrollment.Dispose ();
                cellENoticeAndEStatementEnrollment = null;
            }

            if (cellENoticeEnrollment != null) {
                cellENoticeEnrollment.Dispose ();
                cellENoticeEnrollment = null;
            }

            if (cellStatementDisclosure != null) {
                cellStatementDisclosure.Dispose ();
                cellStatementDisclosure = null;
            }

            if (switchAccountEStatementEnrollment != null) {
                switchAccountEStatementEnrollment.Dispose ();
                switchAccountEStatementEnrollment = null;
            }

            if (switchENoticeAndEStatementEnrollment != null) {
                switchENoticeAndEStatementEnrollment.Dispose ();
                switchENoticeAndEStatementEnrollment = null;
            }

            if (switchENoticeEnrollment != null) {
                switchENoticeEnrollment.Dispose ();
                switchENoticeEnrollment = null;
            }

            if (tableMain != null) {
                tableMain.Dispose ();
                tableMain = null;
            }
        }
    }
}