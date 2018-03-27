using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;

namespace SunMobile.Droid.Common
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : Activity, ICultureConfigurationProvider
    {		
        private ProgressBar _progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			RequestWindowFeature(WindowFeatures.NoTitle);

			if (GeneralUtilities.AllowOnlyPortraitOrientation())
			{
				RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
			}            
        }

        public virtual void SetupView()
        {
        }

		public virtual void SetCultureConfiguration()
		{
		}

		public void ShowActivityIndicator(string message = "Loading...")
		{
			_progressBar = AlertMethods.ShowProgressBar(this, _progressBar);
		}

		public void HideActivityIndicator()
		{
			AlertMethods.HideProgressBar(this, _progressBar);
		}

		protected override void OnResume()
		{
			base.OnResume();

            try
            {
                SetCultureConfiguration();
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "BaseActivity:OnResume");
            }
		}

		protected override void OnPause()
		{
			base.OnPause();

			AlertMethods.HideProgressBar(this, _progressBar);
		}
    }
}