#if __ANDROID__
using System.Threading.Tasks;
using System;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;
using Plugin.CurrentActivity;
#endif

#if __IOS__
using UIKit;
using LocalAuthentication;
using Foundation;
using System;
#endif

namespace SunMobile.Shared.Biometrics
{
	public static class BiometricUtilities
	{
#if __ANDROID__

        public static async Task<BiometryType> HasBiometricReader()
		{
            var returnValue = BiometryType.None;			

			try
			{
				if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
				{
					if (await CrossFingerprint.Current.GetAvailabilityAsync() == FingerprintAvailability.Available)
					{
                        returnValue = BiometryType.Fingerprint;
                    }
				}				
			}
			catch (Exception ex)
			{
                Logging.Logging.Log(ex, "BiometricUtilities:HasFingerprintReader");
			}

			return returnValue;
		}

		public static async Task<bool> AuthenticateFingerprint(string message, string cancelButtonText, string fallbackButtonText)
		{
			var returnValue = false;

			try
			{
				if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
				{
					var dialogConfig = new AuthenticationRequestConfiguration(message)
					{
						CancelTitle = cancelButtonText,
						FallbackTitle = fallbackButtonText
					};

					CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
					var response = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig);

					if (response != null && response.Authenticated)
					{
						returnValue = true;
					}
				}
				/*
                else
                {
                    if (await Biometrics.Instance.Evaluate(message))
                    {
                        returnValue = true;
                    }
                }
                */
			}
			catch (Exception ex)
			{
                Logging.Logging.Log(ex, "BiometricUtilities:AuthenticateFingerprint");
			}

			return returnValue;
		}

#endif


#if __IOS__
        public static BiometryType HasBiometricReader()
        {
            var returnValue = BiometryType.None;

            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    var laContext = new LAContext();

                    NSError error;

                    if (laContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error))
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                        {
                            var biometryType = laContext.BiometryType;

                            switch (biometryType)
                            {
                                case LABiometryType.TouchId:
                                    returnValue = BiometryType.Fingerprint;
                                    break;
                                case LABiometryType.FaceId:
                                    returnValue = BiometryType.Face;
                                    break;
                                default:
                                    returnValue = BiometryType.None;
                                    break;
                            }
                        }
                        else
                        {
                            returnValue = BiometryType.Fingerprint;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.Logging.Log(ex, "BiometricUtilities:HasBiometricReader");
            }

            return returnValue;
        }
#endif
	}
}