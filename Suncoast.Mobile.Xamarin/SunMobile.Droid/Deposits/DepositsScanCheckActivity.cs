using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Deposits
{
	[Activity(Label = "Graphics/Camera Preview", ScreenOrientation = ScreenOrientation.Landscape)]
	#pragma warning disable CS0618 // Type or member is obsolete
	public class DepositsScanCheckActivity : Activity, Android.Hardware.Camera.IPictureCallback, ICultureConfigurationProvider
  	#pragma warning restore CS0618 // Type or member is obsolete
	{
		private TableRow rowRetake;
		private Button btnRetake;
		private Button btnUse;
		private ImageView imagePreview;
		private ImageView imageShutter;
		private ImageView imageFlash;
		private ImageView imageFocus;

		private CameraPreviewView _previewView;
		private CameraOverlayView _overlayView;
		private Bitmap _bitmapPreview;
		const int MAX_IMAGE_WIDTH = 1280;
		const int MAX_IMAGE_HEIGHT = 720;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			RequestWindowFeature(WindowFeatures.NoTitle);
			Window.AddFlags(WindowManagerFlags.Fullscreen);

			RequestedOrientation = ScreenOrientation.Landscape;

			SetupView();			
		}

		public void SetCultureConfiguration()
		{
            try
            {
                CultureTextProvider.SetMobileResourceText(btnUse, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "610b507b-f5c2-44f0-965b-73ffc1e4029d");
                CultureTextProvider.SetMobileResourceText(btnRetake, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "e8b3bbd4-a25e-44c8-86eb-f90364daa99f");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositScanCheckActivity:SetCultureConfiguration");
			}
		}

		public void SetupView()
		{
			SetContentView(Resource.Layout.RemoteDepositsScanCheckView);

			rowRetake = FindViewById<TableRow>(Resource.Id.RetakeRow);
			rowRetake.Visibility = ViewStates.Gone;

			btnRetake = FindViewById<Button>(Resource.Id.btnRetake);
			btnRetake.Click += RetakePicture;

			btnUse = FindViewById<Button>(Resource.Id.btnUse);
			btnUse.Click += UsePicture;

			imageShutter = FindViewById<ImageView>(Resource.Id.imageShutter);
			imageShutter.Click += TakePicture;

			imagePreview = FindViewById<ImageView>(Resource.Id.imagePreview);
			imagePreview.Visibility = ViewStates.Gone;

			imageFlash = FindViewById<ImageView>(Resource.Id.imageFlash);
			imageFlash.Visibility = ViewStates.Gone;
			imageFlash.Click += SetFlash;

			imageFocus = FindViewById<ImageView>(Resource.Id.imageFocus);
			imageFocus.Visibility = ViewStates.Gone;
			imageFocus.Click += SetFocus;

			string helpText = Intent.GetStringExtra("helptext");
			_previewView = (CameraPreviewView)FindViewById(Resource.Id.previewView);
			_previewView.SurfaceIsCreated += (bool obj) =>
			{
				CheckForFlashAndFocus();
			};

			_overlayView = (CameraOverlayView)FindViewById(Resource.Id.overlayView);
			_overlayView.HelpText = helpText;
		}

		private void CheckForFlashAndFocus()
		{
			// Unhide flash toggle button if flash modes are available
			if (_previewView?.GetFlashMode() != null && _previewView.DoesCameraSupportFlash())
			{
				imageFlash.Visibility = ViewStates.Visible;
				SetFlashImage(_previewView.GetFlashMode());
			}

			if (_previewView?.GetFocusMode() != null && _previewView.DoesCameraSupportAutoFocus())
			{
				imageFocus.Visibility = ViewStates.Visible;
				SetFocusImage(_previewView.GetFocusMode());
			}
		}

		private void SetFlashImage(string flashMode)
		{
			switch (flashMode)
			{
				case "On":
					imageFlash.SetImageResource(Resource.Drawable.ic_action_flash_on);
					break;
				case "Auto":
					imageFlash.SetImageResource(Resource.Drawable.ic_action_flash_automatic);
					break;
				default:
					imageFlash.SetImageResource(Resource.Drawable.ic_action_flash_off);
					break;
			}
		}

		private void SetFocusImage(string focusMode)
		{
			switch (focusMode)
			{
				case "Auto":
					imageFocus.SetImageResource(Resource.Drawable.ic_action_autofocus_on);
					break;
				default:
					imageFocus.SetImageResource(Resource.Drawable.ic_action_autofocus_off);
					break;
			}
		}

		private void SetFlash(object sender, EventArgs e)
		{
			try
			{
				_previewView.ToggleFlash();
				SetFlashImage(_previewView.GetFlashMode());
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsScanCheckActivity:SetFlash");
			}
		}

		private void SetFocus(object sender, EventArgs e)
		{
			try
			{
				_previewView.ToggleFocus();
				SetFocusImage(_previewView.GetFocusMode());
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsScanCheckActivity:SetFocus");
			}
		}

		private void RetakePicture(object sender, EventArgs e)
		{
			ShowCamera(true);
		}

		private void UsePicture(object sender, EventArgs e)
		{
			// Saving to IsolatedStorage because it is to big to pass back with the intent.
			string imageBase64String = Images.ConvertBitmapToBase64String(_bitmapPreview);
			RetainedSettings.Instance.CheckImage = imageBase64String;

			var intent = new Intent();
			intent.PutExtra("ClassName", "DepositsScanCheckActivity");
			SetResult(Result.Ok, intent);
			Finish();
		}

		private void TakePicture(object sender, EventArgs e)
		{
			_previewView.TakePicture(this);
		}

		#pragma warning disable CS0618 // Type or member is obsolete
		public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
		#pragma warning restore CS0618 // Type or member is obsolete
		{
			Bitmap picture = null;

			try
			{
				var options = new BitmapFactory.Options();
				options.InPurgeable = true; // InPurgeable is used to free up memory while required
				var parameters = camera.GetParameters();
				var size = parameters.PictureSize;
				var sampleSize = Images.CalculateInSampleSize(size.Width, size.Height, MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT);
				options.InSampleSize = sampleSize;
				picture = BitmapFactory.DecodeByteArray(data, 0, data.Length, options);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsScanCheckActivity:OnPictureTaken.  Unable to decode picture.");
			}

			try
			{
				if (picture != null)
				{
					var displaymetrics = new DisplayMetrics();
					WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
					var screenWidth = displaymetrics.WidthPixels;
					var screenHeight = displaymetrics.HeightPixels;

					var widthRatio = (double)picture.Width / (double)screenWidth;
					var heightRatio = (double)picture.Height / (double)screenHeight;

					int left = (int)((double)_overlayView.LeftTop.X * widthRatio);
					int top = (int)((double)_overlayView.LeftTop.Y * heightRatio);
					int right = (int)((double)_overlayView.RightBottom.X * widthRatio);
					int bottom = (int)((double)_overlayView.RightBottom.Y * heightRatio);

					var checkImage = Images.CropBitmap(picture, new Rect(left, top, right, bottom));

                    // Possible fix for white picture on Galaxy S8
					//picture.Dispose();
					//picture = null;

					SavePhoto(checkImage);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsScanCheckActivity:OnPictureTaken.  Unable to crop picture.");
			}

			ShowCamera(false);
		}

		private void SavePhoto(Bitmap bm)
		{
			if (bm != null && bm.Height != 156)
			{
				_bitmapPreview = bm;
			}
			else
			{
				// Get Test Image
				_bitmapPreview = BitmapFactory.DecodeResource(Resources, Resource.Drawable.checkbackground);
			}

			ShowCamera(false);
		}

		private void ShowCamera(bool bShow)
		{
			if (!bShow)
			{
				_previewView.Visibility = ViewStates.Gone;
				_overlayView.Visibility = ViewStates.Gone;

				if (_bitmapPreview != null)
				{
					imagePreview.SetImageBitmap(_bitmapPreview);
				}

				imagePreview.Visibility = ViewStates.Visible;
				imageShutter.Visibility = ViewStates.Gone;
				rowRetake.Visibility = ViewStates.Visible;

				imageFlash.Visibility = ViewStates.Gone;
				imageFocus.Visibility = ViewStates.Gone;
			}
			else
			{
				_previewView.Visibility = ViewStates.Visible;
				_overlayView.Visibility = ViewStates.Visible;

				imagePreview.Visibility = ViewStates.Gone;
				imageShutter.Visibility = ViewStates.Visible;
				rowRetake.Visibility = ViewStates.Gone;

				CheckForFlashAndFocus();
			}
		}

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                SetCultureConfiguration();
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "DepositsScanCheckActivity:OnResume");
            }
        }
	}
}