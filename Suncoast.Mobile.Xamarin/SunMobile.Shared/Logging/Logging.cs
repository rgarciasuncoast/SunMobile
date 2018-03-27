using System;
using System.Collections.Generic;

namespace SunMobile.Shared.Logging
{
    public enum LogLevels
    {
        Info,
        Warning,
        Error,
		Critital
    }

    public static class Logging
    {
        public static void Log(string message)
        {
			#if DEBUG
			Console.WriteLine(message);
			#endif
        }

		public static void Log(string title, string message)
		{
			#if DEBUG
			Console.WriteLine(string.Format("{0}\n{1}", title, message));
			#endif
		} 

		public static void Log(Exception ex, string messageKey, string messageValue = "", LogLevels loglevel = LogLevels.Warning, bool reportToInsights = true)
		{
			try 
			{
				#if DEBUG
				Console.WriteLine(ex.Message);
				#endif

				if (reportToInsights)
				{
					if (string.IsNullOrEmpty(messageValue))
					{
						messageValue = messageKey;
						messageKey = "Method";
					}

                    // Visual Studio App Center
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Exception Caught", new Dictionary<string, string>{ { messageKey, messageValue } } );					
				}
			}			
			catch {}
		}

		public static void Identify(string memberId)
		{
			try 
			{
				var traits = new Dictionary<string, string>();

				try 
				{
					var deviceId = Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;

					if (deviceId?.Length >= 4)
					{
						deviceId = deviceId.Substring(deviceId.Length - 5, 4);
					}

					//traits.Add(Insights.Traits.Description, deviceId);
					//traits.Add(Insights.Traits.Address, GeneralUtilities.GetDeviceIpAddress());
				}				
				catch {}

                // Xamarin Insights
				//Insights.Identify(memberId, traits);
			}			
			catch {}
		}

		public static void Track(string trackIndentifier, string key = "", string value = "")
		{
			try
			{
				if (string.IsNullOrEmpty(key))
				{
					#if __IOS__
					key = "Operating System";
					value = "iOS";
					#endif 

					#if __ANDROID__
					key = "Operating System";
					value = "Android";
					#endif
				}

                // Visual Studio App Center
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent(trackIndentifier, new Dictionary<string, string> { { key, value }});				
			}			
			catch {}
		}

        public static void Track(string trackIndentifier, Dictionary<string, string> keyValues)
		{
			try
			{
                if (keyValues == null)
                {
                    keyValues = new Dictionary<string, string>();
                }

                string key;
                string value;
				
                #if __IOS__
                key = "Operating System";
                value = "iOS";
                #endif

                #if __ANDROID__
				key = "Operating System";
				value = "Android";
                #endif

                keyValues.Add(key, value);

                // Visual Studio App Center
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent(trackIndentifier, keyValues);
			}			
			catch {}
		}
    }
}