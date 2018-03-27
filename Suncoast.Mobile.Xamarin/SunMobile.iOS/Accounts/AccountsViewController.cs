using System;
using System.Collections.Generic;
using CoreGraphics;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.iOS.Accounts.SubAccounts;
using SunMobile.iOS.Common;
using SunMobile.iOS.LoanCenter;
using SunMobile.iOS.Profile;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public partial class AccountsViewController : BaseViewController
	{
		private AccountListTextViewModel _accountsViewModel;
		private UIRefreshControl _refreshControl;

		public AccountsViewController(IntPtr handle) : base(handle)
		{			
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_refreshControl = new UIRefreshControl();
			tableViewAccounts.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += Refresh;

            /*
            var rightButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            rightButton.Clicked += (sender, e) => AccountOptions(rightButton);
            */

            viewSlideout.Hidden = true;
            viewSlideout.Frame = btnRocketChecking.Frame;

            btnCloseSlideout.TouchUpInside += (sender, e) => CloseSlideout();
            btnOpenRocketChecking.TouchUpInside += (sender, e) => StartSubAcccounts();

            btnRocketChecking.Hidden = true;
            btnRocketChecking.TouchUpInside += (sender, e) =>
            {
                if (_accountsViewModel.RocketEligibility.IsFundedEligible)
                {
                    OpenSlideout();
                }
                else
                {
                    StartSubAcccounts();
                }
            };

            btnDismissRocketChecking.Hidden = true;
            btnDismissRocketChecking.TouchUpInside += (sender, e) => DismissSubAccounts();			

			LoadAccounts(false);			         

			segmentPrimaryJoints.ValueChanged += JointSelectionChanged;			
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "55E7CEFF-4826-4DFF-BF8C-5329FF4ECFF6", "Accounts");
			segmentPrimaryJoints.SetTitle(CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "435CF1F7-5FBB-4317-9D8F-C5ED125BEE17", "Primary"), 0);
			segmentPrimaryJoints.SetTitle(CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "CF221EDA-88A1-47E1-8624-7F9A447FB333", "Joints"), 1);
		}

        private async void AccountOptions(UIBarButtonItem button)
        {
            var response = await AlertMethods.PopoverActionSheet(this, null, null, true, button, "Add a Savings Account", "Add a Money Market Account", "Add a Certificate");

            if (response != "Cancel")
            {
                var contactInfoViewController = AppDelegate.StoryBoard.InstantiateViewController("ContactInfoViewController") as ContactInfoViewController;
                contactInfoViewController.EditMode = false;

                contactInfoViewController.Updated += (obj) =>
                {   
                    var loanCenterViewController = AppDelegate.StoryBoard.InstantiateViewController("LoanCenterViewController") as LoanCenterViewController;
                    loanCenterViewController.LoanType = Shared.Data.LoanCenterTypes.ApplyForLoan;
                    AppDelegate.MenuNavigationController.PopBackAndRunController(loanCenterViewController);
                };

                NavigationController.PushViewController(contactInfoViewController, true);
            }
        }

		private void Refresh(object sender, EventArgs e)
		{
			_accountsViewModel = null;

			var selectedSegmentId = segmentPrimaryJoints.SelectedSegment;

			LoadAccounts(selectedSegmentId == 1);
		}

		private void JointSelectionChanged(object sender, EventArgs e)
		{
			var selectedSegmentId = segmentPrimaryJoints.SelectedSegment;

			LoadAccounts(selectedSegmentId == 1);
		}

		private async void LoadAccounts(bool includeJoints)
		{
			var methods = new AccountMethods();
			var request = new AccountListRequest();

			if (_accountsViewModel == null) 
			{
				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

				_accountsViewModel = await methods.AccountList(request, null);

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.EndRefreshing();
				}
			}

			if (_accountsViewModel?.ClientViewState != null && _accountsViewModel.ClientViewState == "AccountList")
			{
				bool includeRocketAccounts = false;

                if (_accountsViewModel.RocketEligibility != null && _accountsViewModel.RocketEligibility.IsCheckingEligible)
				{
					includeRocketAccounts = true;
                    btnRocketChecking.Hidden = false;
                    btnDismissRocketChecking.Hidden = false;
                    btnRocketChecking.SetImage(UIImage.FromBundle(_accountsViewModel.RocketEligibility.IsFundedEligible ? "subaccountcheckingfundedbanner.png" : "subaccountcheckingbanner.png"), UIControlState.Normal);

                    // TODO: Redo this when XCode 9 is fixed

                    // Resize the image
                    if (View.Frame.Width < 375) // iPhone SE
                    {
                        btnRocketChecking.Frame = new CGRect(View.Frame.X + 8, View.Frame.Bottom - btnRocketChecking.Frame.Height - 108, View.Frame.Width - 16, btnRocketChecking.Frame.Height);
                        btnDismissRocketChecking.Frame = new CGRect(btnRocketChecking.Frame.Right - 40, btnRocketChecking.Frame.Top, 40, btnRocketChecking.Frame.Height);
                    }
                    else
                    {
						btnDismissRocketChecking.Frame = new CGRect(btnRocketChecking.Frame.Right - 40, btnRocketChecking.Frame.Top, 40, btnRocketChecking.Frame.Height);
                    }

                    //tableViewAccounts.Frame = new CGRect(0, 0, tableViewAccounts.Frame.Width, btnRocketChecking.Frame.Top);
				}

				var tableViewSource = new AccountsTableViewSource(_accountsViewModel, includeJoints, true, includeRocketAccounts);
				tableViewSource.ItemSelected += item =>
				{
					var controller = AppDelegate.StoryBoard.InstantiateViewController("TransactionsViewController") as TransactionsViewController;										
                    controller.Account = (SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account)item.Data;
					NavigationController.PushViewController(controller, true);					
				};

				tableViewAccounts.TableFooterView = new UIView(CGRect.Empty);
				tableViewAccounts.Source = tableViewSource;
				tableViewAccounts.ReloadData();
			}
		}	

        private void OpenSlideout()
        {
            viewSlideout.Hidden = false;
            UIView.Animate(.25f, 0f, UIViewAnimationOptions.CurveLinear, () => { viewSlideout.Frame = new CGRect(btnRocketChecking.Frame.Left, btnRocketChecking.Frame.Bottom - 418, btnRocketChecking.Frame.Width, 418); }, null);
        }

        private void CloseSlideout()
        {
            UIView.Animate(.25f, 0f, UIViewAnimationOptions.CurveLinear, () => { viewSlideout.Frame = btnRocketChecking.Frame; }, () => { viewSlideout.Hidden = true; });
        }

        private void StartSubAcccounts()
        {
			var controller = AppDelegate.StoryBoard.InstantiateViewController("SubAccountsViewController") as SubAccountsViewController;
            controller.IsFunded = _accountsViewModel.RocketEligibility.IsFundedEligible;
			NavigationController.PushViewController(controller, true);
		}

        private async void DismissSubAccounts()
        {
            btnRocketChecking.Hidden = true;
            btnDismissRocketChecking.Hidden = true;

            tableViewAccounts.Frame = new CGRect(0, 0, tableViewAccounts.Frame.Width, btnRocketChecking.Frame.Bottom);

			var flagOptions = new List<FlagOption>();			
            var flagDeclined = new FlagOption { FlagType = FlagTypes.InstantCheckingMemberDeclined.ToString(), Value = true };
			flagOptions.Add(flagDeclined);
            var methods = new AuthenticationMethods();
            await methods.SetFlags(flagOptions, View);
        }
	}
}