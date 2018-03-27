using System;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public class DocumentViewerPageViewControllerDataSource : UIPageViewControllerDataSource
	{
		private DocumentViewerViewController _parentViewController;
		private int _pages;

		public DocumentViewerPageViewControllerDataSource(DocumentViewerViewController parent, int pages)
		{
			_parentViewController = parent;
			_pages = pages;
		}

		public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var viewController = referenceViewController as DocumentViewerContentViewController;
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
			var viewController = referenceViewController as DocumentViewerContentViewController;
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