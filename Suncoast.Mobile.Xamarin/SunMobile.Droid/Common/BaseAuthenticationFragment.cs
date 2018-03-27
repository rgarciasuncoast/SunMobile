using System;
using Android.OS;
using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid
{
	public class BaseAuthenticationFragment : Android.Support.V4.App.Fragment, ICultureConfigurationProvider
	{
		private ProgressBar _progressBar;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);			
        }

		public virtual void SetupView()
		{
		}

        public virtual void SetCultureConfiguration()
        {
        }

		protected async void InitialViewMessage(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				var label = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8", "Login");
				await AlertMethods.Alert(Activity, label, message, "OK");
			}
		}

		public void ShowActivityIndicator(string message = "Loading...")
		{
			_progressBar = AlertMethods.ShowProgressBar(Activity, _progressBar);
		}

		public void HideActivityIndicator()
		{
			AlertMethods.HideProgressBar(Activity, _progressBar);
		}

		public override void OnResume()
		{
			base.OnResume();

            try
            {
                SetCultureConfiguration();
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BaseAuthenticationFragment:OnResume");
			}
		}

		public override void OnPause()
		{
			base.OnPause();

			AlertMethods.HideProgressBar(Activity, _progressBar);
		}
    }
}