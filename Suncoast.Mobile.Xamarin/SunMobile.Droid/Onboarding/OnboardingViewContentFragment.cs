using System;
using System.Net;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.OnBoarding;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Images;

namespace SunMobile.Droid.Onboarding
{
	public class OnboardingViewContentFragment : BaseFragment
	{
		public OnboardingCarouselItem CarouselItem { get; set; }
		public OnboardingViewPagerFragment Parent { get; set; }
		private TextView txtTitle;
		private TextView txtDescription;
		private ImageView imageMain;
		private Button btnSkip;
		private byte[] _fileBytes;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			RetainInstance = true;

			var view = (RelativeLayout)inflater.Inflate(Resource.Layout.OnboardingContentView, container, false);

			SetupView(view);

			return view;
		}

		public void SetupView(View view)
		{
			try
			{
				txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
				txtDescription = view.FindViewById<TextView>(Resource.Id.txtDescription);
				imageMain = view.FindViewById<ImageView>(Resource.Id.imageMain);
				btnSkip = view.FindViewById<Button>(Resource.Id.btnSkip);

				btnSkip.Click += (sender, e) =>
				{
					Parent.Skip();
				};

				if (CarouselItem != null)
				{
					try
					{
						view.SetBackgroundColor(Color.ParseColor(CarouselItem.BackgroundColor));
					}
					catch { }

					txtTitle.Text = CarouselItem.Title;
					txtDescription.Text = CarouselItem.Description;

					if (_fileBytes == null)
					{
						LoadImage();
					}
					else
					{
						var bitmap = Images.ConvertByteArrayToBitmap(_fileBytes);
						imageMain.SetImageBitmap(bitmap);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "OnboardingViewContentFragment:SetupView");
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
				Logging.Log(ex, "OnboardingViewContentFragment:LoadImage");
			}
		}

		private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			try
			{
				_fileBytes = e.Result;
				var bitmap = Images.ConvertByteArrayToBitmap(_fileBytes);
				imageMain.SetImageBitmap(bitmap);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "OnboardingViewContentFragment:DownloadDataCompleted");
			}
		}
	}
}