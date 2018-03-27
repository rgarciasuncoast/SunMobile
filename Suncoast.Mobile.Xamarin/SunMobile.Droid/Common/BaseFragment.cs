using System;
using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid
{
	public class BaseFragment : Android.Support.V4.App.Fragment, ICultureConfigurationProvider
	{		
        private ProgressBar _progressBar;

		public virtual void SetupView()
		{
		}

		public virtual void SetCultureConfiguration()
		{
		}

        public void ShowActivityIndicator(string message = "Loading...")
        {
            _progressBar = AlertMethods.ShowProgressBar(Activity, _progressBar);
        }

        public void HideActivityIndicator()
        {
            AlertMethods.HideProgressBar(Activity, _progressBar);
        }

        /*
		public void ShowActivityIndicator(string message = "Loading...")
		{
			if (message == "Loading...")
			{
				message = CultureTextProvider.GetMobileResourceText("7845DFA9-A8BB-4F47-81A8-7244D2AC41C4", "6A655E2B-2EF9-4028-8AD0-D45049222F5D", "Loading...");
			}

			_activityIndicator = AlertMethods.ShowActivityIndicator(Activity, "SunMobile", message);
		}

		public void HideActivityIndicator()
		{
			AlertMethods.HideActivityIndicator(_activityIndicator);
		}
		*/

        public override void OnResume()
        {
            base.OnResume();

            try
            {
                SetCultureConfiguration();
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BaesFragment:OnResume");
			}
        }

		public override void OnPause()
		{
			base.OnPause();

            AlertMethods.HideProgressBar(Activity, _progressBar);
		}
	}
}