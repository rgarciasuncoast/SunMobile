using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Mobile;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public partial class SubAccountsViewController : BaseViewController, ISubAccountsView
    {
        public bool IsFunded { get; set; }
        public int CurrentPage { get; set; }
        private UIPageViewController _pageViewController;
        private List<SubAccountsBaseContentViewController> _contentViewControllers;
        private List<string> _pageHeaders;
        private UIBarButtonItem _rightButton;

        public string MemberFullName { get; set; }
        public string FundingSuffix { get; set; }
        public decimal FundingAmount { get; set; }
        public bool CreateDebitCard { get; set; }
        public bool EnrollInEstatements { get; set; }
        public int CardServiceCode { get; set; }

        private static readonly string cultureViewId = "C717F806-AEDA-4F25-A916-4FA0FC3EA842";
        public MobileStatusResponse<RocketCheckingResponse> CreateAccountResponse { get; set; }

        public SubAccountsViewController(IntPtr handle) : base(handle)
        {
            CurrentPage = 0;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;
            var rightButtonText = CultureTextProvider.GetMobileResourceText(cultureViewId, "5C3D363D-682E-417C-B801-24347767F467", "Next");
            _rightButton = new UIBarButtonItem(rightButtonText, UIBarButtonItemStyle.Plain, null);
            NavigationItem.SetRightBarButtonItem(_rightButton, false);
            _rightButton.Clicked += (sender, e) =>
            {
                if (_rightButton.Title == "Finish")
                {
                    Finish();
                }
                else
                {
                    GotoNextPage();
                }
            };


            btnNext.Clicked += (sender, e) => GotoNextPage();
            btnPrevious.Clicked += (sender, e) => GotoPreviousPage();
            btnPrevious.Enabled = false;

            _pageViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsPageViewController") as SubAccountsPageViewController;
            _contentViewControllers = new List<SubAccountsBaseContentViewController>();

            var yourInformation = CultureTextProvider.GetMobileResourceText(cultureViewId, "8B6E6C60-065E-4E9F-9C8D-E27CD650128E", "Your Information");
            var disclosures = CultureTextProvider.GetMobileResourceText(cultureViewId, "3EEFADD0-624E-45B6-8C34-411D36CADE8A", "Disclosures");
            var selectFundingAccount = CultureTextProvider.GetMobileResourceText(cultureViewId, "ACC5D4DE-9240-4748-8AB0-B3B7D24B701F", "Select Funding Account");
            var debitCard = CultureTextProvider.GetMobileResourceText(cultureViewId, "CCAC366D-0188-4246-BFEC-FC2CFFD4F9C7", "Debit Card");
            var confirmation = CultureTextProvider.GetMobileResourceText(cultureViewId, "BE571AFD-5D29-4626-AF54-6470A1B200FC", "Confirmation");
            var nextSteps = CultureTextProvider.GetMobileResourceText(cultureViewId, "3A7853DA-0ED6-46D2-8FA7-99E01D52162D", "Next Steps");
            Title = CultureTextProvider.GetMobileResourceText(cultureViewId, "4ED0FC62-6883-495B-9F79-56C839E5341F", "Create Account");
            var index = 0;
            _pageHeaders = new List<string>();

            // Contact Information
            var subAccountsContactViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsContactViewController") as SubAccountsContactViewController;
            subAccountsContactViewController.PageIndex = index;
            _contentViewControllers.Add(subAccountsContactViewController);
            _pageHeaders.Add(yourInformation);
            index++;

            // Disclosure
            var subAccountsAgreementViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsAgreementViewController") as SubAccountsAgreementViewController;
            subAccountsAgreementViewController.PageIndex = index;
            _contentViewControllers.Add(subAccountsAgreementViewController);
            _pageHeaders.Add(disclosures);
            index++;

            if (!IsFunded)
            {
                // Funding Account
                var subAccountsFundingViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsFundingViewController") as SubAccountsFundingViewController;
                subAccountsFundingViewController.PageIndex = index;
                _contentViewControllers.Add(subAccountsFundingViewController);
                _pageHeaders.Add(selectFundingAccount);
                index++;
            }

            // Debit Card
            var subAccountsCardViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsCardViewController") as SubAccountsCardViewController;
            subAccountsCardViewController.PageIndex = index;
            _contentViewControllers.Add(subAccountsCardViewController);
            _pageHeaders.Add(debitCard);
            index++;

            // Confirmation
            var subAccountsConfirmationViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsConfirmationViewController") as SubAccountsConfirmationViewController;
            subAccountsConfirmationViewController.PageIndex = index;
            subAccountsConfirmationViewController.IsConfirmationPage = true;
            _contentViewControllers.Add(subAccountsConfirmationViewController);
            _pageHeaders.Add(confirmation);
            index++;

            // Finish
            var subAccountsFinishViewController = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsFinishViewController") as SubAccountsFinishViewController;
            subAccountsFinishViewController.PageIndex = index;
            _contentViewControllers.Add(subAccountsFinishViewController);
            _pageHeaders.Add(nextSteps);
            index++;

            var viewControllers = new UIViewController[] { _contentViewControllers[0] };
            _pageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height - 44);
            AddChildViewController(_pageViewController);
            View.AddSubview(_pageViewController.View);
            _pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
            _pageViewController.DidMoveToParentViewController(this);

            Logging.Track("Starting Rocket Checking.");
        }

        public SubAccountsBaseContentViewController ViewControllerAtIndex(int index)
        {
            return _contentViewControllers[index];
        }

        public void SetProgressImage(int index)
        {
            if (index == 0)
            {
                _pageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height - 44);
            }
            else
            {
                _pageViewController.View.Frame = new CGRect(0, 76, View.Frame.Width, View.Frame.Size.Height - 120);
            }

            CurrentPage = index;
            imageProgress.Image = UIImage.FromBundle(IsFunded ? $"subaccountfundedstep{index + 1}" : $"subaccountstep{index + 1}");
            lblHeader.Text = _pageHeaders[index];

            var rightButtonTextNext = CultureTextProvider.GetMobileResourceText(cultureViewId, "5C3D363D-682E-417C-B801-24347767F467", "Next");
            var rightButtonTextFinish = CultureTextProvider.GetMobileResourceText(cultureViewId, "90C305B5-7C65-4495-821E-84E3F391DB9E", "Finish");
            _rightButton.Title = index == (_contentViewControllers.Count - 1) ? rightButtonTextFinish : rightButtonTextNext;

            btnPrevious.Enabled = index != (_contentViewControllers.Count - 1) && index != 0;
            btnNext.Enabled = index != (_contentViewControllers.Count - 1);
        }

        public async void GotoNextPage()
        {
            if (Validate() == string.Empty)
            {
                var currentViewController = _contentViewControllers[CurrentPage];

                if (currentViewController.IsConfirmationPage)
                {
                    await CreateRocketAccount();

                    //if (CreateAccountResponse != null && CreateAccountResponse.Success && !CreateAccountResponse.OutOfBandChallengeRequired)
                    //{
                    if (CurrentPage < (_contentViewControllers.Count - 1))
                    {
                        var viewController = _contentViewControllers[CurrentPage + 1];
                        _pageViewController.SetViewControllers(new UIViewController[] { viewController }, UIPageViewControllerNavigationDirection.Forward, true, null);
                        CurrentPage++;
                        SetProgressImage(CurrentPage);
                    }
                    //}
                }
                else
                {
                    if (CurrentPage < (_contentViewControllers.Count - 1))
                    {
                        var viewController = _contentViewControllers[CurrentPage + 1];
                        _pageViewController.SetViewControllers(new UIViewController[] { viewController }, UIPageViewControllerNavigationDirection.Forward, true, null);
                        CurrentPage++;
                        SetProgressImage(CurrentPage);
                    }
                }
            }
        }

        public void GotoPreviousPage()
        {
            if (CurrentPage > 0)
            {
                var viewController = _contentViewControllers[CurrentPage - 1];
                _pageViewController.SetViewControllers(new UIViewController[] { viewController }, UIPageViewControllerNavigationDirection.Reverse, true, null);
                CurrentPage--;
                SetProgressImage(CurrentPage);
            }
        }

        private void Finish()
        {
            var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
            AppDelegate.MenuNavigationController.PopBackAndRunController(accountsViewController);
        }

        public string Validate()
        {
            var returnValue = string.Empty;

            var viewController = _contentViewControllers[CurrentPage];

            returnValue = ((ISubAccountsView)viewController).Validate();

            if (!string.IsNullOrEmpty(returnValue))
            {
                AlertMethods.Alert(View, "SunMobile", returnValue, "OK");
            }

            return returnValue;
        }

        public async Task CreateRocketAccount()
        {
            var methods = new AccountMethods();

            var rocketCheckingRequest = new RocketCheckingRequest
            {
                MemberId = GeneralUtilities.GetMemberIdAsInt(),
                FundingSuffix = FundingSuffix,
                FundingAmount = FundingAmount,
                CreateDebitCard = CreateDebitCard,
                EnrollInEstatements = EnrollInEstatements,
                ServiceCode = CardServiceCode
            };

            var request = new MobileDeviceVerificationRequest<RocketCheckingRequest> { Payload = RetainedSettings.Instance.Payload.Payload, Request = rocketCheckingRequest };

            ShowActivityIndicator();

            CreateAccountResponse = await methods.CreateRocketChecking(request, View);

            HideActivityIndicator();
        }
    }
}