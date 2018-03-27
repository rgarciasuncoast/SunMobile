using System;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using UIKit;

namespace SunMobile.iOS.Common
{
    public class BaseNavigationController : UINavigationController, ICultureConfigurationProvider
    {
		public BaseNavigationController(IntPtr handle) : base(handle)
        {
        }

		public BaseNavigationController(UIViewController rootViewController) : base(rootViewController)
		{
		}

        public virtual void SetCultureConfiguration()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                NavigationBar.BarTintColor = AppStyles.BarTintColor;
                NavigationBar.Translucent = true;
                SetCultureConfiguration();
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BaseNavigationController:ViewDidLoad");
            }
        }
    }
}