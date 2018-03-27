using System;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Shared.Culture
{
	public class CultureProvider
	{
		public async Task<bool> GetCultureConfiguration()
		{
			var success = false;

			try
			{
				var culture = RetainedSettings.Instance.Culture;
				var lastUpdateTime = DateTime.MinValue;

				if (culture != null)
				{
					lastUpdateTime = culture.LastUpdateTimeUtc;
				}

				var methods = new AuthenticationMethods();
				var response = await methods.GetCultureConfiguration(lastUpdateTime, this);

				if (response != null && response.Success && response.Result != null)
				{
					RetainedSettings.Instance.Culture = response.Result;
					success = true;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Culture:GetCultureConfiguration");
			}

			return success;
		}

		public CultureConfiguration GetMobileCultureConfiguration()
		{
			return RetainedSettings.Instance.Culture;
		}

		public void SetLanguage()
		{
			var language = LanguageTypes.English;

			try
			{
				var languageCode = "en";

				#if __IOS__
				languageCode = Foundation.NSLocale.PreferredLanguages[0].ToLower();
				#endif

				#if __ANDROID__
				languageCode = Java.Util.Locale.Default.GetDisplayLanguage(Java.Util.Locale.Default);
				#endif

				if (!string.IsNullOrEmpty(languageCode) && languageCode.Length >= 2)
				{
					languageCode = languageCode.Substring(0, 2).ToLower();
				}

				switch (languageCode)
				{
					case "es":
						language = LanguageTypes.Spanish;
                        Logging.Logging.Track("Culture", "Language", "Spanish");
						break;
					default:
						language = LanguageTypes.English;
                        Logging.Logging.Track("Culture", "Language", "English");
						break;
				}

				SessionSettings.Instance.Language = language;
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Culture:SetLanguage");
			}
		}
	}
}