using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using SunBlock.DataTransferObjects.Session;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Main;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS
{
	public static class StartupSettings
	{
		public async static void GetStartupSettings(UIWindow window, UINavigationController navigationController)
		{
			try
			{
				var methods = new AuthenticationMethods();

				var request = new GetStartupSettingsRequest();
				var response = await methods.GetStartupSettings(request, null);

				if (response != null)
				{
					var dict = new Dictionary<string, string>();

					for (int i = 0; i < response.Keys.Count; i++)
					{
						dict.Add(response.Keys[i], response.Values[i]);
					}

					SessionSettings.Instance.GetStartupSettings = dict;

					var popupText = SessionSettings.Instance.GetStartupSettings["PopUp-iOS"];

					if (!string.IsNullOrEmpty(popupText))
					{
						var popupView = new UIView(new CGRect(UIScreen.MainScreen.Bounds.Left, UIScreen.MainScreen.Bounds.Bottom - 200,
							UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));
						popupView.BackgroundColor = AppStyles.MenuBackgroundColorGray;

						var headerLabel = new UILabel(new CGRect(popupView.Bounds.Left, popupView.Bounds.Top,
							popupView.Bounds.Width, 40));
						headerLabel.Text = "  SunMobile Alert";
						headerLabel.TextColor = UIColor.White;
						headerLabel.BackgroundColor = UIColor.Black;
						AppStyles.SetViewBorder(headerLabel, false);
						popupView.AddSubview(headerLabel);

						var btnClose = new UIButton(new CGRect(popupView.Bounds.Right - 30, popupView.Bounds.Top + 10,
							20, 20));
						btnClose.SetImage(UIImage.FromBundle("disclosure-close"), UIControlState.Normal);
						btnClose.TouchUpInside += (sender, e) => popupView.RemoveFromSuperview();
						popupView.AddSubview(btnClose);

						var popupTextView = new UITextView(new CGRect(0, 40,
							popupView.Bounds.Width, popupView.Bounds.Height - 40));
						popupTextView.Text = popupText;
						popupTextView.TextColor = UIColor.White;
						popupTextView.BackgroundColor = UIColor.Clear;
						popupTextView.UserInteractionEnabled = false;
						AppStyles.SetViewBorder(popupTextView, false);
						popupView.AddSubview(popupTextView);

						window.AddSubview(popupView);

						#if DEBUG
						await Task.Delay(5000);
						popupView.RemoveFromSuperview();
						#endif
					}
				}

				if (!ShowOnboarding())
				{
					ShowFeedback(navigationController);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "StartupSettings:GetStartupSettings");
			}
		}

		public static bool ShowOnboarding()
		{
			var show = false;

			try
			{
				if (RetainedSettings.Instance.ShowOnboardingFirstTime || RetainedSettings.Instance.ShowOnboardingUpdate)
				{
					show = true;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "StartupSettings:ShowOnboarding");
			}

			return show;
		}

		private static void ShowFeedback(UINavigationController navigationController)
		{
			try
			{
				if (RetainedSettings.Instance.ShowFeedback)
				{
					var enableFeedback = SessionSettings.Instance.GetStartupSettings["EnableFeedback"];

					if (enableFeedback == "true")
					{
						var feedbackViewController = AppDelegate.StoryBoard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
						feedbackViewController.Completed += (obj) =>
						{
							var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
							navigationController.PushViewController(accountsViewController, true);
						};
						navigationController.PushViewController(feedbackViewController, true);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainActivity:ShowFeedback");
			}
		}
	}
}