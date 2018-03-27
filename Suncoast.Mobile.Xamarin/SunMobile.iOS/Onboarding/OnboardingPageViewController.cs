using System;
using UIKit;

namespace SunMobile.iOS.Onboarding
{
	public partial class OnboardingPageViewController : UIPageViewController
	{
		public OnboardingPageViewController(IntPtr handle) : base(handle)
		{
			var pageController = UIPageControl.Appearance;
			pageController.PageIndicatorTintColor = UIColor.Black;
			pageController.CurrentPageIndicatorTintColor = UIColor.White;
			pageController.BackgroundColor = AppStyles.TableHeaderBackgroundColor;
		}
	}
}