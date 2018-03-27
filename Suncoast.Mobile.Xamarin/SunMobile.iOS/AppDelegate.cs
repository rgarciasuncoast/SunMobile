using System;
using Foundation;
using SunMobile.Forms.Views;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Authentication;
using SunMobile.iOS.BillPay;
using SunMobile.iOS.Common;
using SunMobile.iOS.Deposits;
using SunMobile.iOS.Main;
using SunMobile.iOS.Onboarding;
using SunMobile.iOS.Transfers;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Settings;
using UIKit;
using Xamarin.Forms;




namespace SunMobile.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }
        public static UIStoryboard StoryBoard { get; private set; }
        public static SlideoutNavigationController SlideOuMenuNavigtionController { get; private set; }
        public static MenuNavigationController MenuNavigationController { get; private set; }
        public static MainNavigationController MainNavigationController { get; private set; }
        public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

        public static UIViewController DefaultDetailView { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            SessionSettings.Instance.ClearAll();

            //Initialize Forms

            Xamarin.Forms.Forms.Init();

            // Visual Studio App Center
            Microsoft.AppCenter.AppCenter.Start(AppSettings.VisualStudioAppCenteriOS, typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));

            Logging.Track("Operating Systems", "Operating System", "iOS " + Plugin.DeviceInfo.CrossDeviceInfo.Current.Version);
            Logging.Track("Hardware Models", "Hardware Model", "iOS " + Plugin.DeviceInfo.CrossDeviceInfo.Current.Model);

            // Notifications
            RegisterForNotifications();

            // Reset Badge Count
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            // Add Quick Items
            AddShortcutItems(application);

            // Save Device Language
            var cultureProvider = new CultureProvider();
            cultureProvider.SetLanguage();

            #if DEBUG
            // Xamarin Test Cloud Agent
            //Xamarin.Calabash.Start();
            #endif

            StoryBoard = UIStoryboard.FromName("MainStoryboard_iPhone", null);

            var menuTableViewController = StoryBoard.InstantiateViewController("MenuTableViewController") as MenuTableViewController;
            SlideOuMenuNavigtionController = new SlideoutNavigationController();
            DefaultDetailView = new UIViewController();

            MenuNavigationController = new MenuNavigationController(menuTableViewController, SlideOuMenuNavigtionController) { NavigationBarHidden = true };
            MainNavigationController = new MainNavigationController(DefaultDetailView, SlideOuMenuNavigtionController);
            SlideOuMenuNavigtionController.MainViewController = MainNavigationController;
            SlideOuMenuNavigtionController.MenuViewController = MenuNavigationController;

            Window.RootViewController = SlideOuMenuNavigtionController;

            StartupSettings.GetStartupSettings(Window, SlideOuMenuNavigtionController.MenuViewController);

            Window.MakeKeyAndVisible();

            if (SessionSettings.Instance.Language == SunBlock.DataTransferObjects.Culture.LanguageTypes.Spanish)
            {
                var startupViewController = new StartupViewController();
                SlideOuMenuNavigtionController.MenuViewController.PushViewController(startupViewController, true);
            }
            else
            {
                if (StartupSettings.ShowOnboarding())
                {
                    var onboardingViewController = StoryBoard.InstantiateViewController("OnboardingViewController") as OnboardingViewController;
                    SlideOuMenuNavigtionController.MenuViewController.PushViewController(onboardingViewController, true);
                }
                else
                {
                    var accountsViewController = StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                    SlideOuMenuNavigtionController.MenuViewController.PushViewController(accountsViewController, true);
                }
            }

            var shouldPerformAdditionalDelegateHandling = true;

            // Get possible shortcut item
            if (launchOptions != null)
            {
                LaunchedShortcutItem = launchOptions[UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
                shouldPerformAdditionalDelegateHandling = (LaunchedShortcutItem == null);
            }

            return shouldPerformAdditionalDelegateHandling;
        }

        private void RegisterForNotifications()
        {
            try
            {
                // Registers for push notifications for iOS8
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert
                    | UIUserNotificationType.Badge
                    | UIUserNotificationType.Sound,
                    new NSSet());

                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                    UIApplication.SharedApplication.RegisterForRemoteNotifications();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "AppDelegate:RegisterForNotifications");
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            deviceToken = deviceToken.Description.Trim('<', '>').Replace(" ", "");
            SessionSettings.Instance.DeviceToken = deviceToken.ToString();
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                NSObject incomingObject;

                bool success = userInfo.TryGetValue(new NSString("aps"), out incomingObject);

                if (success)
                {
                    //var dict = (NSDictionary)incomingObject;                    
                    //NSObject alertValue = dict.ValueForKey(new NSString("alert"));
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "AppDelegate:DidReceiveRemoteNotification");
            }
        }

        private void AddShortcutItems(UIApplication application)
        {
            #pragma warning disable iOSAndMacApiUsageIssue

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {

                var shortcut1 = new UIMutableApplicationShortcutItem("Accounts", "Accounts")

                {
                    Icon = UIApplicationShortcutIcon.FromTemplateImageName("ic_action_accounts")
                };

                var shortcut2 = new UIMutableApplicationShortcutItem("Transfers", "Transfers")
                {
                    Icon = UIApplicationShortcutIcon.FromTemplateImageName("ic_action_transfer_blue")
                };

                var shortcut3 = new UIMutableApplicationShortcutItem("Deposits", "Deposits")
                {
                    Icon = UIApplicationShortcutIcon.FromTemplateImageName("ic_action_deposits2_blue")
                };

                var shortcut4 = new UIMutableApplicationShortcutItem("Bill Pay", "Bill Pay")
                {
                    Icon = UIApplicationShortcutIcon.FromTemplateImageName("ic_action_billpay")
                };

                application.ShortcutItems = new UIApplicationShortcutItem[] { shortcut1, shortcut2, shortcut3, shortcut4 };
            }

            #pragma warning restore iOSAndMacApiUsageIssue
        }

        public bool HandleShortcutItem(UIApplicationShortcutItem shortcutItem)
        {
            var handled = false;

            if (shortcutItem == null) return false;

            // Take action based on the shortcut type
            switch (shortcutItem.Type)
            {
                case "Accounts":
                    var accountsViewController = StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                    CommonMethods.PopToRootIfOniPad();
                    MenuNavigationController.PushViewController(accountsViewController, true);
                    handled = true;
                    break;
                case "Transfers":
                    var transfersTableViewController = StoryBoard.InstantiateViewController("TransfersTableViewController") as TransfersTableViewController;
                    CommonMethods.PopToRootIfOniPad();
                    MenuNavigationController.PushViewController(transfersTableViewController, true);
                    handled = true;
                    break;
                case "Deposits":
                    var depositsTableViewController = StoryBoard.InstantiateViewController("DepositsTableViewController") as DepositsTableViewController;
                    CommonMethods.PopToRootIfOniPad();
                    MenuNavigationController.PushViewController(depositsTableViewController, true);
                    handled = true;
                    break;
                case "Bill Pay":
                    var billPayViewController = StoryBoard.InstantiateViewController("BillPayMenuTableViewController") as BillPayMenuTableViewController;
                    CommonMethods.PopToRootIfOniPad();
                    MenuNavigationController.PushViewController(billPayViewController, true);
                    handled = true;
                    break;
            }

            return handled;
        }


        public void ShowContactUsPage()
        {

            UIViewController ContactUSViewController =  new ContactUspage().CreateViewController();


            MenuNavigationController.PushViewController(ContactUSViewController, true);
        }
       
       
        public override void OnActivated(UIApplication application)
        {
            HandleShortcutItem(LaunchedShortcutItem);
            LaunchedShortcutItem = null;

            // Remove the blur window.
            Window?.ViewWithTag(221122)?.RemoveFromSuperview();
        }

        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            completionHandler(HandleShortcutItem(shortcutItem));
        }

        // Block third party keyboards.
        public override bool ShouldAllowExtensionPointIdentifier(UIApplication application, NSString extensionPointIdentifier)
        {
            if (extensionPointIdentifier.ToString() == "com.apple.keyboard-service")
            {
                return false;
            }

            return true;
        }

        // This method is invoked when the application is about to move from active to inactive state.
        // OpenGL applications should use this method to pause.
        public override void OnResignActivation(UIApplication application)
        {
            // Blur the window in app switcher for security.
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
                var blurEffectView = new UIVisualEffectView(blurEffect);
                blurEffectView.Frame = Window.Frame;
                blurEffectView.Tag = 221122;
                Window?.AddSubview(blurEffectView);
            }
        }

        // This method should be used to release shared resources and it should store the application state.
        // If your application supports background exection this method is called instead of WillTerminate
        // when the user quits.
        public override void DidEnterBackground(UIApplication application)
        {
        }

        // This method is called as part of the transiton from background to active state.
        public override void WillEnterForeground(UIApplication application)
        {
            // Remove the blur window.
            Window?.ViewWithTag(221122)?.RemoveFromSuperview();

            var mainNavigationController = (MainNavigationController)SlideOuMenuNavigtionController.MainViewController;

            if (mainNavigationController.VisibleViewController is AdaptiveAuthenticationViewController)
            {
                if (!SessionSettings.Instance.IsAuthenticated)
                {
                    ((AdaptiveAuthenticationViewController)mainNavigationController.VisibleViewController).LoginUsingBiometrics();
                }
            }
        }

        // This method is called when the application is about to terminate. Save data, if needed.
        public override void WillTerminate(UIApplication application)
        {
            SessionSettings.Instance.ClearAll();
        }
    }
}