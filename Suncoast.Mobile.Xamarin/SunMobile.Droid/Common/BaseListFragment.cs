using System;
using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid
{
	public class BaseListFragment : Android.Support.V4.App.ListFragment, ICultureConfigurationProvider
	{
		protected LinearLayout LayoutMain;
		protected ListView ListViewMain;
		private ProgressBar _progressBar;

		protected virtual void SetupView()
		{
			LayoutMain = (LinearLayout)Activity.FindViewById(Resource.Id.layoutMain);
			ListViewMain = (ListView)Activity.FindViewById(global::Android.Resource.Id.List);
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

		public override void OnResume()
		{
			base.OnResume();

            try
            {
                SetCultureConfiguration();
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BaseListFragment:OnResume");
			}
		}

		public override void OnPause()
		{
			base.OnPause();

			AlertMethods.HideProgressBar(Activity, _progressBar);
		}
	}
}