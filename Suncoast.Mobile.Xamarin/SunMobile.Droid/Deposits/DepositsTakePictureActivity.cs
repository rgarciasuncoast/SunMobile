using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Graphics;
using Android.Content.Res;
using Android.Media;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Permissions;
using Android.Content.PM;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;
using Android.Content;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Culture;

namespace SunMobile.Droid.Deposits
{
	// This requires API 21 (Lollipop)
    #pragma warning disable XA0001 // Find issues with Android API usage
	[Activity(Label = "Graphics/Camera Preview2", ScreenOrientation = ScreenOrientation.Portrait)]
	public class DepositsTakePictureActivity : Activity, TextureView.ISurfaceTextureListener
	{
		private Button btnRetake;
		private Button btnUse;
		private ImageView imagePreview;
		private ImageView imageShutter;
		private ImageView imageFlash;
		private TextView txtHelpText;
		private CameraOverlayView _overlayView;
		public bool FlashOn { get; set; }
		public Bitmap BitmapPreview { get; set; }
		const int MAX_IMAGE_WIDTH = 1920;
		const int MAX_IMAGE_HEIGHT = 1080;
		const int MIN_IMAGE_WIDTH = 1280;
		const int MIN_IMAGE_HEIGHT = 720;
		private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();

		// The size of the camera preview
		private Size _previewSize;
		// True if the app is currently trying to open the camera
		private bool isOpeningCamera;
		// CameraDevice.StateListener is called when a CameraDevice changes its state
		private CameraStateListener _stateListener;
		// An AutoFitTextureView for camera preview
		private AutoFitTextureView _textureView;
		// A CameraRequest.Builder for camera preview
		private CaptureRequest.Builder _previewBuilder;
		// A CameraCaptureSession for camera preview
		private CameraCaptureSession _previewSession;
		// A reference to the opened CameraDevice
		private CameraDevice _cameraDevice;
		private ImageAvailableListener _imageAvailableListener;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			RequestWindowFeature(WindowFeatures.NoTitle);
			Window.AddFlags(WindowManagerFlags.Fullscreen);

			RequestedOrientation = ScreenOrientation.Portrait;

			SetupView();
		}

