using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;

#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using SunMobile.Droid;
#endif

#if __IOS__
using Foundation;
using MessageUI;
using SunMobile.iOS;
using SunMobile.Shared.Culture;
using UIKit;
#endif

namespace SunMobile.Shared.Utilities.General
{
	public static class GeneralUtilities
	{
		public static string GetDeviceId()
		{
			string returnValue = string.Empty;

			try
			{
				returnValue = Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		public static string GetDeviceIpAddress()
		{
			var returnValue = string.Empty;

			foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
					netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				{
					foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
					{
						if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							returnValue = addrInfo.Address.ToString();
							break;
						}
					}
				}
			}

			return returnValue;
		}

		public static int GetMemberIdAsInt()
		{
			int returnValue;

			int.TryParse(SessionSettings.Instance.UserId, out returnValue);

			return returnValue;
		}

		public static string GetIpAddress()
		{
			string ipAddress = string.Empty;

			try
			{
				foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
				{
					if (address.AddressFamily != AddressFamily.InterNetwork && address.AddressFamily != AddressFamily.InterNetworkV6)
					{
						ipAddress = address.ToString();
						break;
					}
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return ipAddress;
		}


#if __ANDROID__

		public static string GetAppVersion(Activity activity)
		{
			string returnValue = string.Empty;

			try 
			{	
				returnValue = activity.PackageManager.GetPackageInfo(activity.PackageName, 0).VersionName;
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		public static string GetAppShortVersionNumber(Activity activity)
		{
			string returnValue = string.Empty;

			try
			{
				returnValue = activity.PackageManager.GetPackageInfo(activity.PackageName, 0).VersionName;

				if (returnValue.Length == 8)
				{
					returnValue = returnValue.Substring(0, 5);
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		public static void CloseKeyboard(object view)
		{
			try 
			{
				if (view != null)
				{
					var inputManager = (InputMethodManager)((Activity)view).BaseContext.GetSystemService(Context.InputMethodService);
					inputManager.HideSoftInputFromWindow(((Activity)view).CurrentFocus.WindowToken, 0);
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		public static void ShowKeyboard(object view)
		{
			try 
			{
				if (view != null)
				{
					var inputManager = (InputMethodManager)((Activity)view).BaseContext.GetSystemService(Context.InputMethodService);
					inputManager.ShowSoftInput(((Activity)view).CurrentFocus, 0);
				}
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		/*
		public static bool IsOrientationPortrait(Activity context)
		{
			bool returnValue;

			var rotation = context.WindowManager.DefaultDisplay.Rotation;

			if (rotation == Android.Views.SurfaceOrientation.Rotation0 || rotation == Android.Views.SurfaceOrientation.Rotation180)
			{
				returnValue = true;
			}
			else
			{
				returnValue = false;
			}

			return returnValue;
		}
		*/

		public static bool IsOrientationPortrait(Activity activity)
		{
			var display = activity.WindowManager.DefaultDisplay;
			var displayMetrics = new DisplayMetrics();

			display.GetMetrics(displayMetrics);

			int width = displayMetrics.WidthPixels;
			int height = displayMetrics.HeightPixels;

			return width <= height;
		}

		public static async void SendEmail(object view, string recipient, string subject, string body, bool isHtmlBody = false, bool popControllerAfterFinished = false)
		{
			try 
			{
				var email = new Intent(Intent.ActionSend);

				if (!string.IsNullOrEmpty(recipient))
				{
					email.PutExtra(Intent.ExtraCc, new [] { recipient });
				}

				if (!string.IsNullOrEmpty(subject))
				{
					email.PutExtra(Intent.ExtraSubject, subject);
				}			
		
				if (!string.IsNullOrEmpty(body))
				{
					email.PutExtra(Intent.ExtraText, body);
				}

				email.SetType("message/rfc822");

				((Activity)view).StartActivity(email);
			}
			catch
			{
				try
				{
					await AlertMethods.Alert((Activity)view, "SunMobile", "You don't have an email client configured.", "OK");
				}
				// Analysis disable once EmptyGeneralCatchClause
				catch
				{
				}
			}
		}

		public static bool CanMakeCalls(Context view)
		{
			var returnValue = false;

			var telephonyManager = (TelephonyManager)view.GetSystemService(Context.TelephonyService);

			if (telephonyManager != null)
			{
				if (telephonyManager.PhoneType != PhoneType.None)
				{
					returnValue = true;
				}
			}

			return returnValue;
		}

		public static ViewGroup GetActionBar(ViewGroup viewGroup)
		{
            ViewGroup actionbar = null;

			int childCount = viewGroup.ChildCount;

			for (int i = 0; i < childCount; i++)
			{
				View view = viewGroup.GetChildAt(i);

                if (view.Class.Name == "android.support.v7.widget.Toolbar" || view.Class.Name == "android.widget.Toolbar")
                {
                    actionbar = (ViewGroup)view;
                }
                else if (view is ViewGroup)
                {
                    actionbar = GetActionBar((ViewGroup)view);
                }

                if (actionbar != null)
                {
                    break;
                }
			}

            return actionbar;
		}

		public static void DimView(ViewGroup viewGroup, bool dim)
		{
			int childCount = viewGroup.ChildCount;

			for (int i = 0; i < childCount; i++)
			{
				View view = viewGroup.GetChildAt(i);				
				view.Alpha = (!dim) ? 1.0f : 0.75f;

				if (view is ViewGroup)
				{
					DisableView((ViewGroup)view, dim);
				}
			}
		}

		public static void DisableView(ViewGroup viewGroup, bool disabled)
		{
			int childCount = viewGroup.ChildCount;
			for (int i = 0; i < childCount; i++)
			{
				View view = viewGroup.GetChildAt(i);
				view.Enabled = !disabled;
				view.Alpha = (!disabled) ? 1.0f : 0.75f;

				if (view is ViewGroup)
				{
					DisableView((ViewGroup) view, disabled);
				}
			}
		}

		public static float ConvertDpToPixel(Context context, float dp)
		{
			var resources = context.Resources;
			var metrics = resources.DisplayMetrics;
			float px = dp * ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);

			return px;
		}

		public static float ConvertPixelsToDp(Context context, float px)
		{
			var resources = context.Resources;
			var metrics = resources.DisplayMetrics;
			float dp = px / ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);

			return dp;
		}

		public static Size GetScreenResolution(Activity activity)
		{
			var metrics = new DisplayMetrics();
			activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
			var size = new Size(metrics.WidthPixels, metrics.HeightPixels);

			return size;
		}
#endif

		public static bool IsPhone()
		{
			var returnValue = false;

			#if __IOS__

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				returnValue = true;
			}

			#endif

			#if __ANDROID__

			var isTablet = Application.Context.Resources.GetBoolean(Resource.Boolean.isTablet);

			returnValue = !isTablet;

			#endif

			return returnValue;
		}

		public static bool AllowOnlyPortraitOrientation()
		{
			var returnValue = false;

			#if __IOS__
			
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				returnValue = true;
			}

			#endif

			#if __ANDROID__

			var isPortraitOnly = Application.Context.Resources.GetBoolean(Resource.Boolean.portrait_only);

			returnValue = isPortraitOnly;

			#endif

			return returnValue;
		}

		#if __IOS__

		public static string GetAppVersion()
		{
			string returnValue = string.Empty;

			try 
			{
				returnValue = string.Format("Version {0}", NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")]);
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		public static string GetAppVersionNumber()
		{
			string returnValue = string.Empty;

			try
			{
				returnValue = string.Format("{0}", NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")]);
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		public static string GetAppShortVersionNumber()
		{
			string returnValue = string.Empty;

			try
			{
				returnValue = string.Format("{0}", NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")]);
			}
			// Analysis disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		public static void CloseKeyboard(object view)
		{
			try
			{
				if (view != null)
				{
					((UIView)view).EndEditing(true);
				}
				else
				{
					AppDelegate.MenuNavigationController.View.EndEditing(true);
				}
			}
			catch
			{
			}
		}

        public static bool IsIPhoneX()
        {
            var returnValue = false;

            if (IsPhone())
            {
                if (UIScreen.MainScreen.NativeBounds.Height == 2436)
                {
                    returnValue = true;
                }

                /*
                case 1136:
                        print("iPhone 5 or 5S or 5C")
                case 1334:
                        print("iPhone 6/6S/7/8")
                case 2208:
                        print("iPhone 6+/6S+/7+/8+")
                case 2436:
                        print("iPhone X")
                default:
                        print("unknown")
                */
            }

            return returnValue;
        }

		public static async void SendEmail(object view, string recipient, string subject, string body, bool isHtmlBody = false, bool popControllerAfterFinished = false)
		{
			try 
			{
				if (MFMailComposeViewController.CanSendMail)
				{
					var mailController = new MFMailComposeViewController();

					if (!string.IsNullOrEmpty(recipient))
					{
						mailController.SetToRecipients(new [] { recipient });
					}

					if (!string.IsNullOrEmpty(subject))
					{
						mailController.SetSubject(subject);
					}

					if (!string.IsNullOrEmpty(body))
					{
						mailController.SetMessageBody(body, isHtmlBody);
					}

					mailController.Finished += (sender, e) => 
					{
						((UIViewController)view).DismissViewController(true, null);

						if (popControllerAfterFinished)
						{
							((UIViewController)view)?.NavigationController?.PopViewController(true);
						}
					};

					((UIViewController)view).PresentViewController(mailController, true, null);
				}
			}
			catch
			{
				try
				{
					await AlertMethods.Alert(view, "SunMobile", "You don't have an email client configured.", "OK");
				}
				// Analysis disable once EmptyGeneralCatchClause
				catch
				{
				}			
			}
		}
		#endif
	}
}