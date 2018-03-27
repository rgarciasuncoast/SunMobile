using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Gcm.Client;
using Plugin.Permissions;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications;
using SunBlock.DataTransferObjects.Session;
using SunMobile.Droid.About;
using SunMobile.Droid.Accounts;
using SunMobile.Droid.Authentication;
using SunMobile.Droid.Cards;
using SunMobile.Droid.Common;
using SunMobile.Droid.Deposits;
using SunMobile.Droid.Documents;
using SunMobile.Droid.ExternalServices;
using SunMobile.Droid.LoanCenter;
using SunMobile.Droid.Locations;
using SunMobile.Droid.Main;
using SunMobile.Droid.Messaging;
using SunMobile.Droid.Onboarding;
using SunMobile.Droid.Profile;
using SunMobile.Droid.Transfers;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;

using SunMobile.Forms.Views;
using Xamarin.Forms.Platform.Android;

namespace SunMobile.Droid
{
	[Activity(Label = "SunMobile", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/iconwithouttext")]
	public class MainActivity : AppCompatActivity
	{
		private Toolbar _toolbar;
		private DrawerLayout _drawerLayout;
		private NavigationView _navigationView;
		private int _listItemPosition;
		private bool _hasBeenLoaded;
		private bool _showInfoAction;
        private bool _showAddAction;
       

        public void ShowCOntentPage()
        {
         
            var content = new ContactUspage();
            var contentFragment = content.CreateSupportFragment(this);
           SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, contentFragment).Commit();

        }


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);			


            //{
            // #1 Initialize
            Xamarin.Forms.Forms.Init(this, null);

            // Visual Studio App Center
            Microsoft.AppCenter.AppCenter.Start(AppSettings.VisualStudioAppCenterAndroid, typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));

			Logging.Track("Operating Systems", "Operating System", "Android " + Plugin.DeviceInfo.CrossDeviceInfo.Current.Version);
			Logging.Track("Hardware Models", "Hardware Model", "Android " + Plugin.DeviceInfo.CrossDeviceInfo.Current.Model);

			// Notifications - Google Cloud Messaging
			RegisterForNotifications();

			// Allow orientation changes for tablet only
			if (GeneralUtilities.AllowOnlyPortraitOrientation())
			{
				RequestedOrientation = ScreenOrientation.Portrait;
			}

			_hasBeenLoaded = SessionSettings.Instance.HasBeenLoaded;

			if (!_hasBeenLoaded)
			{
				// Clear all previous session settings
				SessionSettings.Instance.ClearAll();
				_hasBeenLoaded = true;
				SessionSettings.Instance.HasBeenLoaded = true;
			}			

			if (GeneralUtilities.IsPhone() || (!GeneralUtilities.IsPhone() && GeneralUtilities.IsOrientationPortrait(this)))
			{
				SetContentView(Resource.Layout.page_home_view);

				_toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

				if (_toolbar != null)
				{
					SetSupportActionBar(_toolbar);
					SupportActionBar.SetDisplayHomeAsUpEnabled(true);
					SupportActionBar.SetHomeButtonEnabled(true);
					SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
				}

				_drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
				_drawerLayout.DrawerOpened += (sender, e) =>
				{
					GeneralUtilities.CloseKeyboard(this);
				};

				_navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
				_navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
				{
					e.MenuItem.SetChecked(true);

					switch (e.MenuItem.ItemId)
					{
						case Resource.Id.nav_accounts:
							ListItemClicked(1);
							break;
						case Resource.Id.nav_cards:
							ListItemClicked(2);
							break;
						case Resource.Id.nav_transfer:
							ListItemClicked(3);
							break;
						case Resource.Id.nav_deposits:
							ListItemClicked(4);
							break;
						case Resource.Id.nav_billpay:
							ListItemClicked(5);
							break;
						case Resource.Id.nav_loancenter:
							ListItemClicked(6);
							break;
						case Resource.Id.nav_sunmoney:
							ListItemClicked(7);
							break;
						case Resource.Id.nav_locations:
							ListItemClicked(8);
							break;
						case Resource.Id.nav_messagecenter:
							ListItemClicked(9);
							break;
						case Resource.Id.nav_documentcenter:
							ListItemClicked(10);
							break;
						case Resource.Id.nav_profile:
							ListItemClicked(11);
							break;
						case Resource.Id.nav_about:
							ListItemClicked(12);
							break;
						case Resource.Id.nav_signout:
							ListItemClicked(13);
							break;
					}

					_drawerLayout.CloseDrawers();
				};
			}
			else
			{
				SetContentView(Resource.Layout.MainMenuContainer_Tablet);

				// Load the static menu
				var fragment = new MainMenuFragment();
				SupportFragmentManager.BeginTransaction().Replace(Resource.Id.list_frame, fragment).Commit();

				_toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

				if (_toolbar != null)
				{
					SetSupportActionBar(_toolbar);
				}
			}

