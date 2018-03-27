using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Settings;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Utilities.General;

#if __ANDROID__
using Plugin.CurrentActivity;
#endif

namespace SunMobile.Shared.Utilities.Settings
{
	public class RetainedSettings
	{
		private static RetainedSettings _instance;	

		public static RetainedSettings Instance 
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new RetainedSettings();
				}

				return _instance;
			}
		}

		#pragma warning disable IDE0001 // Simplify Names

		public string MemberId 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("MemberId", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("MemberId", value);
			}
		}

		public string RememberMemberId 
		{
			get 
			{
				var returnValue = CrossSettings.Current.GetValueOrDefault("RememberMemberId", "false");
				return returnValue;
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("RememberMemberId", value);
			}
		}

		public string IsTouchIdRegistered 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("istouchidregistered", "false");
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("istouchidregistered", value);
			}
		}

		public string TouchIdRegisteredMemberId 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("touchidregisteredmemberid", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("touchidregisteredmemberid", value);
			}
		}

		public string TouchIdRegisteredPin 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("touchidregisteredpin", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("touchidregisteredpin", value);
			}
		}

		public string LastAuthenticatedMemberId 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("LastAuthenticatedMember", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("LastAuthenticatedMember", value);
			}
		}

		public AnyMemberInfo AnyMemberInfo 
		{
			get 
			{
				AnyMemberInfo returnValue;

				try
				{
					var json = CrossSettings.Current.GetValueOrDefault("anymemberinfo", string.Empty);
					returnValue = JsonConvert.DeserializeObject<AnyMemberInfo>(json);
				}
				catch
				{
					returnValue = new AnyMemberInfo();
				}

				return returnValue;
			}
			set 
			{
				var json = JsonConvert.SerializeObject(value);
				CrossSettings.Current.AddOrUpdateValue("anymemberinfo", json);
			}
		}

		public string CheckImage 
		{
			get 
			{
				return CrossSettings.Current.GetValueOrDefault("checkimage", string.Empty);
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("checkimage", value);
			}
		}

		public PayloadMessage Payload 
		{
			get 
			{
				var returnValue = new PayloadMessage();

				try
				{
					var json = CrossSettings.Current.GetValueOrDefault("payload", string.Empty);
					returnValue = JsonConvert.DeserializeObject<PayloadMessage>(json);
				}
				catch
				{
					returnValue = new PayloadMessage();
				}

				return returnValue;
			}
			set 
			{
				var json = JsonConvert.SerializeObject(value);
				CrossSettings.Current.AddOrUpdateValue("payload", json);
			}
		}

		public bool Version351Initialized 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("version319initialized", false);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("version319initialized", value);
			}
		}

		public bool ShowFeedback 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("showfeedback", true);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("showfeedback", value);
			}
		}

		public CultureConfiguration Culture
		{
			get 
			{
				CultureConfiguration culture = null;

				try
				{
					var json = CrossSettings.Current.GetValueOrDefault("cultureconfiguration", string.Empty);
					culture = JsonConvert.DeserializeObject<CultureConfiguration>(json);
				}
				catch {}

				return culture;
			}
			set 
			{
				var json = JsonConvert.SerializeObject(value);
				CrossSettings.Current.AddOrUpdateValue("cultureconfiguration", json);
			}
		}

        public string DateInAuthConfigOptionsLastChanged
		{
			get
			{
                var returnValue = CrossSettings.Current.GetValueOrDefault("DateInAuthConfigOptionsLastChanged", string.Empty);
				return returnValue;
			}
			set
			{
				CrossSettings.Current.AddOrUpdateValue("DateInAuthConfigOptionsLastChanged", value);
			}
		}

        public string InAuthConfigOptions
		{
			get
			{
                var returnValue = CrossSettings.Current.GetValueOrDefault("InAuthConfigOptions", string.Empty);
				return returnValue;
			}
			set
			{
				CrossSettings.Current.AddOrUpdateValue("InAuthConfigOptions", value);
			}
		}

		public bool ShowOnboardingFirstTime
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("showonboarding", true);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("showonboarding", value);
			}
		}

		public bool ShowOnboardingUpdate
		{
			get 
			{
				try
				{
					#if __IOS__
					return CrossSettings.Current.GetValueOrDefault($"showonboardingupdate{GeneralUtilities.GetAppShortVersionNumber()}", true);
					#endif
					#if __ANDROID__
					return CrossSettings.Current.GetValueOrDefault($"showonboardingupdate{GeneralUtilities.GetAppShortVersionNumber(CrossCurrentActivity.Current.Activity)}", true);
					#endif
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				#if __IOS__
				CrossSettings.Current.AddOrUpdateValue($"showonboardingupdate{GeneralUtilities.GetAppShortVersionNumber()}", value);
				#endif
				#if __ANDROID__
				CrossSettings.Current.AddOrUpdateValue($"showonboardingupdate{GeneralUtilities.GetAppShortVersionNumber(CrossCurrentActivity.Current.Activity)}", value);
				#endif
			}
		}

		public List<Alert> Alerts 
		{
			get 
			{
				List<Alert> returnValue;

				try
				{
					var json = CrossSettings.Current.GetValueOrDefault("alerts", string.Empty);
					returnValue = JsonConvert.DeserializeObject<List<Alert>>(json);
				}
				catch
				{
					returnValue = new List<Alert>();
				}

				return returnValue;
			}
			set 
			{
				var json = JsonConvert.SerializeObject(value);
				CrossSettings.Current.AddOrUpdateValue("alerts", json);
			}
		}

		public bool SettingsCopiedFromOldToNew 
		{
			get 
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("settingscopiedfromoldtonew", false);
				}
				catch
				{
					return false;
				}
			}
			set 
			{
				CrossSettings.Current.AddOrUpdateValue("settingscopiedfromoldtonew", value);
			}
		}

		#pragma warning restore IDE0001 // Simplify Names

		public void DeleteAlert(string id)
		{
			var alerts = Alerts;

			foreach (var alert in alerts)
			{
				if (alert.Id == id)
				{
					alerts.Remove(alert);
					break;
				}
			}

			Alerts = alerts;
		}

		public void AddAlert(Alert alert)
		{
			var alerts = Alerts;
			alerts.Insert(0, alert);
			Alerts = alerts;
		}

		public void ClearAlerts()
		{
			Alerts = new List<Alert>();
		}

		public void MarkAlertAsRead(string id)
		{
			var alerts = Alerts;
			var alert = alerts.Single(x => x.Id == id);
			var alertIndex = alerts.IndexOf(alert);
			alert.IsRead = true;
			alerts[alertIndex] = alert;
			Alerts = alerts;		
		}
	}
}