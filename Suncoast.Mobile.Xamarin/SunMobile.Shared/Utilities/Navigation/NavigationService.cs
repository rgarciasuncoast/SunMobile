using System;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.Mobile;

#if __ANDROID__
using Android.Support.V4.App;
using Plugin.CurrentActivity;
using SunMobile.Droid;
using SunMobile.Droid.Profile;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
#endif

#if __IOS__
using SunMobile.iOS;
using SunMobile.iOS.Authentication;
using SunMobile.Shared.Culture;
using UIKit;
#endif

namespace SunMobile.Shared.Navigation
{
	public static class NavigationService
	{
#if __ANDROID__
		public static async void NavigatePush(object fragment, bool addToBackStack = false, bool popToRoot = true, bool commitAllowingStateLoss = false)
		{
			try
			{
				if (fragment != null && (fragment is Fragment || fragment is ListFragment))
				{
					var currentActivity = (MainActivity)CrossCurrentActivity.Current.Activity;
					GeneralUtilities.CloseKeyboard(currentActivity);

					var challengeResponse = new MobileStatusResponse { OutOfBandChallengeRequired = false };

					if (fragment is ProfileFragment)
					{
						if (fragment is BaseFragment)
						{
							((BaseFragment)fragment).ShowActivityIndicator();
						}

						if (fragment is BaseListFragment)
						{
							((BaseListFragment)fragment).ShowActivityIndicator();
						}

						var methods = new AuthenticationMethods();
						challengeResponse = await methods.IsOutOfBandChallengeRequired(OutOfBandTransactionTypes.Profile, currentActivity, null, fragment);

						if (fragment is BaseFragment)
						{
							((BaseFragment)fragment).HideActivityIndicator();
						}

						if (fragment is BaseListFragment)
						{
							((BaseListFragment)fragment).HideActivityIndicator();
						}
					}

					if (challengeResponse == null || !challengeResponse.OutOfBandChallengeRequired)
					{
						if (popToRoot)
						{
							PopToRoot(currentActivity);
						}

						var fragmentTransaction = currentActivity.SupportFragmentManager.BeginTransaction()
							.Replace(Resource.Id.content_frame, fragment is Fragment ? (Fragment)fragment : (ListFragment)fragment);

						if (addToBackStack)
						{
							fragmentTransaction.AddToBackStack(null);
						}

						if (commitAllowingStateLoss)
						{
							fragmentTransaction.CommitAllowingStateLoss();
						}
						else
						{
							fragmentTransaction.Commit();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "NavigationService:NavigateTo");
			}
		}

		public static void NavigatePop(bool popToRoot = false)
		{
			var currentActivity = (MainActivity)CrossCurrentActivity.Current.Activity;

			if (popToRoot)
			{
				PopToRoot(currentActivity);
			}
			else
			{
				currentActivity.OnBackPressed();
			}
		}

		private static void PopToRoot(FragmentActivity activity)
		{
			var currentActivity = (MainActivity)CrossCurrentActivity.Current.Activity;

			while (currentActivity.SupportFragmentManager.BackStackEntryCount > 0)
			{
				currentActivity.SupportFragmentManager.PopBackStackImmediate();
			}
		}

#endif


#if __IOS__
		public static void NavigatePush(UINavigationController navigationController, UIViewController viewController, bool animate = true, bool popToRoot = true)
		{
			try
			{
				if (navigationController != null && viewController != null)
				{
					// Call inauth to see if we need to validate account
					// var requiresValidate = await methods.ValidateNavigation();

					bool requiresValidation = false;

					if (requiresValidation)
					{
						var accountVerificationViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountVerificationViewController") as AccountVerificationViewController;
						accountVerificationViewController.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
						accountVerificationViewController.OutOfBandTransactionType = OutOfBandTransactionTypes.Profile.ToString();
						accountVerificationViewController.Completed += (isValidated) =>
						{
							if (isValidated)
							{
								if (popToRoot)
								{
									AppDelegate.MenuNavigationController.PopToRootViewController(false);
								}

								navigationController.PushViewController(viewController, animate);
							}
						};

						navigationController.PushViewController(accountVerificationViewController, animate);
					}
					else
					{
						if (popToRoot)
						{
							AppDelegate.MenuNavigationController.PopToRootViewController(false);
						}

						navigationController.PushViewController(viewController, animate);
					}			
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "NavigationService:NavigatePush");
			}
		}

		public static void NavigatePop(UINavigationController navigationController, bool animate = true, bool popToRoot = true)
		{
			if (popToRoot)
			{
				AppDelegate.MenuNavigationController.PopToRootViewController(false);
			}
			else
			{
				navigationController.PopViewController(animate);
			}
		}
#endif
	}
}