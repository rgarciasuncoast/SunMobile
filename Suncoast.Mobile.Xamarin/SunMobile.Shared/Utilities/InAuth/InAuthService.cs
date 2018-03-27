using System;
using System.Text;
using System.Threading.Tasks;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;

#if __IOS__
using Foundation;

#if DEBUG
using InAuth.iOS.NonProtected;
#endif

#if __ADHOC__
using InAuth.iOS.NonProtected;
#endif

#if __APPSTORE__
using InAuth.iOS.Protected;
#endif

#endif

#if __ANDROID__
using System.IO;
using Android.App;
using System.Collections.Generic;
using Com.Inmobile;
using System.Linq;
using Newtonsoft.Json;
#endif

namespace SunMobile.Shared.Utilities.InAuth
{
	public static class InAuthService
	{
#if __IOS__
		private static MME _mmeManager;

		public static MME GetMmeManager()
		{
			try
			{
				if (_mmeManager == null)
				{
					var keys = new NSString(System.IO.File.ReadAllText("server_keys_message_hosted.json", Encoding.UTF8));
					var jsonKeyData = keys.Encode(NSStringEncoding.UTF8);
					_mmeManager = new MME(AppSettings.InAuthAccountId, AppSettings.InAuthApplicationId ?? "", true, jsonKeyData);
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "InAuthService:GetMmeManager");
			}

			return _mmeManager;
		}

		public static async Task<PayloadMessage> GetPayloadForLogin()
		{
			var returnValue = new PayloadMessage();

			bool isRegistered = await RegisterInAuth();

			if (isRegistered)
			{
				var logOptions = await GetInAuthLogConfig();
				returnValue = GetInAuthLogsPayload(logOptions);
			}

			return returnValue;
		}