			Title = "Suncoast Credit Union";

			// If first time you will want to go ahead and click first item.
			if (savedInstanceState == null)
			{
				GetStartupSettings();

				if (ShouldShowOnboarding())
				{
					ShowOnboarding();
				}
				else
				{
					ListItemClicked(_listItemPosition);
				}
			}			
		}

		public void SetCultureConfiguration()
		{
            try
            {
                if (_navigationView != null)
                {
                    _navigationView.Menu.FindItem(Resource.Id.nav_accounts).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "956E2062-2EA2-44BD-884F-A92EFDECDF9F", "Accounts"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_cards).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "2481D95B-D465-4B87-AA3A-3CC420B1C296", "Cards"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_transfer).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "294EB33E-925F-42A6-8560-F7C3676530D3", "Transfer Funds"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_deposits).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "99446298-1B9F-41BA-894A-875772D7CF4E", "Deposit Funds"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_billpay).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "FC274687-8339-4618-8960-5C202AC14881", "Bill Pay"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_loancenter).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "72B7A071-14D7-40BB-B1F7-F6C5F94BF188", "Loan Center"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_sunmoney).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "B888D315-F938-4283-BE4A-8624454218FB", "SunMoney"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_locations).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "2896CB59-E999-49A9-9059-4E12786FA1B5", "Find ATM/Branch"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_messagecenter).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "26C013AC-ED93-406F-8DE9-DBB33579F8B5", "Message Center"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_documentcenter).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "9F23B326-B342-47F2-A4BC-D4578D17F9E2", "Documents"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_profile).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "91E519FD-02C1-479B-A770-AC20CE256EB2", "My Profile"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_about).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "9EC061A4-3B17-412A-87C7-FE119ECF96D5", "Contact Us"));
                    _navigationView.Menu.FindItem(Resource.Id.nav_signout).SetTitle(CultureTextProvider.GetMobileResourceText("D7C907BA-E9E8-454E-B87F-AAE7AE226022", "7096369F-20F4-4124-97DC-CC9F72F8BFFF", "Sign Out"));
                }
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:SetCultureConfiguration");
			}
		}

		private async void DisplayModel()
		{
			await AlertMethods.Alert(this, "SunMobile", $"Model: {Build.Model}", "OK");
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
            Android.Support.V4.App.Fragment currentFragment;

			switch (item.ItemId)
			{
				case Resource.Id.btnActionSignOut:
					SignOut();
					return true;
                case Resource.Id.btnActionAdd:
                    currentFragment = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                    if (currentFragment == null || !(currentFragment is AccountsFragment))
                    {
                        var fragment = new AccountsFragment();
                        NavigationService.NavigatePush(fragment, true, false);
                    }
                    else if (currentFragment != null && currentFragment is AccountsFragment)
                    {
                        ((AccountsFragment)currentFragment).AccountOptions();
                    }
                    break;
				case Resource.Id.btnActionInfo:
					currentFragment = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
					if (currentFragment == null || !(currentFragment is PasswordReminderFragment))
					{
						var fragment = new PasswordReminderFragment();
						NavigationService.NavigatePush(fragment, true, false);
					}
					break;
				case Resource.Id.btnActionHome:
					GoHome();
					return true;
				case Android.Resource.Id.Home:
					_drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
					return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		public void ListItemClicked(int position)
		{
			try
			{
				Android.Support.V4.App.Fragment fragment = null;
				_listItemPosition = position;

				switch (position)
				{
					case 0:
					//fragment = new MainMenuFragment();
					//break;                    
					case 1:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new AccountsFragment();
							Logging.Track("Starting accounts.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 2:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new CardsMenuFragment();
							Logging.Track("Starting cards.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 3:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new TransfersFragment();
							Logging.Track("Starting transfers.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 4:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new DepositsFragment();
							Logging.Track("Starting deposits.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 5:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new BillPayMenuFragment();
							Logging.Track("Starting bill pay.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 6:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new LoanCenterMenuFragment();
							Logging.Track("Starting loan center.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 7:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new SunMoneyFragment();
							Logging.Track("Starting SunMoney.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 8:
						fragment = new LocationsFragment();
						Logging.Track("Starting locations.");
						break;
					case 9:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new MessageCenterFragment();
							Logging.Track("Starting messaging.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 10:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new DocumentsMenuFragment();
							Logging.Track("Starting documents.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 11:
						if (SessionSettings.Instance.IsAuthenticated)
						{
							fragment = new ProfileFragment();
							Logging.Track("Starting profile.");
						}
						else
						{
							fragment = new AdaptiveAuthenticationFragment();
						}
						break;
					case 12:
						fragment = new ContactUsFragment();
						Logging.Track("Starting about.");
						break;
					case 13:
						SignOut();
						break;
				}

				if (fragment != null)
				{
					if (GeneralUtilities.IsPhone() || (!GeneralUtilities.IsPhone() && GeneralUtilities.IsOrientationPortrait(this)))
					{
						NavigationService.NavigatePush(fragment, false, false);
					}
					else
					{
						NavigationService.NavigatePush(fragment);
					}

					SessionSettings.Instance.LastMenuIndex = position;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:ListItemClicked");
			}
		}

        public async void ProcessViewControllerState(string state, string clientMessage, MobileLoginResponse mobileLoginResponse)
		{
			try
			{
				Android.Support.V4.App.Fragment fragment = null;

                if (!string.IsNullOrEmpty(clientMessage) && !(clientMessage == "Success" || clientMessage == "Satisfactorio."))
				{
					CancelLogin(clientMessage);
					return;
				}

				switch (state.ToLower())
				{
					case "authenticated":
						SessionSettings.Instance.IsAuthenticated = true;
						Logging.Identify(SessionSettings.Instance.UserId);
						Logging.Track("Authenticated.");

						var lastAuthenticatedMember = RetainedSettings.Instance.LastAuthenticatedMemberId;

						if (lastAuthenticatedMember != null && lastAuthenticatedMember != SessionSettings.Instance.UserId)
						{
							RetainedSettings.Instance.ClearAlerts();
						}

						RetainedSettings.Instance.LastAuthenticatedMemberId = SessionSettings.Instance.UserId;

						RegisterWithSunblockForNotifications();
                        CheckForReminders(mobileLoginResponse.ShouldShowUpdateNotification);						

						// Display online disclosure
						var methods = new AuthenticationMethods();
                        if (!await methods.IsOnLineDisclosureAccepted(mobileLoginResponse.IsOnlineDisclosureAccepted, mobileLoginResponse.OnlineBankingAgreementText, this))
						{
							SignOut();
						}
						else
						{
							// EStatement Opt-In
                            //public async Task EstatementsOptIn(bool eStatementOptInViewed, bool eStatementsEnrolled, string enrollmentAgreementText, object View)
                            await methods.EstatementsOptIn(mobileLoginResponse.EStatementOptInViewed, mobileLoginResponse.EStatementsEnrolled, mobileLoginResponse.EStatementAgreementText, this);
							ListItemClicked(_listItemPosition);
						}
						break;
					case "cancelled":
						CancelLogin();
						break;
					case "unauthorized":
						CancelLogin();
						break;
				}

				if (fragment != null)
				{
					SupportFragmentManager.BeginTransaction()
					   .Replace(Resource.Id.content_frame, fragment)
					   .CommitAllowingStateLoss();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:ListItemClicked");
			}
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
					await methods.RegisterForNotification(request, this);
				}
			}
		}

		private async void GetStartupSettings()
		{
			try
			{
				var methods = new AuthenticationMethods();

				var request = new GetStartupSettingsRequest();
				var response = await methods.GetStartupSettings(request, this);

				if (response != null)
				{
					var dict = new Dictionary<string, string>();

					for (int i = 0; i < response.Keys.Count; i++)
					{
						dict.Add(response.Keys[i], response.Values[i]);
					}

					SessionSettings.Instance.GetStartupSettings = dict;

					var popupText = SessionSettings.Instance.GetStartupSettings["PopUp-Android"];

					if (!string.IsNullOrEmpty(popupText))
					{
						var toastDialog = new ToastDialogFragment(popupText);
						toastDialog.Show(FragmentManager, "dialog");
					}

					if (!ShouldShowOnboarding())
					{
						ShowFeedback();
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:GetStartupSettings");
			}
		}

		private void ShowFeedback()
		{
			try
			{
				if (RetainedSettings.Instance.ShowFeedback)
				{
					var enableFeedback = SessionSettings.Instance.GetStartupSettings["EnableFeedback"];

					if (enableFeedback == "true")
					{
						var feedbackFragment = new FeedbackFragment();
						feedbackFragment.Completed += (obj) =>
						{
							ListItemClicked(_listItemPosition);
						};
						NavigationService.NavigatePush(feedbackFragment, false, true);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:OnCreate");
			}
		}

		private bool ShouldShowOnboarding()
		{
			var show = false;

			try
			{
				if (RetainedSettings.Instance.ShowOnboardingFirstTime || RetainedSettings.Instance.ShowOnboardingUpdate)
				{
					show = true;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:ShouldShowOnboarding");
			}

			return show;
		}

		private bool ShowOnboarding()
		{
			var show = false;

			try
			{
				if (RetainedSettings.Instance.ShowOnboardingFirstTime || RetainedSettings.Instance.ShowOnboardingUpdate)
				{
					show = true;
					var onboardingViewPagerFragment = new OnboardingViewPagerFragment();
					onboardingViewPagerFragment.Completed += (obj) =>
					{
						ListItemClicked(_listItemPosition);
					};
					NavigationService.NavigatePush(onboardingViewPagerFragment, false, true, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:ShowOnboarding");
			}

			return show;
		}

		private void RegisterForNotifications()
		{
			try
			{
				GcmClient.CheckDevice(this);
				GcmClient.CheckManifest(this);

				//Call to Register the device for Push Notifications
				GcmClient.Register(this, GcmBroadcastReceiver.SENDER_IDS);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:RegisterForNotifications");
			}
		}

		public void SetActionBarTitle(string title)
		{
			try
			{
				SupportActionBar.Title = title;
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:SetActionBarTitle");
			}
		}

		// This is used by the GCM service because sending an alert from there is not working.
		public async Task<string> DisplayAlert(string title, string message, params string[] buttons)
		{
			return await AlertMethods.Alert(this, title, message, buttons);
		}

		public void GoHome()
		{
			try
			{
				while (SupportFragmentManager.BackStackEntryCount > 0)
				{
					SupportFragmentManager.PopBackStackImmediate();
				}

				GeneralUtilities.CloseKeyboard(this);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:GoHome");
			}
		}

        private void CheckForReminders(bool shouldShowUpdateNotification)
		{			
            SessionSettings.Instance.ShowPasswordReminder = shouldShowUpdateNotification;

			if (SessionSettings.Instance.ShowPasswordReminder)
			{
				ShowInfoActionButton(true);
			}			
		}

		public void ShowInfoActionButton(bool show)
		{
			_showInfoAction = show;
			InvalidateOptionsMenu();
		}		

        public void ShowAddActionButton(bool show)
        {
            _showAddAction = show;
            InvalidateOptionsMenu();
        }       

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			var inflater = MenuInflater;

            if (_showAddAction && _showInfoAction)
            {
                inflater.Inflate(Resource.Menu.MainActivityActionsInfoAdd, menu);
            }
            else if (_showAddAction)
            {
                inflater.Inflate(Resource.Menu.MainActivityActionsAdd, menu);
            }
            else if (_showInfoAction)
            {
                inflater.Inflate(Resource.Menu.MainActivityActionsInfo, menu);
            }

			return base.OnCreateOptionsMenu(menu);
		}

		public async void CancelLogin(string message = "")
		{
			if (!string.IsNullOrEmpty(message))
			{
				var label = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8", "Login");
				await AlertMethods.Alert(this, label, message, "OK");
			}

			ListItemClicked(_listItemPosition);
		}

		public async void NetworkError()
		{
			await AlertMethods.Alert(this, "SunMobile",
				CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "6D840D69-6915-463B-B3C5-1D811336FFF6", "Unexpected network error."),
				CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "28B92076-D260-4DA1-8552-A2A129A1BEC2", "OK"));
		}

		public async void Timeout()
		{
			if (SessionSettings.Instance.IsAuthenticated)
			{
				SessionSettings.Instance.ClearAll();

				SessionSettings.Instance.HasSignedOutOrTimedOut = true;

				await AlertMethods.Alert(this, "SunMobile",
					CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "C83D707C-4C55-4287-BC36-5A1E6F843B7B", "Your session has timed out and you will be logged off."),
					CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "28B92076-D260-4DA1-8552-A2A129A1BEC2", "OK"));

				var authenthenticationMethods = new AuthenticationMethods();
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				authenthenticationMethods.Logout(null, this);
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed				

				GoHome();

				ListItemClicked(1);
			}
		}

		public async void SignOut()
		{
			SessionSettings.Instance.ClearAll();

			SessionSettings.Instance.HasSignedOutOrTimedOut = true;

			await AlertMethods.Alert(this, "SunMobile",
				CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "D986717E-0D20-40F3-A5A5-051A627E11B7", "You have been successfully signed out."),
				CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "28B92076-D260-4DA1-8552-A2A129A1BEC2", "OK"));

			if (SessionSettings.Instance.IsAuthenticated)
			{
				var authenthenticationMethods = new AuthenticationMethods();
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				authenthenticationMethods.Logout(null, this);
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}			

			GoHome();

			ListItemClicked(1);
		}

		protected override async void OnResume()
		{
			base.OnResume();

			Logging.Log("App became active.");

			if (SessionSettings.Instance.IsAuthenticated)
			{
				var methods = new AuthenticationMethods();

				var request = new AnalyzeRequest
				{
					UserId = SessionSettings.Instance.UserId
				};

				await methods.SessionIsActive(request, this);
			}

            SetCultureConfiguration();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}