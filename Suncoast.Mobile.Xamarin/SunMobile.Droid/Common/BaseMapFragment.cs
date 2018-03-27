using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid
{
	public class BaseMapFragment : Android.Gms.Maps.SupportMapFragment, ICultureConfigurationProvider
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

		public override void OnResume()
		{
			base.OnResume();

			SetCultureConfiguration();
		}

		public override void OnPause()
		{
			base.OnPause();

			AlertMethods.HideProgressBar(Activity, _progressBar);
		}
	}
}