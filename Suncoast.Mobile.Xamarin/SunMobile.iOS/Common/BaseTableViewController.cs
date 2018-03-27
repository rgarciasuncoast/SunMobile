using System;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Common
{
    public class BaseTableViewController : UITableViewController, ICultureConfigurationProvider
    {
		protected UIView ActivityIndicator;
		private string _title;

        public BaseTableViewController(IntPtr handle) : base(handle)
        {
			_title = string.Empty;
        }        

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                View.TintColor = AppStyles.TintColor;
                NavigationController.NavigationBar.TintColor = UIColor.White;
                CommonMethods.AddBottomToolbar(this);
                SetCultureConfiguration();
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BaseTableViewController:ViewDidLoad");
			}
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (!string.IsNullOrEmpty(_title))
			{
				Title = _title;
			}

			CommonMethods.SetNavigationMenuButton(this);
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
				ActivityIndicator = AlertMethods.ShowActivityIndicator(NavigationController.View, false);
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