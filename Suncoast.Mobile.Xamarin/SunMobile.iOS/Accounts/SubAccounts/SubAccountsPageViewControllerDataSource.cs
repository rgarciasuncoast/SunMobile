using UIKit;

namespace SunMobile.iOS.Accounts.SubAccounts
{
	public class SubAccountsPageViewControllerDataSource : UIPageViewControllerDataSource
	{
        private SubAccountsViewController _parentViewController;
		private int _pages;

		public SubAccountsPageViewControllerDataSource(SubAccountsViewController parent, int pages)
		{
			_parentViewController = parent;
			_pages = pages;
		}

		public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
            var viewController = referenceViewController as SubAccountsBaseContentViewController;
			var index = viewController.PageIndex;

            _parentViewController.SetProgressImage(index);

			if (index == 0)
			{
				return null;
			}

			index--;

			return _parentViewController.ViewControllerAtIndex(index);
		}

		public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var viewController = referenceViewController as SubAccountsBaseContentViewController;
			var index = viewController.PageIndex;

            _parentViewController.SetProgressImage(index);

			index++;

			if (index == _pages)
			{
				return null;
			}

			return _parentViewController.ViewControllerAtIndex(index);
		}

        /*
		public override nint GetPresentationCount(UIPageViewController pageViewController)
		{
			return _pages;
		}

		public override nint GetPresentationIndex(UIPageViewController pageViewController)
		{
			return 0;
		}
		*/
	}
}