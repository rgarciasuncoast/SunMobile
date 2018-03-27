using System;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunMobile.iOS.Profile;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public partial class SubAccountsContactViewController : SubAccountsBaseContentViewController, ISubAccountsView
    {
        public SubAccountsContactViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ClearAll();

            btnUpdate.TouchUpInside += (sender, e) => UpdateContactInformation();
            btnConfirmInfo.TouchUpInside += (sender, e) => ConfirmContactInformation();

            if (View.Frame.Width < 375)
            {
                viewWelcomeText.Frame = new CoreGraphics.CGRect(375 - View.Frame.Width - 10, viewWelcomeText.Frame.Top, viewWelcomeText.Frame.Width, viewWelcomeText.Frame.Height);
            }

            GetMemberInformation();
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(lblTitleBody, cultureViewId, "7EFD9659-0118-4C5F-B550-B5D333D04170", "It is the smart choice for you and " +
                "your community. Get all the benefits of free checking plus the added bonus of helping local schools.");
                CultureTextProvider.SetMobileResourceText(lblInfoConfirm, cultureViewId, "A1C02C99-F922-4E27-A3C8-2AB9EAF62CAD", "Once you have confirmed your information, click 'Information is Correct' or click 'Update Information' to make a change.");
                CultureTextProvider.SetMobileResourceText(btnUpdate, cultureViewId, "AAA7DC49-8503-4270-8774-A7AF5C93555A", "Update Information");
                CultureTextProvider.SetMobileResourceText(btnConfirmInfo, cultureViewId, "6AB51975-6EAD-49FC-A58E-177CAAB1320D", "Information Is Correct");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsContactViewController:SetCultureConfiguration");
            }
        }

        private async void GetMemberInformation()
        {
            var methods = new AccountMethods();
            var request = new MemberInformationRequest
            {
                MemberId = GeneralUtilities.GetMemberIdAsInt()
            };

            ShowActivityIndicator();

            var response = await methods.GetMemberInformation(request, View);

            HideActivityIndicator();

            if (response != null)
            {
                var welcomeText = CultureTextProvider.GetMobileResourceText(cultureViewId, "81B232A0-8B93-40B9-8C50-80E1F6BAABA4", ", as a valued member, we wish to " +
                "extend an invitation to you to open a new checking account to help meet your financial needs.");
                var friendlyFirstName = response.FirstName.Substring(0, 1) + response.FirstName.Substring(1).ToLower();
                lblWelcomeText.Text = friendlyFirstName + welcomeText;
                lblWhatWeHave.Text = CultureTextProvider.GetMobileResourceText(cultureViewId, "60B8A27A-2615-44BA-9250-45FBC733CD04", "Here is the information we have for you:");
                lblFullName.Text = response.FullName;
                lblAddress1.Text = response.Address1;
                lblAddress2.Text = $"{response.City}, {response.State} {response.Zip}";
                lblPhone.Text = response.HomePhone;
                lblEmailAddress.Text = response.EmailAddress;

                ((SubAccountsViewController)ParentViewController.ParentViewController).MemberFullName = response.FullName;
            }
        }

        private void ClearAll()
        {
            lblWelcomeText.Text = string.Empty;
            lblWhatWeHave.Text = string.Empty;
            lblFullName.Text = string.Empty;
            lblAddress1.Text = string.Empty;
            lblAddress2.Text = string.Empty;
            lblPhone.Text = string.Empty;
            lblEmailAddress.Text = string.Empty;
        }

        private void UpdateContactInformation()
        {
            var contactInfoViewController = AppDelegate.StoryBoard.InstantiateViewController("ContactInfoViewController") as ContactInfoViewController;
            contactInfoViewController.EditMode = true;

            contactInfoViewController.Updated += (obj) =>
            {
                GetMemberInformation();
            };

            NavigationController.PushViewController(contactInfoViewController, true);
        }

        private void ConfirmContactInformation()
        {
            ((SubAccountsViewController)ParentViewController.ParentViewController).GotoNextPage();
        }

        public string Validate()
        {
            return string.Empty;
        }
    }
}