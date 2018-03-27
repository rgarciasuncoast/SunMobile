using System;
using System.Net;
using SunBlock.DataTransferObjects.OnBoarding;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Images;

namespace SunMobile.iOS.Onboarding
{
	public partial class OnboardingContentViewController : BaseViewController
	{
		public OnboardingCarouselItem CarouselItem { get; set; }
		public int PageIndex { get; set; }

		public OnboardingContentViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			try
			{
				NavigationController.SetNavigationBarHidden(true, false);

				if (CarouselItem != null)
				{
					try
					{
						View.BackgroundColor = AppStyles.ColorFromHexString(CarouselItem.BackgroundColor);
					}
					catch { }

					lblTitle.Text = CarouselItem.Title;
					lblDescription.Text = CarouselItem.Description;
					lblDescription.Lines = 0;
					lblDescription.SizeToFit();

					btnSkip.TouchUpInside += (sender, e) =>
					{
						var accountsViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
						AppDelegate.MenuNavigationController.PushViewController(accountsViewController, true);
					};

					LoadImage();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "OnboardingContentViewController:ViewDidLoad");
			}
		}

		private void LoadImage()
		{
			try
			{
				if (!string.IsNullOrEmpty(CarouselItem.OnboardingCarouselImages[0].OnboardingPictureUrl))
				{
					var webClient = new WebClient();
					webClient.DownloadDataCompleted += DownloadDataCompleted;
					webClient.DownloadDataAsync(new Uri(CarouselItem.OnboardingCarouselImages[0].OnboardingPictureUrl));
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "OnboardingContentViewController:LoadImage");
			}
		}

		private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			try
			{
				var fileBytes = e.Result;

				if (fileBytes != null)
				{
					var image = Images.ConvertByteArrayToUIImage(fileBytes);
					imageMain.Image = image;

					if (GeneralUtilities.IsPhone())
					{
						viewMain.Frame = View.Frame;
					}
					else
					{
						viewMain.Frame = new CoreGraphics.CGRect((View.Frame.Height - image.Size.Height) / 2, (View.Frame.Width - image.Size.Width) / 2, image.Size.Width, image.Size.Height);
						viewMain.Center = View.Center;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "OnboardingContentViewController:DownloadDataCompleted");
			}
		}
	}
}