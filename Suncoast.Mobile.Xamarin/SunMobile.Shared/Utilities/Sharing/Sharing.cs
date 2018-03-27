using System;

#if __IOS__
using System.Collections.Generic;
using System.Drawing;
using Foundation;
using SunMobile.Shared.Utilities.Settings;
using UIKit;
#endif

#if __ANDROID__
using SunMobile.Shared.Utilities.Settings;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Print;
using Android.Support.V4.Content;
using Android.Webkit;
#endif

namespace SunMobile.Shared.Sharing
{
	public static class Sharing
	{
		#if __IOS__
		public static void Print(UIWebView webView)
		{
			try
			{
				var printInfo = UIPrintInfo.PrintInfo;
				printInfo.OutputType = UIPrintInfoOutputType.General;
				printInfo.JobName = "SunMobile";

				var printer = UIPrintInteractionController.SharedPrintController;
				printer.PrintInfo = printInfo;
				printer.PrintFormatter = webView.ViewPrintFormatter;

				var popupPrint = new RectangleF(-400, -200, 500, 500);

				printer.PresentFromRectInView(popupPrint, webView, true, (handler, completed, err) =>
				{
					if (!completed && err != null)
					{
						Logging.Logging.Log("Sharing:Print");
					}
				});
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Sharing:Print");
			}
		}

		public static void Print(UIImage image)
		{
			try
			{
				var printInfo = UIPrintInfo.PrintInfo;
				printInfo.OutputType = UIPrintInfoOutputType.General;
				printInfo.JobName = "SunMobile";

				var printer = UIPrintInteractionController.SharedPrintController;
				printer.PrintInfo = printInfo;
				printer.PrintingItem = image;

				printer.Present(true, (handler, completed, err) =>
				{
					if (!completed && err != null)
					{
						Console.WriteLine("Sharing:Print");
					}
				});
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Sharing:Print");
			}
		}

		public static void Share(UIViewController viewController, byte[] fileBytes, object viewToPresentFrom)
		{
            string tempFileName = string.Empty;

			try
			{
                var items = new List<NSObject>();

				var image = UIImage.LoadFromData(NSData.FromArray(fileBytes));

                if (image != null)
                {
                    items.Add(image);        
                }
                else
                {
                    var fileName = "Suncoast.pdf";
                    tempFileName = IsolatedStorage.SaveBytesToFile(fileName, fileBytes);
                    var data = NSData.FromFile(tempFileName);

                    if (data != null)
                    {
                        items.Add(data);
                    }
                }

				var controller = new UIActivityViewController(items.ToArray(), null);

				if (viewToPresentFrom != null)
				{
					if (viewToPresentFrom is UIBarButtonItem)
					{
						controller.PopoverPresentationController.BarButtonItem = (UIBarButtonItem)viewToPresentFrom;
					}
					else
					{
						controller.PopoverPresentationController.SourceView = (UIView)viewToPresentFrom;
					}
				}
				else
				{
					controller.PopoverPresentationController.SourceView = viewController.View;
				}

				viewController.PresentViewController(controller, false, null);
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Sharing:Share");
			}
            finally
            {
                if (!string.IsNullOrEmpty(tempFileName))
                {
                    IsolatedStorage.DeleteFile(tempFileName);
                }
            }
		}
		#endif

		#if __ANDROID__
		public static void Print(Activity activity, WebView webView)
		{
			try
			{
				// Will only work in Android 4.4 and higher
				var printManager = (PrintManager)activity.BaseContext.GetSystemService(Context.PrintService);
				#pragma warning disable CS0618 // Type or member is obsolete
				var printAdapter = webView.CreatePrintDocumentAdapter();
				#pragma warning restore CS0618 // Type or member is obsolete
				string jobName = "SunMobile";

				printManager.Print(jobName, printAdapter, new PrintAttributes.Builder().Build());
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Sharing.Print");
			}
		}

		public static string Share(Activity activity, string tempFileName, byte[] fileBytes, string mimeType = "image/jpeg")
		{
			try
			{
				var tempFullFileName = string.Empty;
				tempFullFileName = IsolatedStorage.SaveBytesToFile(tempFileName, fileBytes);
				var file = new Java.IO.File(tempFullFileName);
				var contentUri = FileProvider.GetUriForFile(activity.ApplicationContext, "org.suncoast.fileprovider", file);

				var shareIntent = new Intent();
				shareIntent.SetAction(Intent.ActionSend);
				shareIntent.SetType(mimeType);
				shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
				shareIntent.PutExtra(Intent.ExtraStream, contentUri);

				// Grant apps permission to use our shared content
				var resolveInfoList = activity.PackageManager.QueryIntentActivities(shareIntent, PackageInfoFlags.MatchDefaultOnly);

				foreach (var resolveInfo in resolveInfoList)
				{
					string packageName = resolveInfo.ActivityInfo.PackageName;
					activity.GrantUriPermission(packageName, contentUri, ActivityFlags.GrantReadUriPermission);
				}

				activity.StartActivity(Intent.CreateChooser(shareIntent, "Share"));
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Sharing.Share");
			}

			return tempFileName;
		}
		#endif
	}
}