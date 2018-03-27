using System;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class DocumentViewerPageViewController : UIPageViewController
	{
		public DocumentViewerPageViewController(IntPtr handle) : base(handle)
		{
			var pageController = UIPageControl.Appearance;
			pageController.PageIndicatorTintColor = UIColor.Black;
			pageController.CurrentPageIndicatorTintColor = UIColor.White;
			pageController.BackgroundColor = AppStyles.TableHeaderBackgroundColor;
		}
	}
}