		public static async Task<bool> RegisterInAuth()
		{
			bool isInitialized = false;

			try
			{
				GetMmeManager();

				if (_mmeManager != null)
				{
					NSError error;

					// Check and see if we are already registered
					var root = _mmeManager.WhiteBoxReadItem(@"root1", out error);
					var malware = _mmeManager.WhiteBoxReadItem(@"malware", out error);

					if (root == null || malware == null)
					{
						// Register with InAuth
						var registrationRequest = _mmeManager.GenerateRegistrationPayload(out error);

						var payloadMessage = new PayloadMessage
						{
							IpAddress = GeneralUtilities.GetIpAddress(),
							Payload = registrationRequest.ToString()
						};

						// Send registration to SunBlock
						var methods = new AuthenticationMethods();
						var registrationResponse = await methods.RegisterInAuth(payloadMessage, null);

						if (registrationResponse != null && registrationResponse.Success)
						{
							// Handle the response from SunBlock
							var bytes = Convert.FromBase64String(registrationResponse.Result);
							var decodedResponse = Encoding.UTF8.GetString(bytes);

							var userInfo = _mmeManager.HandlePayload(decodedResponse, out error);

							if (userInfo != null)
							{
								NSString keyNameString;
								NSData keyNameData;

								keyNameString = (NSString)userInfo[@"root"];
								keyNameData = keyNameString.Encode(NSStringEncoding.UTF8);
								_mmeManager.WhiteBoxCreateItem(keyNameData, @"root1", out error);

								keyNameString = (NSString)userInfo[@"malware"];
								keyNameData = keyNameString.Encode(NSStringEncoding.UTF8);
								_mmeManager.WhiteBoxCreateItem(keyNameData, @"malware", out error);

								isInitialized = true;
							}
						}
					}
					else
					{
						isInitialized = true;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed RegisterInAuth", ex.Message);
			}

			return isInitialized;
		}

		public static async Task<uint> GetInAuthLogConfig()
		{
			uint returnValue = 0;

			try
			{
				GetMmeManager();

				if (_mmeManager != null)
				{
					NSError error;

					var logRegistrationRequest = _mmeManager.GenerateListRequestPayload((ushort)MMEListType.LogConfig, out error);

                    var payloadMessage = new PayloadMessage
                    {
                        IpAddress = GeneralUtilities.GetIpAddress(),
                        Payload = logRegistrationRequest.ToString(),
                        DateLastChanged = RetainedSettings.Instance.DateInAuthConfigOptionsLastChanged
					};

					var methods = new AuthenticationMethods();
					var logResponse = await methods.GetInAuthLogConfig(payloadMessage, null);

					if (logResponse != null && logResponse.Success)
					{
                        if (!string.IsNullOrEmpty(logResponse.DateLastChanged) && 
                            logResponse.DateLastChanged == RetainedSettings.Instance.DateInAuthConfigOptionsLastChanged &&
                            !string.IsNullOrEmpty(RetainedSettings.Instance.InAuthConfigOptions))
                        {
                            int result = 0;
                            int.TryParse(RetainedSettings.Instance.InAuthConfigOptions, out result);

                            if (result != 0)
                            {
                                returnValue = ((NSNumber)result).UInt32Value;
                            }
                        }

                        if (returnValue == 0)
                        {
                            var bytes = Convert.FromBase64String(logResponse.DeviceResponse);
                            var decodedResponse = Encoding.UTF8.GetString(bytes);

                            var userInfo = _mmeManager.HandlePayload(decodedResponse, out error);

                            if (userInfo != null)
                            {
                                var payloadLogSet = userInfo[@"log_config"];

                                if (payloadLogSet != null)
                                {
                                    returnValue = ((NSNumber)payloadLogSet).UInt32Value;
                                    RetainedSettings.Instance.DateInAuthConfigOptionsLastChanged = logResponse.DateLastChanged;
                                    RetainedSettings.Instance.InAuthConfigOptions = returnValue.ToString();
                                }
                            }
                        }
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed GetInAuthLogConfig", ex.Message);
			}

			return returnValue;
		}

		public static PayloadMessage GetInAuthLogsPayload(uint logChoices)
		{
			var returnValue = new PayloadMessage();

			try
			{
				GetMmeManager();

				if (_mmeManager != null)
				{
					NSError error;

					var logResponse = _mmeManager.GenerateLogPayload(logChoices, out error);

					if (logResponse != null)
					{
						returnValue.IpAddress = GeneralUtilities.GetIpAddress();
						returnValue.Payload = logResponse.ToString();

						if (string.IsNullOrEmpty(returnValue.Payload))
						{
							Logging.Logging.Track("InAuth Events", "GenerateLogPayload - Payload is null.");
						}

						RetainedSettings.Instance.Payload = returnValue;
					}
					else
					{
						Logging.Logging.Track("InAuth Events", "GenerateLogPayload - logResponse is null.");
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed GetInAuthLogsPayload", ex.Message);
			}

			return returnValue;
		}
#endif

#if __ANDROID__
		public static void InitMmeManager(Application application, Android.Content.Res.AssetManager assetManager)
		{
			try
			{
				string serverKeysMessage;

				using (StreamReader sr = new StreamReader(assetManager.Open("server_keys_message_hosted.json")))
				{
					serverKeysMessage = sr.ReadToEnd();
				}

				var serverKeysMessageBytes = Encoding.UTF8.GetBytes(serverKeysMessage);

				MMEManager.Instance.Init(application, AppSettings.InAuthAccountId, serverKeysMessageBytes, AppSettings.InAuthApplicationId);
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "InAuthService:InitMmeManager");
			}
		}

		public static async Task<PayloadMessage> GetPayloadForLogin(Application application, Activity activity, Android.Content.Res.AssetManager assetManager, bool forceInAuthUnregister = false)
		{
			var returnValue = new PayloadMessage();

			try
			{
				if (!RetainedSettings.Instance.Version351Initialized)
				{
					forceInAuthUnregister = true;
                    RetainedSettings.Instance.Version351Initialized = true;
				}

				if (forceInAuthUnregister)
				{
					await UnregisterInAuth(application, assetManager);
				}

				bool isRegistered = await RegisterInAuth(application, activity, assetManager);

				if (isRegistered)
				{
					var logOptions = await GetInAuthLogConfig(application, activity, assetManager);
					returnValue = GetInAuthLogsPayload(application, assetManager, logOptions);
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "InAuthService:GetPayloadForLogin");
			}

			if (returnValue?.Payload != null)
			{
				RetainedSettings.Instance.Payload = returnValue;
			}
			else
			{
				RetainedSettings.Instance.Payload = new PayloadMessage { IpAddress = string.Empty, Payload = string.Empty };
			}

			return returnValue;
		}

		public static async Task<bool> RegisterInAuth(Application application, Activity activity, Android.Content.Res.AssetManager assetManager)
		{
			bool isRegistered = false;

			try
			{
				InitMmeManager(application, assetManager);

				if (!MMEManager.Instance.IsRegistered)
				{
					// Register with InAuth
					var registrationRequest = MMEManager.Instance.GenerateRegistrationPayload();

					var payloadMessage = new PayloadMessage
					{
						IpAddress = GeneralUtilities.GetIpAddress(),
						Payload = Encoding.UTF8.GetString(registrationRequest)
					};

					var methods = new AuthenticationMethods();
					var registrationResponse = await methods.RegisterInAuth(payloadMessage, activity);

					if (registrationResponse?.Result != null && registrationResponse.Success)
					{
						var bytes = Convert.FromBase64String(registrationResponse.Result);
						var decodedResponse = Encoding.UTF8.GetString(bytes);

						var userInfo = MMEManager.Instance.HandlePayload(Encoding.UTF8.GetBytes(decodedResponse));

						if (userInfo != null)
						{
							Java.Lang.Object keyNameString;
							byte[] keyNameData;

							if (userInfo.TryGetValue(@"root", out keyNameString))
							{
								keyNameData = Encoding.UTF8.GetBytes(keyNameString.ToString());
								MMEManager.Instance.WhiteBoxCreateItem(@"root", keyNameData);
							}

							if (userInfo.TryGetValue(@"malware", out keyNameString))
							{
								keyNameData = Encoding.UTF8.GetBytes(keyNameString.ToString());
								MMEManager.Instance.WhiteBoxCreateItem(@"malware", keyNameData);
							}

							isRegistered = true;
						}
					}
				}
				else
				{
					isRegistered = true;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed RegisterInAuth", ex.Message);
			}

			return isRegistered;
		}

		#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public static async Task<bool> UnregisterInAuth(Application application, Android.Content.Res.AssetManager assetManager)
		#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			bool isUnRegistered = false;

			try
			{
				InitMmeManager(application, assetManager);

				MMEManager.Instance.UnRegister();
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed UnregisterInAuth", ex.Message);
			}

			return isUnRegistered;
		}

		public static async Task<List<string>> GetInAuthLogConfig(Application application, Activity activity, Android.Content.Res.AssetManager assetManager)
		{
			var returnValue = new List<string>();

			try
			{
				InitMmeManager(application, assetManager);

				var requestSelectionList = new List<string>();
				requestSelectionList.Add(MMEConstants.LogConfig);

				var logRegistrationRequest = MMEManager.Instance.GenerateListRequestPayload(requestSelectionList);

				var payloadMessage = new PayloadMessage
				{
					IpAddress = GeneralUtilities.GetIpAddress(),
					Payload = Encoding.UTF8.GetString(logRegistrationRequest),
                    DateLastChanged = RetainedSettings.Instance.DateInAuthConfigOptionsLastChanged
				};

				var methods = new AuthenticationMethods();
				var logResponse = await methods.GetInAuthLogConfig(payloadMessage, activity);

				if (logResponse != null && logResponse.Success)
				{					
					if (!string.IsNullOrEmpty(logResponse.DateLastChanged) &&
						logResponse.DateLastChanged == RetainedSettings.Instance.DateInAuthConfigOptionsLastChanged &&
						!string.IsNullOrEmpty(RetainedSettings.Instance.InAuthConfigOptions))
					{
                        try
                        {
                            returnValue = JsonConvert.DeserializeObject<List<string>>(RetainedSettings.Instance.InAuthConfigOptions);
                        }
                        catch{}
					}

                    if (returnValue == null)
                    {
                        returnValue = new List<string>();
                    }

                    if (returnValue.Count == 0)
                    {
                        var bytes = Convert.FromBase64String(logResponse.DeviceResponse);
                        var decodedResponse = Encoding.UTF8.GetString(bytes);

                        var userInfo = MMEManager.Instance.HandlePayload(Encoding.UTF8.GetBytes(decodedResponse));

                        if (userInfo != null)
                        {
                            Java.Lang.Object keyNameString;

                            if (userInfo.TryGetValue(@"log_config", out keyNameString))
                            {
                                returnValue = ParseInAuthLogResponse(keyNameString.ToString());
                                RetainedSettings.Instance.DateInAuthConfigOptionsLastChanged = logResponse.DateLastChanged;
                                var json = JsonConvert.SerializeObject(returnValue);
                                RetainedSettings.Instance.InAuthConfigOptions = json;
                            }
                        }
                    }
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed GetInAuthLogConfig", ex.Message);
			}

			return returnValue;
		}

		public static PayloadMessage GetInAuthLogsPayload(Application application, Android.Content.Res.AssetManager assetManager, List<string> logOptions)
		{
			var returnValue = new PayloadMessage();

			try
			{
				InitMmeManager(application, assetManager);

				byte[] logResponse = null;

				if (logOptions != null && logOptions.Count > 0)
				{
					logResponse = MMEManager.Instance.GenerateLogPayload(logOptions);
				}
				else
				{
					logResponse = MMEManager.Instance.GenerateLogPayload();
				}

				if (logResponse != null)
				{
					returnValue.IpAddress = GeneralUtilities.GetIpAddress();
					returnValue.Payload = Encoding.UTF8.GetString(logResponse);

					if (string.IsNullOrEmpty(returnValue.Payload))
					{
						Logging.Logging.Track("InAuth Events", "GenerateLogPayload - Payload is null.");
					}
				}
				else
				{
					Logging.Logging.Track("InAuth Events", "GenerateLogPayload - logResponse is null.");
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed GetInAuthLogsPayload", ex.Message);
			}

			return returnValue;
		}

		public static List<string> ParseInAuthLogResponse(string configString)
		{
			var logList = new List<string>();

			try
			{
				var startIndex = configString.IndexOf("[", StringComparison.Ordinal) + 1;
				var length = configString.IndexOf("]", StringComparison.Ordinal) - startIndex;
				configString = configString.Substring(startIndex, length);

				logList = configString.Split(',').ToList();
			}
			catch (Exception ex)
			{
				Logging.Logging.Track("InAuth Events", "Failed ParseInAuthLogResponse - " + ex.Message);
			}

			return logList;
		}
#endif
	}
}