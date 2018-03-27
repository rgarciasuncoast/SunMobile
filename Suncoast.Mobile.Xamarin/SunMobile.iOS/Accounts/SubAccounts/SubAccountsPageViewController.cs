using System;
using UIKit;

namespace SunMobile.iOS.Accounts.SubAccounts
{
	public partial class SubAccountsPageViewController : UIPageViewController
	{
		public SubAccountsPageViewController(IntPtr handle) : base(handle)
		{
			var pageController = UIPageControl.Appearance;
            pageController.PageIndicatorTintColor = UIColor.White;
			pageController.CurrentPageIndicatorTintColor = UIColor.White;
            pageController.BackgroundColor = UIColor.White;
            /*
			pageController.PageIndicatorTintColor = UIColor.Black;
            pageController.CurrentPageIndicatorTintColor = UIColor.White;
            pageController.BackgroundColor = AppStyles.TableHeaderBackgroundColor;
            */
		}
	}
}