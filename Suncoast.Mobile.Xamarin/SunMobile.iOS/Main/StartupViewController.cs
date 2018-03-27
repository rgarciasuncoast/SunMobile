using System;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Common;
using SunMobile.iOS.Onboarding;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;

namespace SunMobile.iOS.Main
{
	public partial class StartupViewController : BaseViewController
	{
		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			try
			{
				Title = "Suncoast Credit Union";

				// Load Culture Information
				var methods = new CultureProvider();

				ShowActivityIndicator();

				await methods.GetCultureConfiguration();

				HideActivityIndicator();

				if (StartupSettings.ShowOnboarding())
				{
					var onboardingViewController = AppDelegate.StoryBoard.InstantiateViewController("OnboardingViewController") as OnboardingViewController;
					AppDelegate.SlideOuMenuNavigtionController.MenuViewController.PushViewController(onboardingViewController, true);
				}
				else
				{
					var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
					AppDelegate.SlideOuMenuNavigtionController.MenuViewController.PushViewController(accountsViewController, true);
				}
			}
			catch (Exception ex)
			{
				var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
				AppDelegate.SlideOuMenuNavigtionController.MenuViewController.PushViewController(accountsViewController, true);
				Logging.Log(ex, "StartupViewController:ViewDidLoad");
			}
		}
	}
}