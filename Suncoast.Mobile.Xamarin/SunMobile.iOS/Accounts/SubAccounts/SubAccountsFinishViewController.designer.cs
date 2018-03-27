// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    [Register ("SubAccountsFinishViewController")]
    partial class SubAccountsFinishViewController
    {
        [Outlet]
        UIKit.UIButton btnFinish { get; set; }


        [Outlet]
        UIKit.UITextView labelNextSteps { get; set; }


        [Outlet]
        UIKit.UITableView tableForms { get; set; }


        [Outlet]
        UIKit.UILabel txtAccountFormsHeading { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (labelNextSteps != null) {
                labelNextSteps.Dispose ();
                labelNextSteps = null;
            }

            if (tableForms != null) {
                tableForms.Dispose ();
                tableForms = null;
            }
        }
    }
}