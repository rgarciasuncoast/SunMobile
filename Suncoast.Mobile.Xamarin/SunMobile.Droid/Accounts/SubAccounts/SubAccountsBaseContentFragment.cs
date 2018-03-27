using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Mobile;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Accounts.SubAccounts
{
    public class SubAccountsBaseContentFragment : BaseFragment
    {
        public SubAccountsInfo Info { get; set; }
        public int PageIndex { get; set; }
        private int _pages;

        private TextView lblHeaderText;
        private ImageView btnPrevious;
        private ImageView btnNext;
        private ImageView imageProgress;
        private List<string> _pageHeaders;
        private List<int> _progressImages;
        protected static readonly string cultureViewId = "C717F806-AEDA-4F25-A916-4FA0FC3EA842";

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if (savedInstanceState != null)
            {
                var json = savedInstanceState.GetString("SubAccountsInfo");
                Info = JsonConvert.DeserializeObject<SubAccountsInfo>(json);
            }

            var yourInformation = CultureTextProvider.GetMobileResourceText(cultureViewId, "8B6E6C60-065E-4E9F-9C8D-E27CD650128E", "Your Information");
            var disclosures = CultureTextProvider.GetMobileResourceText(cultureViewId, "3EEFADD0-624E-45B6-8C34-411D36CADE8A", "Disclosures");
            var selectFundingAccount = CultureTextProvider.GetMobileResourceText(cultureViewId, "ACC5D4DE-9240-4748-8AB0-B3B7D24B701F", "Select Funding Account");
            var debitCard = CultureTextProvider.GetMobileResourceText(cultureViewId, "CCAC366D-0188-4246-BFEC-FC2CFFD4F9C7", "Debit Card");
            var confirmation = CultureTextProvider.GetMobileResourceText(cultureViewId, "BE571AFD-5D29-4626-AF54-6470A1B200FC", "Confirmation");
            var nextSteps = CultureTextProvider.GetMobileResourceText(cultureViewId, "3A7853DA-0ED6-46D2-8FA7-99E01D52162D", "Next Steps");

            _pageHeaders = Info.IsFunded ? new List<string> { yourInformation, disclosures, debitCard, confirmation, nextSteps } : new List<string> { yourInformation, disclosures, selectFundingAccount, debitCard, confirmation, nextSteps };
            _progressImages = Info.IsFunded ? new List<int> { Resource.Drawable.subaccountfundedstep1, Resource.Drawable.subaccountfundedstep2, Resource.Drawable.subaccountfundedstep3, Resource.Drawable.subaccountfundedstep4, Resource.Drawable.subaccountfundedstep5 } : new List<int> { Resource.Drawable.subaccountstep1, Resource.Drawable.subaccountstep2, Resource.Drawable.subaccountstep3, Resource.Drawable.subaccountstep4, Resource.Drawable.subaccountstep5, Resource.Drawable.subaccountstep6 };

            _pages = _pageHeaders.Count;

            try
            {
                lblHeaderText = Activity.FindViewById<TextView>(Resource.Id.lblHeaderText);
                lblHeaderText.Text = _pageHeaders[Info.CurrentPage];
            }
            catch { }

            try
            {
                btnPrevious = Activity.FindViewById<ImageView>(Resource.Id.btnPrevious);
                btnPrevious.Enabled = Info.CurrentPage > 0;
                btnPrevious.Click += (sender, e) => GotoPreviousPage();
            }
            catch { }

            try
            {
                btnNext = Activity.FindViewById<ImageView>(Resource.Id.btnNext);
                btnNext.Enabled = Info.CurrentPage < _pages;
                btnNext.Click += (sender, e) => GotoNextPage();
            }
            catch { }

            try
            {
                imageProgress = Activity.FindViewById<ImageView>(Resource.Id.imageProgress);
                imageProgress.SetImageResource(_progressImages[Info.CurrentPage]);
            }
            catch { }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            var json = JsonConvert.SerializeObject(Info);
            outState.PutString("SubAccountsInfo", json);

            base.OnSaveInstanceState(outState);
        }

        public void GotoNextPage()
        {
            if (ValidatePage() == string.Empty)
            {
                if (Info.CurrentPage < (_pages - 1))
                {
                    Info.CurrentPage++;

                    SubAccountsBaseContentFragment fragment = null;

                    switch (Info.CurrentPage)
                    {
                        case 0:
                            fragment = new SubAccountsContactFragment();
                            break;
                        case 1:
                            fragment = new SubAccountsAgreementFragment();
                            break;
                        case 2:
                            fragment = new SubAccountsCardFragment();
                            break;
                        case 3:
                            fragment = new SubAccountsConfirmationFragment();
                            break;
                        case 4:
                            //fragment = new SubAccountsFinishFragment();
                            break;
                    }

                    if (fragment != null)
                    {
                        fragment.PageIndex = Info.CurrentPage;
                        fragment.Info = Info;
                        NavigationService.NavigatePush(fragment, true, false);
                    }
                }
                else if (Info.CurrentPage == (_pages - 1))
                {
                    // We are finished!
                    var accountsFragment = new AccountsFragment();
                    NavigationService.NavigatePush(accountsFragment, false, true);
                }
            }
        }

        public void GotoPreviousPage()
        {
            if (Info.CurrentPage > 0)
            {
                Info.CurrentPage--;
                NavigationService.NavigatePop();
            }
        }

        public override void OnResume()
        {
            // This is to make sure the CurrentPage is always correct.  Even if they hit the hardware Back button.
            base.OnResume();

            if (Info.CurrentPage != PageIndex)
            {
                Info.CurrentPage = PageIndex;

                try
                {
                    lblHeaderText.Text = _pageHeaders[Info.CurrentPage];
                    imageProgress.SetImageResource(_progressImages[Info.CurrentPage]);
                }
                catch { }
            }
        }

        public async Task CreateRocketAccount(bool gotoFinishFragment)
        {
            MobileStatusResponse<RocketCheckingResponse> createRocketCheckingResponse;

            bool bContinue = false;

            if (gotoFinishFragment)
            {
                SessionSettings.Instance.CreateRocketCheckingResponse = null;
                bContinue = true;
            }
            else
            {
                createRocketCheckingResponse = SessionSettings.Instance.CreateRocketCheckingResponse;

                if (createRocketCheckingResponse != null && createRocketCheckingResponse.OutOfBandChallengeRequired)
                {
                    bContinue = true;
                }
            }

            if (bContinue)
            {
                var methods = new AccountMethods();

                var rocketCheckingRequest = new RocketCheckingRequest
                {
                    MemberId = GeneralUtilities.GetMemberIdAsInt(),
                    FundingSuffix = Info.FundingSuffix,
                    FundingAmount = Info.FundingAmount,
                    CreateDebitCard = Info.CreateDebitCard,
                    EnrollInEstatements = Info.EnrollInEstatements,
                    ServiceCode = Info.CardServiceCode
                };

                var request = new MobileDeviceVerificationRequest<RocketCheckingRequest> { Payload = RetainedSettings.Instance.Payload.Payload, Request = rocketCheckingRequest };

                var subAccountsFinishFragment = new SubAccountsFinishFragment();
                Info.CurrentPage++;
                subAccountsFinishFragment.PageIndex = Info.CurrentPage;
                subAccountsFinishFragment.Info = Info;

                ShowActivityIndicator();

                createRocketCheckingResponse = await methods.CreateRocketChecking(request, Activity, gotoFinishFragment ? subAccountsFinishFragment : null);
                SessionSettings.Instance.CreateRocketCheckingResponse = createRocketCheckingResponse;

                HideActivityIndicator();

                if (gotoFinishFragment)
                {
                    if (createRocketCheckingResponse != null && !createRocketCheckingResponse.OutOfBandChallengeRequired)
                    {
                        // Go to Finish
                        var fragment = new SubAccountsFinishFragment();
                        Info.CurrentPage++;
                        fragment.PageIndex = Info.CurrentPage;
                        fragment.Info = Info;
                        NavigationService.NavigatePush(fragment, true, false);
                    }
                }
            }
        }

        public string ValidatePage()
        {
            var returnValue = string.Empty;

            returnValue = ((ISubAccountsView)this).Validate();

            if (!string.IsNullOrEmpty(returnValue))
            {
                AlertMethods.Alert(Activity, "SunMobile", returnValue, "OK");
            }

            return returnValue;
        }
    }
}