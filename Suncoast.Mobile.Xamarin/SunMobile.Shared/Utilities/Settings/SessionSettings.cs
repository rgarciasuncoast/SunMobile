using System.Collections.Generic;
using System;
using Plugin.Settings;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Culture;
using SunBlock.DataTransferObjects.Mobile;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;

namespace SunMobile.Shared.Utilities.Settings
{
	public class SessionSettings
	{
		private static SessionSettings _instance;

		public static SessionSettings Instance 
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new SessionSettings();
				}

				return _instance;
			}
		}

		#pragma warning disable IDE0001 // Simplify Names

		public bool IsAuthenticated 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("isauthenticated", false);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("isauthenticated", value);
			}
		}

		public bool HasSignedOutOrTimedOut 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("hassignedoutortimedout", false);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("hassignedoutortimedout", value);
			}
		}

		public bool HasBeenLoaded 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("hasbeenloaded", false);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("hasbeenloaded", value);
			}
		}

		public string SunBlockToken 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("sunblocktoken", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("sunblocktoken", value);
			}
		}

		public string UserId 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("userid", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("userid", value);
			}
		}

		public int LastMenuIndex 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("lastmenuindex", 0);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("lastmenuindex", value);
			}
		}

		public bool ShowPasswordReminder 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("showpasswordreminder", false);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("showpasswordreminder", value);
			}
		}

		public List<DateTime> Holidays 
		{
			get 
			{
				List<DateTime> holidays;

				try
				{
					var json = CrossSettings.Current.GetValueOrDefault("holidays", string.Empty);
					holidays = JsonConvert.DeserializeObject<List<DateTime>>(json);
				}
				catch
				{
					holidays = new List<DateTime>();
				}

				return holidays;
			}
			set 
			{
				var json = JsonConvert.SerializeObject(value);
				CrossSettings.Current.AddOrUpdateValue("holidays", json);
			}
		}

		public string DeviceToken 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("devicetoken", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("devicetoken", value);
			}
		}

		public Dictionary<string, string> GetStartupSettings 
		{
			get 
			{
				var dict = new Dictionary<string, string>();

				try
				{
					var json = CrossSettings.Current.GetValueOrDefault("getstartupsettings", string.Empty);
					dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				}
				catch
				{
					dict = new Dictionary<string, string>();
				}

				if (dict == null)
				{
					dict = new Dictionary<string, string>();
				}

				return dict;
			}
			set 
			{
				var json = JsonConvert.SerializeObject(value);
				CrossSettings.Current.AddOrUpdateValue("getstartupsettings", json);
			}
		}

		public LanguageTypes Language 
		{
			get 
			{
				var language = LanguageTypes.English;
				var languageString = CrossSettings.Current.GetValueOrDefault("language", LanguageTypes.English.ToString());

				switch (languageString)
				{
					case "Spanish":
						language = LanguageTypes.Spanish;
						break;
					default:
						language = LanguageTypes.English;
						break;
				}

				return language;
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("language", value.ToString());
			}
		}       

        public MobileStatusResponse<RocketCheckingResponse> CreateRocketCheckingResponse
        {
            get
            {
                MobileStatusResponse<RocketCheckingResponse> rocketCheckingResponse;

                try
                {
                    var json = CrossSettings.Current.GetValueOrDefault("RocketCheckingResponse", string.Empty);
                    rocketCheckingResponse = JsonConvert.DeserializeObject<MobileStatusResponse<RocketCheckingResponse>>(json);
                }
                catch
                {
                    rocketCheckingResponse = new MobileStatusResponse<RocketCheckingResponse>();
                }

                return rocketCheckingResponse;
            }
            set
            {
                var json = JsonConvert.SerializeObject(value);
                CrossSettings.Current.AddOrUpdateValue("RocketCheckingResponse", json);
            }
        }

		public void ClearAll()
		{
			HasBeenLoaded = false;
			HasSignedOutOrTimedOut = false;
			Holidays = new List<DateTime>();
			IsAuthenticated = false;
			LastMenuIndex = 0;
			ShowPasswordReminder = false;
			SunBlockToken = string.Empty;
			UserId = string.Empty;
            CreateRocketCheckingResponse = null;

			/*
			var startupSettings = GetStartupSettings;
			var deviceToken = DeviceToken;

			CrossSettings.Current.Clear();

			GetStartupSettings = startupSettings;
			DeviceToken = deviceToken;
			*/
		}

		#pragma warning restore IDE0001 // Simplify Names
	}
}