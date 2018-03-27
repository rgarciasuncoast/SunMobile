using System;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public partial class SubAccountsConfirmationViewController : SubAccountsBaseContentViewController, ISubAccountsView
    {
        public SubAccountsConfirmationViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (HandlesKeyboardNotifications())
            {
                RegisterForKeyboardNotifications();
                DismissKeyboardOnBackgroundTap();
            }

            lblMemberFullName.Text = ((SubAccountsViewController)ParentViewController.ParentViewController).MemberFullName;

            if (((SubAccountsViewController)ParentViewController.ParentViewController).CreateDebitCard)
            {
                labelConfirmationDescription.Text = CultureTextProvider.GetMobileResourceText(cultureViewId, "6C08C3C1-9CCC-4766-92C9-53AF3EB06232", "By proceeding, after electronically signing below, " +
                "you will be opening a Smart Checking™ account.\n\nYour requested debit card will be mailed to your address of record and received within " +
                "7 - 10 business days.Your PIN will be received 3 - 5 business days following receipt of the card.\n\nIf immediate card access is required, " +
                "you may request an Instant Issue Debit Card by visiting your local Suncoast Credit Union branch.Please note: If a debit card has already been " +
                "ordered, we will not be able to fulfill your Instant Issue Debit Card request.");
            }
            else
            {
                labelConfirmationDescription.Text = CultureTextProvider.GetMobileResourceText(cultureViewId, "C1999BC8-18E8-43C8-BAEA-09D2AC2827AC", "By proceeding, after electronically signing below, you will be opening a Smart Checking™ account.");
            }

            labelConfirmationDescription.SizeToFit();
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(lblSignatureHeading, cultureViewId, "F0E65503-483D-4D59-95F5-0EAA7A432EE4", "Electronic Signature");
                CultureTextProvider.SetMobileResourceText(lblSignatureConfirmationText, cultureViewId, "CB8D7FFA-265A-43D1-830D-3309F0D702F1", "By typing your name as it appears below, you hereby request the " +
                "account and services indicated above. You also agree that your typed signature shall have the same force and effect as your written signature.");
                txtSignature.Placeholder = CultureTextProvider.GetMobileResourceText(cultureViewId, "D11C9134-AEA8-45D6-AE11-18938E0856B8", "Full Name");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsConfirmationView:SetCultureConfiguration");
            }
        }

        public override bool HandlesKeyboardNotifications()
        {
            return true;
        }

        public string Validate()
        {
            var returnValue = string.Empty;

            if (txtSignature.Text.ToLower().Trim() != lblMemberFullName.Text.ToLower())
            {
                returnValue = CultureTextProvider.GetMobileResourceText(cultureViewId, "B61B9980-66C4-4B2F-84BB-01EBB36F1DD4", "Signature does not match the member's full name.");
            }

            return returnValue;
        }
    }
}