using System;
using System.Collections.Generic;
using CoreGraphics;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.iOS.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.Sharing;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class DocumentViewerViewController : BaseViewController
	{
		public DocumentViewerTypes DocumentType { get; set; }
		public List<DocumentCenterFile> Files { get; set; }
		public List<string> Urls { get; set; }
		public int CurrentPage { get; set; }
		private UIPageViewController _pageViewController;
		private List<DocumentViewerContentViewController> _contentViewControllers;

		public DocumentViewerViewController(IntPtr handle) : base(handle)
		{
			CurrentPage = 0;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = AppStyles.TableHeaderBackgroundColor;

			btnPrint.Clicked += (sender, e) => Print();
            btnPrint.AccessibilityLabel = "Print";
			btnShare.Clicked += (sender, e) => Share();
            btnShare.AccessibilityLabel = "Share";

			var pages = 0;

			if (DocumentType == DocumentViewerTypes.Url)
			{
				pages = Urls.Count;
			}
			else
			{
				pages = Files.Count;
			}

			_pageViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerPageViewController") as DocumentViewerPageViewController;
			_pageViewController.DataSource = new DocumentViewerPageViewControllerDataSource(this, pages);
			_contentViewControllers = new List<DocumentViewerContentViewController>();

			var index = 0;

			if (DocumentType == DocumentViewerTypes.Url)
			{
				foreach (var url in Urls)
				{
					var documentViewerContentViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerContentViewController") as DocumentViewerContentViewController;
					documentViewerContentViewController.Url = url;
					documentViewerContentViewController.PageIndex = index;
					_contentViewControllers.Add(documentViewerContentViewController);
					index++;
				}
			}
			else
			{
				foreach (var file in Files)
				{
					var documentViewerContentViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerContentViewController") as DocumentViewerContentViewController;
					documentViewerContentViewController.File = file;
					documentViewerContentViewController.PageIndex = index;
					_contentViewControllers.Add(documentViewerContentViewController);
					index++;
				}
			}

			var viewControllers = new UIViewController[] { _contentViewControllers[0] };
			_pageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height - 44);
			AddChildViewController(_pageViewController);
			View.AddSubview(_pageViewController.View);
			_pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
			_pageViewController.DidMoveToParentViewController(this);
		}

		public DocumentViewerContentViewController ViewControllerAtIndex(int index)
		{
			CurrentPage = index;

			return _contentViewControllers[index];
		}

		private void Print()
		{
			Sharing.Print(_contentViewControllers[CurrentPage].GetWebView());
		}

		private void Share()
		{
			Sharing.Share(this, _contentViewControllers[CurrentPage].FileBytes, btnShare);
		}
	}
}