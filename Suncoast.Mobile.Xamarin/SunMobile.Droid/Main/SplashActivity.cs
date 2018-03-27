using Android.App;
using Android.OS;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid
{
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Save Device Language
			var cultureProvider = new CultureProvider();
			cultureProvider.SetLanguage();

			if (SessionSettings.Instance.Language == SunBlock.DataTransferObjects.Culture.LanguageTypes.Spanish)
			{
				// Load Culture Information
				var methods = new CultureProvider();
				await methods.GetCultureConfiguration();
			}
			else
			{
				RetainedSettings.Instance.Culture = null;
			}

			StartActivity(typeof(MainActivity));
		}
	}
}