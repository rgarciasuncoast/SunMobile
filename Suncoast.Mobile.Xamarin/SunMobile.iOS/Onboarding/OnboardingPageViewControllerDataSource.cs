using System;
using UIKit;

namespace SunMobile.iOS.Onboarding
{
	public class OnboardingPageViewControllerDataSource : UIPageViewControllerDataSource
	{
		private OnboardingViewController _parentViewController;
		private int _pages;

		public OnboardingPageViewControllerDataSource(OnboardingViewController parent, int pages)
		{
			_parentViewController = parent;
			_pages = pages;
		}

		public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var viewController = referenceViewController as OnboardingContentViewController;
			var index = viewController.PageIndex;

			if (index == 0)
			{
				return null;
			}

			index--;

			return _parentViewController.ViewControllerAtIndex(index);
		}

		public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var viewController = referenceViewController as OnboardingContentViewController;
			var index = viewController.PageIndex;

			index++;

			if (index == _pages)
			{
				return null;
			}

			return _parentViewController.ViewControllerAtIndex(index);
		}

		public override nint GetPresentationCount(UIPageViewController pageViewController)
		{
			return _pages;
		}

		public override nint GetPresentationIndex(UIPageViewController pageViewController)
		{
			return 0;
		}
	}
}