		public void SetupView()
		{
			SetContentView(Resource.Layout.RemoteDepositsTakePictureView);

			string helpText = Intent.GetStringExtra("helptext");

			_textureView = FindViewById<AutoFitTextureView>(Resource.Id.textureView);
			_textureView.SurfaceTextureListener = this;

			_overlayView = (CameraOverlayView)FindViewById(Resource.Id.overlayView);
			_overlayView.HelpText = string.Empty;

			txtHelpText = FindViewById<TextView>(Resource.Id.txtHelpText);
			txtHelpText.Text = helpText;

			btnRetake = FindViewById<Button>(Resource.Id.btnRetake);
			btnRetake.Click += RetakePicture;
			btnRetake.Visibility = ViewStates.Gone;

			btnUse = FindViewById<Button>(Resource.Id.btnUse);
			btnUse.Click += UsePicture;
			btnUse.Visibility = ViewStates.Gone;

			imageFlash = FindViewById<ImageView>(Resource.Id.imageFlash);
			imageFlash.Click += SetFlash;

			imageShutter = FindViewById<ImageView>(Resource.Id.imageShutter);
			imageShutter.Click += TakePicture;

			imagePreview = FindViewById<ImageView>(Resource.Id.imagePreview);
			imagePreview.Visibility = ViewStates.Gone;

			_stateListener = new CameraStateListener { Activity = this };
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);			
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
				Logging.Log(ex, "DepositsTakePictureActivity:SetCultureConfiguration");
			}
		}

		protected override void OnStart()
		{
			base.OnStart();

			OpenCamera();
		}

		protected override void OnStop()
		{
			base.OnStop();

			if (_cameraDevice != null)
			{
				_cameraDevice.Close();
				_cameraDevice = null;
			}
		}

        protected override void OnResume()
        {
            base.OnResume();

            SetCultureConfiguration();
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (BitmapPreview != null)
			{
				BitmapPreview.Recycle();
				BitmapPreview = null;
			}
		}

		private async void OpenCamera()
		{
			if (IsFinishing || isOpeningCamera)
			{
				return;
			}

			isOpeningCamera = true;

			var manager = (CameraManager)GetSystemService(CameraService);

			try
			{
				var isCameraAvailable = await Permissions.GetCameraPermission(this);

				if (isCameraAvailable)
				{
					string cameraId = manager.GetCameraIdList()[0];

					// To get a list of available sizes of camera preview, we retrieve an instance of
					// StreamConfigurationMap from CameraCharacteristics
					var displayResolution = GeneralUtilities.GetScreenResolution(this);
					var characteristics = manager.GetCameraCharacteristics(cameraId);
					var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
					_previewSize = GetOptimalPreviewSize(map.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture))), displayResolution.Height, displayResolution.Width);
					Android.Content.Res.Orientation orientation = Resources.Configuration.Orientation;

					if (orientation == Android.Content.Res.Orientation.Landscape)
					{
						_textureView.SetAspectRatio(_previewSize.Width, _previewSize.Height);
					}
					else
					{
						_textureView.SetAspectRatio(_previewSize.Height, _previewSize.Width);
					}

					// We are opening the camera with a listener. When it is ready, OnOpened of mStateListener is called.
					manager.OpenCamera(cameraId, _stateListener, null);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:OpenCamera");
				Finish();
			}
		}

		private Size GetOptimalPreviewSize(Size[] sizes, int width, int height)
		{
			var previewSize = new Size(0, 0);
			double SIZE_RATIO_16BY9 = 16d / 9d;

			var displaySizeRatio = (double)((double)width / (double)height);

			var test = string.Empty;

			test = $"Display Resolution: {width}, {height}\nPreview Sizes\nSize Ratio: {displaySizeRatio}\n";

			foreach (var size in sizes)
			{
				test += $"{size.Width}, {size.Height}\n";

				var sizeRatio = (double)((double)size.Width / (double)size.Height);

				if (sizeRatio == displaySizeRatio && size.Width >= MIN_IMAGE_WIDTH && size.Height >= MIN_IMAGE_HEIGHT)// && size.Width <= width && size.Height <= height)
				{
					if (previewSize.Width == 0)
					{
						previewSize = size;
					}
					else
					{
						if (size.Width < previewSize.Width)
						{
							previewSize = size;
						}
					}
				}
			}

			if (previewSize.Width == 0)
			{
				test = $"Display Resolution: {width}, {height}\nPreview Sizes\nSize Ratio: {displaySizeRatio}\n";

				foreach (var size in sizes)
				{
					test += $"{size.Width}, {size.Height}\n";

					var sizeRatio = (double)((double)size.Width / (double)size.Height);

					if (sizeRatio == SIZE_RATIO_16BY9 && size.Width >= MIN_IMAGE_WIDTH && size.Height >= MIN_IMAGE_HEIGHT)
					{
						if (previewSize.Width == 0)
						{
							previewSize = size;
						}
						else
						{
							if (size.Width < previewSize.Width)
							{
								previewSize = size;
							}
						}
					}
				}
			}

			if (previewSize.Width == 0)
			{
				previewSize = new Size(MIN_IMAGE_WIDTH, MIN_IMAGE_HEIGHT);
			}

			test += $"Selected size: {previewSize.Width}, {previewSize.Height}";

			//await AlertMethods.Alert(this, "Testing", test, "OK");

			return previewSize;
		}

		private void StartPreview()
		{
			if (_cameraDevice == null || !_textureView.IsAvailable || _previewSize == null)
			{
				return;
			}

			try
			{
				var texture = _textureView.SurfaceTexture;

				// We configure the size of the default buffer to be the size of the camera preview we want
				texture.SetDefaultBufferSize(_previewSize.Width, _previewSize.Height);

				// This is the output Surface we need to start the preview
				var surface = new Surface(texture);

				// We set up a CaptureRequest.Builder with the output Surface
				_previewBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
				_previewBuilder.AddTarget(surface);

				// Here, we create a CameraCaptureSession for camera preview.
				_cameraDevice.CreateCaptureSession(new List<Surface>() { surface },
					new CameraCaptureStateListener()
					{
						OnConfigureFailedAction = (CameraCaptureSession session) =>
						{
							Logging.Log("DepositsTakePictureActivity:Failed to configure camera");
						},
						OnConfiguredAction = (CameraCaptureSession session) =>
						{
							_previewSession = session;
							UpdatePreview();
						}
					},
					null);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:StartPreview");
			}
		}

		private void UpdatePreview()
		{
			if (_cameraDevice == null)
			{
				return;
			}

			try
			{
				// The camera preview can be run in a background thread. This is a Handler for the camere preview
				SetUpCaptureRequestBuilder(_previewBuilder);
				var thread = new HandlerThread("CameraPreview");
				thread.Start();
				var backgroundHandler = new Handler(thread.Looper);

				// Finally, we start displaying the camera preview
				_previewSession.SetRepeatingRequest(_previewBuilder.Build(), null, backgroundHandler);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:UpdatePreview");
			}
		}

		private void SetUpCaptureRequestBuilder(CaptureRequest.Builder builder)
		{
			// In this sample, we just let the camera device pick the automatic settings
			builder.Set(CaptureRequest.ControlMode, new Java.Lang.Integer((int)ControlMode.Auto));
			builder.Set(CaptureRequest.ControlAfMode, new Java.Lang.Integer((int)ControlAFMode.ContinuousPicture));
			// Set the flash
			builder.Set(CaptureRequest.FlashMode, new Java.Lang.Integer(FlashOn ? (int)FlashMode.Torch : (int)FlashMode.Off));
		}

		private void ConfigureTransform(int viewWidth, int viewHeight)
		{
			try
			{
				if (_textureView == null || _previewSize == null)
				{
					return;
				}

				var rotation = WindowManager.DefaultDisplay.Rotation;
				var matrix = new Matrix();
				var viewRect = new RectF(0, 0, viewWidth, viewHeight);
				var bufferRect = new RectF(0, 0, _previewSize.Width, _previewSize.Height);
				float centerX = viewRect.CenterX();
				float centerY = viewRect.CenterY();

				if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
				{
					bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
					matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
					float scale = Math.Max((float)viewHeight / _previewSize.Height, (float)viewWidth / _previewSize.Width);
					matrix.PostScale(scale, scale, centerX, centerY);
					matrix.PostRotate(90 * ((int)rotation - 2), centerX, centerY);
				}

				_textureView.SetTransform(matrix);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:ConfigureTransform");
			}
		}

		private void TakePicture(object sender, EventArgs e)
		{
			try
			{
				Activity activity = this;

				if (activity == null || _cameraDevice == null)
				{
					return;
				}

				/*
                // Pick the best JPEG size that can be captures with this CameraDevice
                var manager = (CameraManager)activity.GetSystemService(Context.CameraService);
                var characteristics = manager.GetCameraCharacteristics(_cameraDevice.Id);
                Size[] jpegSizes = null;

                if (characteristics != null)
                {
                    jpegSizes = ((StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap)).GetOutputSizes((int)ImageFormatType.Jpeg);
                }

                int width = 640;
                int height = 480;
                
                if (jpegSizes != null && jpegSizes.Length > 0)
                {
                    width = jpegSizes[0].Width;
                    height = jpegSizes[0].Height;

                    foreach (var jpegSize in jpegSizes)
                    {
                        if (jpegSize.Width >= _previewSize.Width && jpegSize.Height >= _previewSize.Height && jpegSize.Width < width)
                        //if (jpegSize.Width <= MAX_IMAGE_WIDTH && jpegSize.Height <= MAX_IMAGE_HEIGHT && jpegSize.Width > width)
                        {
                            width = jpegSize.Width;
                            height = jpegSize.Height;
                        }
                    }
                }
                */

				int width = _previewSize.Width;
				int height = _previewSize.Height;

				// We use an ImageReader to get a JPEG from CameraDevice
				// Here, we create a new ImageReader and prepare its Surface as an output from the camera
				var reader = ImageReader.NewInstance(width, height, ImageFormatType.Jpeg, 1);
				var outputSurfaces = new List<Surface>(2);
				outputSurfaces.Add(reader.Surface);
				outputSurfaces.Add(new Surface(_textureView.SurfaceTexture));

				var captureBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
				captureBuilder.AddTarget(reader.Surface);
				SetUpCaptureRequestBuilder(captureBuilder);

				// Orientation
				SurfaceOrientation rotation = activity.WindowManager.DefaultDisplay.Rotation;
				captureBuilder.Set(CaptureRequest.JpegOrientation, new Java.Lang.Integer(ORIENTATIONS.Get((int)rotation)));

				// This listener is called when an image is ready in ImageReader 
				_imageAvailableListener = new ImageAvailableListener { Activity = this };

				// We create a Handler since we want to handle the resulting JPEG in a background thread
				var thread = new HandlerThread("CameraPicture");
				thread.Start();
				var backgroundHandler = new Handler(thread.Looper);
				reader.SetOnImageAvailableListener(_imageAvailableListener, backgroundHandler);

				// This listener is called when the capture is completed
				// Note that the JPEG data is not available in this listener, but in the ImageAvailableListener we created above
				var captureListener = new CameraCaptureListener { Activity = this };

				_cameraDevice.CreateCaptureSession(outputSurfaces, new CameraCaptureStateListener()
				{
					OnConfiguredAction = (CameraCaptureSession session) =>
					{
						try
						{
							session.Capture(captureBuilder.Build(), captureListener, backgroundHandler);
						}
						catch (Exception ex)
						{
							Logging.Log(ex, "DepositsTakePictureActivity:TakePicture");
						}
					}
				}, backgroundHandler);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:TakePicture");
			}
		}

		private void RetakePicture(object sender, EventArgs e)
		{
			ShowCamera(true);
			StartPreview();
		}

		private void UsePicture(object sender, EventArgs e)
		{
			try
			{
				if (BitmapPreview != null)
				{
					var displaymetrics = new DisplayMetrics();
					WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
					var screenWidth = displaymetrics.WidthPixels;
					var screenHeight = displaymetrics.HeightPixels;

					var widthRatio = (double)BitmapPreview.Width / (double)screenWidth;
					var heightRatio = (double)BitmapPreview.Height / (double)screenHeight;

					int left = (int)((double)_overlayView.LeftTop.X * widthRatio);
					int top = (int)((double)_overlayView.LeftTop.Y * heightRatio);
					int right = (int)((double)_overlayView.RightBottom.X * widthRatio);
					int bottom = (int)((double)_overlayView.RightBottom.Y * heightRatio);

					var tempBitmap = Images.CropBitmap(BitmapPreview, new Rect(left, top, right, bottom));
					var checkImage = Images.RotateBitmap(tempBitmap, -90);

					tempBitmap.Dispose();
					tempBitmap = null;

					BitmapPreview.Dispose();
					BitmapPreview = null;

					// Saving to IsolatedStorage because it is to big to pass back with the intent.
					string imageBase64String = Images.ConvertBitmapToBase64String(checkImage);
					RetainedSettings.Instance.CheckImage = imageBase64String;

					var intent = new Intent();
					intent.PutExtra("ClassName", "DepositsTakePictureActivity");
					SetResult(Result.Ok, intent);
					Finish();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:UsePicture.  Unable to crop picture.");
			}
		}

		private void SetFlash(object sender, EventArgs e)
		{
			try
			{
				FlashOn = !FlashOn;
				imageFlash.SetImageResource(FlashOn ? Resource.Drawable.ic_action_flash_on : Resource.Drawable.ic_action_flash_off);
				UpdatePreview();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsTakePictureActivity:SetFlash");
			}
		}

		private void ShowCamera(bool bShow)
		{
			if (!bShow)
			{
				_textureView.Visibility = ViewStates.Gone;

				if (BitmapPreview != null)
				{
					imagePreview.SetImageBitmap(BitmapPreview);
				}

				imagePreview.Visibility = ViewStates.Visible;
				imageShutter.Visibility = ViewStates.Gone;
				imageFlash.Visibility = ViewStates.Gone;
				txtHelpText.Visibility = ViewStates.Gone;
				btnUse.Visibility = ViewStates.Visible;
				btnRetake.Visibility = ViewStates.Visible;

				if (_cameraDevice != null)
				{
					_cameraDevice.Close();
					_cameraDevice = null;
				}
			}
			else
			{
				_textureView.Visibility = ViewStates.Visible;
				imagePreview.Visibility = ViewStates.Gone;
				imageShutter.Visibility = ViewStates.Visible;
				imageFlash.Visibility = ViewStates.Visible;
				txtHelpText.Visibility = ViewStates.Visible;
				btnUse.Visibility = ViewStates.Gone;
				btnRetake.Visibility = ViewStates.Gone;

				OpenCamera();
			}
		}

		#region ISurfaceTextureListener

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
		{
			ConfigureTransform(width, height);
			StartPreview();
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			return true;
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
			ConfigureTransform(width, height);
			StartPreview();
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{
		}

		#endregion

		#region Supporting classes

		private class CameraStateListener : CameraDevice.StateCallback
		{
			public DepositsTakePictureActivity Activity;

			public override void OnOpened(CameraDevice camera)
			{
				if (Activity != null)
				{
					Activity._cameraDevice = camera;
					Activity.StartPreview();
					Activity.isOpeningCamera = false;
				}
			}

			public override void OnDisconnected(CameraDevice camera)
			{
				if (Activity != null)
				{
					camera.Close();
					Activity._cameraDevice = null;
					Activity.isOpeningCamera = false;
				}
			}

			public override void OnError(CameraDevice camera, CameraError error)
			{
				camera.Close();

				if (Activity != null)
				{
					Activity._cameraDevice = null;
					Activity.isOpeningCamera = false;

					Activity.Finish();
				}
			}
		}

		private class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
		{
			public DepositsTakePictureActivity Activity;

			public void OnImageAvailable(ImageReader reader)
			{
				Image image = null;

				try
				{
					image = reader.AcquireLatestImage();
					Java.Nio.ByteBuffer buffer = image.GetPlanes()[0].Buffer;
					byte[] bytes = new byte[buffer.Capacity()];
					buffer.Get(bytes);

					var options = new BitmapFactory.Options();
					options.InPurgeable = true; // InPurgeable is used to free up memory while required
					var sampleSize = Images.CalculateInSampleSize(Activity._previewSize.Width, Activity._previewSize.Height, MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT);
					options.InSampleSize = sampleSize;

					Activity.BitmapPreview = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
					bytes = null;

					if (!Build.Model.ToLower().Contains("nexus 5x"))
					{
						if (Activity.BitmapPreview.Width > Activity.BitmapPreview.Height)
						{
							Activity.BitmapPreview = Images.RotateBitmap(Activity.BitmapPreview, 90);
						}
						else
						{
							Activity.BitmapPreview = Images.RotateBitmap(Activity.BitmapPreview, 0);
						}
					}
					else
					{
						if (Activity.BitmapPreview.Width > Activity.BitmapPreview.Height)
						{
							Activity.BitmapPreview = Images.RotateBitmap(Activity.BitmapPreview, -90);
						}
						else
						{
							Activity.BitmapPreview = Images.RotateBitmap(Activity.BitmapPreview, 180);
						}
					}

					Activity.RunOnUiThread(() => Activity.ShowCamera(false));
				}
				catch (Exception ex)
				{
					Logging.Log(ex, "DepositsTakePictureActivity:ImageAvailableListener");
				}
				finally
				{
					if (image != null)
					{
						image.Close();
					}
				}
			}
		}

		private class CameraCaptureListener : CameraCaptureSession.CaptureCallback
		{
			public DepositsTakePictureActivity Activity;

			public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
			{
			}
		}

		// This CameraCaptureSession.StateListener uses Action delegates to allow the methods to be defined inline, as they are defined more than once
		private class CameraCaptureStateListener : CameraCaptureSession.StateCallback
		{
			public Action<CameraCaptureSession> OnConfigureFailedAction;
			public Action<CameraCaptureSession> OnConfiguredAction;

			public override void OnConfigureFailed(CameraCaptureSession session)
			{
				if (OnConfigureFailedAction != null)
				{
					OnConfigureFailedAction(session);
				}
			}

			public override void OnConfigured(CameraCaptureSession session)
			{
				if (OnConfiguredAction != null)
				{
					OnConfiguredAction(session);
				}
			}
		}
		#endregion
	}
#pragma warning restore XA0001 // Find issues with Android API usage
}