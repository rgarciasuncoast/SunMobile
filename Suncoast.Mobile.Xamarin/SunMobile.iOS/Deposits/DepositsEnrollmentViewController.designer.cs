// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Deposits
{
    [Register ("DepositsEnrollmentViewController")]
    partial class DepositsEnrollmentViewController
    {
        [Outlet]
        UIKit.UIButton btnEmailAgreement { get; set; }


        [Outlet]
        UIKit.UILabel lblAcceptAgreement { get; set; }


        [Outlet]
        UIKit.UILabel lblAgreedToSms { get; set; }


        [Outlet]
        UIKit.UILabel lblRemoteDepositsEnrollmentHeader { get; set; }


        [Outlet]
        UIKit.UISwitch switchAcceptedAgreement { get; set; }


        [Outlet]
        UIKit.UISwitch switchAgreedToSms { get; set; }


        [Outlet]
        UIKit.UITextField txtCellPhone { get; set; }


        [Outlet]
        UIKit.UITextField txtEmail { get; set; }


        [Outlet]
        UIKit.UIWebView webViewAgreementText { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnEmailAgreement != null) {
                btnEmailAgreement.Dispose ();
                btnEmailAgreement = null;
            }

            if (lblAcceptAgreement != null) {
                lblAcceptAgreement.Dispose ();
                lblAcceptAgreement = null;
            }

            if (lblAgreedToSms != null) {
                lblAgreedToSms.Dispose ();
                lblAgreedToSms = null;
            }

            if (lblRemoteDepositsEnrollmentHeader != null) {
                lblRemoteDepositsEnrollmentHeader.Dispose ();
                lblRemoteDepositsEnrollmentHeader = null;
            }

            if (switchAcceptedAgreement != null) {
                switchAcceptedAgreement.Dispose ();
                switchAcceptedAgreement = null;
            }

            if (switchAgreedToSms != null) {
                switchAgreedToSms.Dispose ();
                switchAgreedToSms = null;
            }

            if (txtCellPhone != null) {
                txtCellPhone.Dispose ();
                txtCellPhone = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (webViewAgreementText != null) {
                webViewAgreementText.Dispose ();
                webViewAgreementText = null;
            }
        }
    }
}