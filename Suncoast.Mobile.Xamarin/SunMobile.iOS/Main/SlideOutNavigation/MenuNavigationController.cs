using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Authentication;
using SunMobile.iOS.BillPay;
using SunMobile.iOS.Cards;
using SunMobile.iOS.Common;
using SunMobile.iOS.Deposits;
using SunMobile.iOS.Documents;
using SunMobile.iOS.ExternalServices;
using SunMobile.iOS.LoanCenter;
using SunMobile.iOS.Messaging;
using SunMobile.iOS.Profile;
using SunMobile.iOS.Transfers;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Main
{
	public class MenuNavigationController : BaseNavigationController
	{
		private readonly SlideoutNavigationController _slideoutNavigationController;
		private UIViewController _requestedViewController;
		public UIViewController CurrentViewController { get; private set; }
        public MenuTableViewController MenuTableViewController { get; private set; }

		public MenuNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
			: base(rootViewController)
		{
			_slideoutNavigationController = slideoutNavigationController;
            MenuTableViewController = (MenuTableViewController)rootViewController;
		}

		public override async void PushViewController(UIViewController viewController, bool animated)
		{
			if (_slideoutNavigationController == null)
			{
				base.PushViewController(viewController, animated);
			}
			else
			{
				_slideoutNavigationController.Close(true);
				//_slideoutNavigationController.CanClose = true;

				bool isAuthenticated = SessionSettings.Instance.IsAuthenticated;

				if (viewController.GetType().ToString().EndsWith("AccountsViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
						viewController = accountsViewController;
						Logging.Track("Starting accounts.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("CardsMenuTableViewController", System.StringComparison.Ordinal))				
				{
					if (isAuthenticated)
					{
						var cardsMenuTableViewController = AppDelegate.StoryBoard.InstantiateViewController("CardsMenuTableViewController") as CardsMenuTableViewController;
						viewController = cardsMenuTableViewController;
						Logging.Track("Starting cards.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("TransfersTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var transfersTableViewController = AppDelegate.StoryBoard.InstantiateViewController("TransfersTableViewController") as TransfersTableViewController;
						viewController = transfersTableViewController;
						Logging.Track("Starting transfers.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("DepositsTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var depositsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("DepositsTableViewController") as DepositsTableViewController;
						viewController = depositsTableViewController;
						Logging.Track("Starting deposits.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("BillPayMenuTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var billPayViewController = AppDelegate.StoryBoard.InstantiateViewController("BillPayMenuTableViewController") as BillPayMenuTableViewController;
						viewController = billPayViewController;
						Logging.Track("Starting bill pay.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("SunMoneyViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var sunMoneyViewController = AppDelegate.StoryBoard.InstantiateViewController("SunMoneyViewController") as SunMoneyViewController;
						viewController = sunMoneyViewController;
						Logging.Track("Starting SunMoney.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("MessageCenterTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var messageCenterTableViewController = AppDelegate.StoryBoard.InstantiateViewController("MessageCenterTableViewController") as MessageCenterTableViewController;
						viewController = messageCenterTableViewController;
						Logging.Track("Starting messaging.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("DocumentsMenuTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var documentsMenuTableViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentsMenuTableViewController") as DocumentsMenuTableViewController;
						viewController = documentsMenuTableViewController;
						Logging.Track("Starting documents.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("LocationsViewController", System.StringComparison.Ordinal))
				{
					Logging.Track("Starting locations.");
				}

                if (viewController.GetType().ToString().EndsWith("LoanCenterMenuTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
                        var loanCenterMenuTableViewController = AppDelegate.StoryBoard.InstantiateViewController("LoanCenterMenuTableViewController") as LoanCenterMenuTableViewController;
                        viewController = loanCenterMenuTableViewController;
						Logging.Track("Starting loan center.");
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController.GetType().ToString().EndsWith("ProfileTableViewController", System.StringComparison.Ordinal))
				{
					if (isAuthenticated)
					{
						var profileViewController = AppDelegate.StoryBoard.InstantiateViewController("ProfileTableViewController") as ProfileTableViewController;
						viewController = profileViewController;
						Logging.Track("Starting profile.");

						var methods = new AuthenticationMethods();
						var challengeResponse = await methods.IsOutOfBandChallengeRequired(OutOfBandTransactionTypes.Profile, View, null, profileViewController);

						if (challengeResponse != null && challengeResponse.OutOfBandChallengeRequired)
						{
							viewController = null;
						}
					}
					else
					{
						viewController = GetAuthenticationViewController(viewController);
					}
				}

				if (viewController != null && viewController.GetType().ToString().EndsWith("ContactUsTableViewController", System.StringComparison.Ordinal))
				{
					Logging.Track("Starting about.");
				}

				if (viewController != null)
				{
					_slideoutNavigationController.SetMainViewController(new MainNavigationController(viewController, _slideoutNavigationController), animated);
				}
			}

			CurrentViewController = viewController;
		}

		private UIViewController GetAuthenticationViewController(UIViewController viewController)
		{
			var adaptiveAuthenticationViewController = AppDelegate.StoryBoard.InstantiateViewController("AdaptiveAuthenticationViewController") as AdaptiveAuthenticationViewController;
			_requestedViewController = viewController;

			if (adaptiveAuthenticationViewController != null)
			{
                adaptiveAuthenticationViewController.Authenticated += async (arg1, arg2) => 				
				{
                    if (arg1 == "authenticated")
					{
						Logging.Identify(SessionSettings.Instance.UserId);
						Logging.Track("Authenticated.");

                        var mobileStatusResponse = (MobileLoginResponse)arg2;

						var lastAuthenticatedMember = RetainedSettings.Instance.LastAuthenticatedMemberId;

						if (lastAuthenticatedMember != null && lastAuthenticatedMember != SessionSettings.Instance.UserId)
						{
							RetainedSettings.Instance.ClearAlerts();
						}

						RetainedSettings.Instance.LastAuthenticatedMemberId = SessionSettings.Instance.UserId;

						RegisterWithSunblockForNotifications();
                        CheckForReminders(mobileStatusResponse.ShouldShowUpdateNotification);

						// Display online disclosure
						var methods = new AuthenticationMethods();
                        if (!await methods.IsOnLineDisclosureAccepted(mobileStatusResponse.IsOnlineDisclosureAccepted, mobileStatusResponse.OnlineBankingAgreementText, View))
						{
							SignOut();
						}
						else
						{
							// EStatement Opt-In
                            await methods.EstatementsOptIn(mobileStatusResponse.EStatementOptInViewed, mobileStatusResponse.EStatementsEnrolled, mobileStatusResponse.EStatementAgreementText, View);
							viewController = _requestedViewController;
							PushViewController(viewController, true);

                            // Courtesy Pay Bill Pay Opt-In
                            // TODO: Wire this up when completed
                            bool shouldShowCourtesyPayOptIn = false;

                            if (shouldShowCourtesyPayOptIn)
                            {
                                var promotionViewController = AppDelegate.StoryBoard.InstantiateViewController("PromotionViewController") as PromotionViewController;
                                promotionViewController.HeaderText = "Courtesy Pay";
                                promotionViewController.Url = "https://www.suncoastcreditunion.com/community/promotions/auto-loan-refinance";
                                promotionViewController.Finished += (obj) =>
                                {
                                    if (obj == true)
                                    {
                                        // Sign them up for courtesy pay.
                                    }
                                    {
                                        // Decline them from courtesy pay.
                                    }

                                    viewController = _requestedViewController;
                                    PushViewController(viewController, true);
                                };

                                PushViewController(promotionViewController, true);    
                            }
						}
					}
					else
					{
						PopToRootViewController(true);
					}
				};

				viewController = adaptiveAuthenticationViewController;
			}

			return viewController;
		}

		private async void RegisterWithSunblockForNotifications()
		{
			if (!string.IsNullOrEmpty(SessionSettings.Instance.DeviceToken))
			{
				if (SessionSettings.Instance.UserId != null)
				{
					var request = new PSNRegistrationData
					{
						PlatformSpecificHandle = SessionSettings.Instance.DeviceToken
					};

					var methods = new AuthenticationMethods();
					await methods.RegisterForNotification(request, View);
				}
			}
		}

        private void CheckForReminders(bool shouldShowUpdateNotification)
		{
			SessionSettings.Instance.ShowPasswordReminder = shouldShowUpdateNotification;
			CommonMethods.AddBottomToolbar(CurrentViewController);			
		}

		public async void NetworkError()
		{
			await AlertMethods.Alert(View, "SunMobile", "Unexpected network error.", "OK");
		}

		public async void TimeOut()
		{
			if (SessionSettings.Instance.IsAuthenticated)
			{
				SessionSettings.Instance.ClearAll();

				SessionSettings.Instance.HasSignedOutOrTimedOut = true;

				await AlertMethods.Alert(View, "SunMobile",
					CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "C83D707C-4C55-4287-BC36-5A1E6F843B7B", "Your session has timed out and you will be logged off."),
					CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "28B92076-D260-4DA1-8552-A2A129A1BEC2", "OK")); 

				var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
				PushViewController(accountsViewController, true);
			}
		}

		public async void SignOut()
		{
			if (SessionSettings.Instance.IsAuthenticated)
			{
				SessionSettings.Instance.ClearAll();

				SessionSettings.Instance.HasSignedOutOrTimedOut = true;

				await AlertMethods.Alert(View, "SunMobile",
					CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "D986717E-0D20-40F3-A5A5-051A627E11B7", "You have been successfully signed out."),
					CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "28B92076-D260-4DA1-8552-A2A129A1BEC2", "OK"));

				var authenthenticationMethods = new AuthenticationMethods();
				#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				authenthenticationMethods.Logout(null, View);
				#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

				var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
				PushViewController(accountsViewController, true);
			}
		}

		public void PopBackAndRunController(UIViewController controller)
		{
			PopToRootViewController(true);
			PushViewController(controller, true);
		}

		public void SetCurrentViewController(UIViewController viewController)
		{
			CurrentViewController = viewController;
		}

		public override bool ShouldAutorotate()
		{
			if (CurrentViewController != null)
			{
				return CurrentViewController.ShouldAutorotate();
			}

			return base.ShouldAutorotate();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			if (CurrentViewController != null)
			{
				return CurrentViewController.GetSupportedInterfaceOrientations();
			}

			return base.GetSupportedInterfaceOrientations();
		}
	}
}	