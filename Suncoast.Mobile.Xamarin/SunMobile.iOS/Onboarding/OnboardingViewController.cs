using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using SunBlock.DataTransferObjects.OnBoarding;
using SunBlock.DataTransferObjects.Session;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Common;
using SunMobile.Shared;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Onboarding
{
	public partial class OnboardingViewController : BaseViewController
	{
		public int CurrentPage { get; set; }
		private UIPageViewController _pageViewController;
		private List<OnboardingContentViewController> _contentViewControllers;
		private OnboardingCarousel _onboardingCarousel;

		public OnboardingViewController(IntPtr handle) : base(handle)
		{
			CurrentPage = 0;
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = AppStyles.TableHeaderBackgroundColor;

			await LoadOnboardingInfo();

			if (_onboardingCarousel != null)
			{
				_pageViewController = AppDelegate.StoryBoard.InstantiateViewController("OnboardingPageViewController") as OnboardingPageViewController;
				_pageViewController.DataSource = new OnboardingPageViewControllerDataSource(this, _onboardingCarousel.CarouselItems.Count);
				_contentViewControllers = new List<OnboardingContentViewController>();

				var index = 0;

				foreach (var carouselItem in _onboardingCarousel.CarouselItems)
				{
					var onboardingContentViewController = AppDelegate.StoryBoard.InstantiateViewController("OnboardingContentViewController") as OnboardingContentViewController;
					onboardingContentViewController.CarouselItem = carouselItem;
					onboardingContentViewController.PageIndex = index;
					_contentViewControllers.Add(onboardingContentViewController);
					index++;
				}

				var viewControllers = new UIViewController[] { _contentViewControllers[0] };
				_pageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height - 0);
				AddChildViewController(_pageViewController);
				View.AddSubview(_pageViewController.View);
				_pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
				_pageViewController.DidMoveToParentViewController(this);
			}

			RetainedSettings.Instance.ShowOnboardingFirstTime = false;
			RetainedSettings.Instance.ShowOnboardingUpdate = false;
		}

		public OnboardingContentViewController ViewControllerAtIndex(int index)
		{
			CurrentPage = index;

			return _contentViewControllers[index];
		}

		private async Task LoadOnboardingInfo()
		{
			try
			{
				var version = string.Empty;

				var authenticationMethods = new AuthenticationMethods();
				var settingsRequest = new GetStartupSettingsRequest();
				var settingsResponse = await authenticationMethods.GetStartupSettings(settingsRequest, null);

				if (settingsResponse != null)
				{
					var dict = new Dictionary<string, string>();

					for (int i = 0; i < settingsResponse.Keys.Count; i++)
					{
						dict.Add(settingsResponse.Keys[i], settingsResponse.Values[i]);
					}

					SessionSettings.Instance.GetStartupSettings = dict;

					var enableOnboarding = SessionSettings.Instance.GetStartupSettings["EnableOnboarding"];

					if (enableOnboarding == "true")
					{
						if (RetainedSettings.Instance.ShowOnboardingFirstTime)
						{
							version = "0";
						}
						else if (RetainedSettings.Instance.ShowOnboardingUpdate)
						{
							version = GeneralUtilities.GetAppShortVersionNumber();
						}

						var methods = new OnboardingMethods();
						var request = new GetOnboardingInfoRequest { Version = version };

						if (UIScreen.MainScreen.Scale > 2.0)
						{
							request.PictureType = OnboardingPictureTypes.RetinaPlus.ToString();
						}
						else if (UIScreen.MainScreen.Scale > 1.0)
						{
							request.PictureType = OnboardingPictureTypes.Retina.ToString();
						}
						else
						{
							request.PictureType = OnboardingPictureTypes.Standard.ToString();
						}

						ShowActivityIndicator();

						request.PictureType = OnboardingPictureTypes.Standard.ToString();

						var response = await methods.GetOnboardingInfo(request, View);

						if (response?.Result?.CarouselItems != null && response.Result.CarouselItems.Count > 0)
						{
							_onboardingCarousel = response.Result;
						}
						else if (!string.IsNullOrEmpty(version))
						{
							request.Version = "0";
							response = await methods.GetOnboardingInfo(request, View);

							if (response?.Result?.CarouselItems != null && response.Result.CarouselItems.Count > 0)
							{
								_onboardingCarousel = response.Result;
							}
						}

						HideActivityIndicator();
					}
				}

				if (_onboardingCarousel == null)
				{
					var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
					AppDelegate.MenuNavigationController.PushViewController(accountsViewController, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:ShowOnboarding");
			}
		}
	}
}