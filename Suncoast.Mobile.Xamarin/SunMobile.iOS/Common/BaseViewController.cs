using System;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using UIKit;

namespace SunMobile.iOS.Common
{
    public class BaseViewController : UIViewController, ICultureConfigurationProvider
    {
		protected UIView ActivityIndicator;
		private string _title;

		public BaseViewController()
		{
		}

        public BaseViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                View.TintColor = AppStyles.TintColor;
                NavigationController.NavigationBar.TintColor = UIColor.White;
                EdgesForExtendedLayout = UIRectEdge.None;
                SetCultureConfiguration();
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BaseViewController:ViewDidLoad");
			}
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

            SetCultureConfiguration();

			if (!string.IsNullOrEmpty(_title))
			{
				Title = _title;
			}

			CommonMethods.AddBottomToolbar(this);
			CommonMethods.SetNavigationMenuButton(this);

            /*
            if (GeneralUtilities.IsIPhoneX())
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    View.Frame = new CoreGraphics.CGRect(View.Frame.X, View.Frame.Y, View.Frame.Width, View.Frame.Height - 88);
                    View.Bounds = new CoreGraphics.CGRect(View.Bounds.X, View.Bounds.Y, View.Bounds.Width, View.Bounds.Height - 88);

                    var guide = View.SafeAreaLayoutGuide;

                    var constraints = new[]
                    {
                        View.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor(guide.TopAnchor, 1),
                        guide.BottomAnchor.ConstraintEqualToSystemSpacingBelowAnchor(View.BottomAnchor, 1)
                    };

                    NSLayoutConstraint.ActivateConstraints(constraints);
                }
            }
            */  
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			// Make title blank so the back button for the next view will just be the back icon.
			_title = Title;
			Title = string.Empty;

			HideActivityIndicator();
		}

		public virtual void SetCultureConfiguration()
		{
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			CommonMethods.SetNavigationMenuButton(this);
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				return UIInterfaceOrientationMask.Portrait;
			}
			else
			{
				return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown | UIInterfaceOrientationMask.Landscape | UIInterfaceOrientationMask.LandscapeLeft | UIInterfaceOrientationMask.LandscapeRight;
			}
		}

		public void ShowActivityIndicator()
		{
			try
			{
				ActivityIndicator = AlertMethods.ShowActivityIndicator(NavigationController.View);
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		public void HideActivityIndicator()
		{
			try
			{
				AlertMethods.HideActivityIndicator(ActivityIndicator);
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}
		}
    }
}