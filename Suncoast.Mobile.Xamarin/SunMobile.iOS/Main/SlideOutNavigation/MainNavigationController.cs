using UIKit;

namespace SunMobile.iOS.Main
{
	public class MainNavigationController : UINavigationController
	{
		private readonly SlideoutNavigationController _slideoutNavigationController;

		public MainNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
			: this(rootViewController, slideoutNavigationController, 
			       new UIBarButtonItem(UIImage.FromBundle("HamburgerBlack"), UIBarButtonItemStyle.Plain, (s, e) => {}))
		{
		}

		public MainNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController, UIBarButtonItem openMenuButton)
			: base(rootViewController)
		{
			_slideoutNavigationController = slideoutNavigationController;
            openMenuButton.AccessibilityLabel = "Navigation Menu";
			openMenuButton.Clicked += (s, e) =>
			{
				_slideoutNavigationController.ToggleMenu(true);
			};
			rootViewController.NavigationItem.LeftBarButtonItem = openMenuButton;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Delegate = new NavigationControllerDelegate();            

			InteractivePopGestureRecognizer.Enabled = true;
			NavigationBar.BarTintColor = AppStyles.BarTintColor;
			NavigationBar.Translucent = true;
		}

		public override void PushViewController(UIViewController viewController, bool animated)
		{
			// To avoid corruption of the navigation stack during animations disabled the pop gesture
			if (InteractivePopGestureRecognizer != null)
			{
				InteractivePopGestureRecognizer.Enabled = false;
			}

			base.PushViewController(viewController, animated);

			if (_slideoutNavigationController != null)
			{
				_slideoutNavigationController.SetCurrentViewController(viewController);
			}
		}

		public override UIViewController PopViewController(bool animated)
		{
			var viewController = base.PopViewController(animated);

			if (ViewControllers != null && ViewControllers.Length > 0)
			{
				_slideoutNavigationController.SetCurrentViewController(ViewControllers[0]);
			}

			return viewController;
		}

		private class NavigationControllerDelegate : UINavigationControllerDelegate
		{
			public override void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
			{
				/*
				// Hide the text of the back button, but keep the arrow.
				var backItem = navigationController.NavigationBar.BackItem;

				if (backItem != null)
				{
					backItem.Title = string.Empty;
				}
				*/

				// Enable the gesture after the view has been shown
				navigationController.InteractivePopGestureRecognizer.Enabled = true;
			}
		}
	}
}	