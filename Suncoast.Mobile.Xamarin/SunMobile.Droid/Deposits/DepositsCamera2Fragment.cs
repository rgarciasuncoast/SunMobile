using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
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
using SunMobile.Shared.Methods;
using SunMobile.Shared.Permissions;

namespace SunMobile.Droid.Deposits
{
	// This requires API 21
	#pragma warning disable XA0001 // Find issues with Android API usage
	public class DepositsCamera2Fragment : BaseFragment
	{
		private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
		private SurfaceTextureListener _surfaceTextureListener;
		private ImageView imageShutter;

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

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);

			var view = inflater.Inflate(Resource.Layout.RemoteDepositsTakePictureView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			_textureView = Activity.FindViewById<AutoFitTextureView>(Resource.Id.textureView);
			_textureView.SurfaceTextureListener = _surfaceTextureListener;
			imageShutter = Activity.FindViewById<ImageView>(Resource.Id.imageShutter);
			imageShutter.Click += (sender, e) => TakePicture();

			_stateListener = new CameraStateListener() { Fragment = this };
			_surfaceTextureListener = new SurfaceTextureListener(this);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
			ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
		}

		public override void OnResume()
		{
			base.OnResume();

			OpenCamera();
		}

		public override void OnPause()
		{
			base.OnPause();

			if (_cameraDevice != null)
			{
				_cameraDevice.Close();
				_cameraDevice = null;
			}
		}

		// Opens a CameraDevice. The result is listened to by 'mStateListener'.
		private async void OpenCamera()
		{
			Activity activity = Activity;

			if (activity == null || activity.IsFinishing || isOpeningCamera)
			{
				return;
			}

			isOpeningCamera = true;

			var manager = (CameraManager)activity.GetSystemService(Context.CameraService);

			try
			{
				var isCameraAvailable = await Permissions.GetCameraPermission(Activity);

				if (isCameraAvailable)
				{
					string cameraId = manager.GetCameraIdList()[0];

					// To get a list of available sizes of camera preview, we retrieve an instance of
					// StreamConfigurationMap from CameraCharacteristics
					var characteristics = manager.GetCameraCharacteristics(cameraId);
					var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
					_previewSize = map.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture)))[0];
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
				Logging.Log(ex, "DepositsCamera2Fragment:OpenCamera");
				await AlertMethods.Alert(Activity, "SunMobile", "Cannot access the camera.", "OK");
				//Activity.Finish();
			}
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
							Logging.Log("DepositsCamera2Fragment:Failed to configure camera");
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
				Logging.Log(ex, "DepositsCamera2Fragment:StartPreview");
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
				Logging.Log(ex, "DepositsCamera2Fragment:UpdatePreview");
			}
		}

		private void SetUpCaptureRequestBuilder(CaptureRequest.Builder builder)
		{
			// In this sample, w just let the camera device pick the automatic settings
			builder.Set(CaptureRequest.ControlMode, new Java.Lang.Integer((int)ControlMode.Auto));
		}

		/// <summary>
		/// Configures the necessary transformation to mTextureView.
		/// This method should be called after the camera preciew size is determined in openCamera, and also the size of mTextureView is fixed
		/// </summary>
		private void ConfigureTransform(int viewWidth, int viewHeight)
		{
			Activity activity = Activity;

			if (_textureView == null || _previewSize == null || activity == null)
			{
				return;
			}

			var rotation = activity.WindowManager.DefaultDisplay.Rotation;
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

		private void TakePicture()
		{
			try
			{
				Activity activity = Activity;

				if (activity == null || _cameraDevice == null)
				{
					return;
				}

				var manager = (CameraManager)activity.GetSystemService(Context.CameraService);

				// Pick the best JPEG size that can be captures with this CameraDevice
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
				}

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

				// Output file
				Java.IO.File file = new Java.IO.File(activity.GetExternalFilesDir(null), "pic.jpg");

				// This listener is called when an image is ready in ImageReader 
				var readerListener = new ImageAvailableListener() { File = file };

				// We create a Handler since we want to handle the resulting JPEG in a background thread
				var thread = new HandlerThread("CameraPicture");
				thread.Start();
				var backgroundHandler = new Handler(thread.Looper);
				reader.SetOnImageAvailableListener(readerListener, backgroundHandler);

				// This listener is called when the capture is completed
				// Note that the JPEG data is not available in this listener, but in the ImageAvailableListener we created above
				var captureListener = new CameraCaptureListener() { Fragment = this, File = file };

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
							Logging.Log(ex, "DepositsCamera2Fragment:TakePicture");
						}
					}
				}, backgroundHandler);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsCamera2Fragment:TakePicture");
			}
		}

		#region Supporting classes

		// TextureView.ISurfaceTextureListener handles several lifecycle events on a TextureView
		private class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
		{
			private DepositsCamera2Fragment _fragment;

			public SurfaceTextureListener(DepositsCamera2Fragment fragment)
			{
				_fragment = fragment;
			}

			public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
			{
				_fragment.ConfigureTransform(width, height);
				_fragment.StartPreview();
			}

			public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
			{
				return true;
			}

			public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
			{
				_fragment.ConfigureTransform(width, height);
				_fragment.StartPreview();
			}

			public void OnSurfaceTextureUpdated(SurfaceTexture surface)
			{
			}
		}

		private class CameraStateListener : CameraDevice.StateCallback
		{
			public DepositsCamera2Fragment Fragment;

			public override void OnOpened(CameraDevice camera)
			{
				if (Fragment != null)
				{
					Fragment._cameraDevice = camera;
					Fragment.StartPreview();
					Fragment.isOpeningCamera = false;
				}
			}

			public override void OnDisconnected(CameraDevice camera)
			{
				if (Fragment != null)
				{
					camera.Close();
					Fragment._cameraDevice = null;
					Fragment.isOpeningCamera = false;
				}
			}

			public override void OnError(CameraDevice camera, CameraError error)
			{
				camera.Close();

				if (Fragment != null)
				{
					Fragment._cameraDevice = null;
					Activity activity = Fragment.Activity;
					Fragment.isOpeningCamera = false;

					if (activity != null)
					{
						//activity.Finish();
					}
				}
			}
		}

		private class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
		{
			public Java.IO.File File;

			public void OnImageAvailable(ImageReader reader)
			{
				Image image = null;

				try
				{
					image = reader.AcquireLatestImage();
					Java.Nio.ByteBuffer buffer = image.GetPlanes()[0].Buffer;
					byte[] bytes = new byte[buffer.Capacity()];
					buffer.Get(bytes);
					Save(bytes);
				}
				catch (Exception ex)
				{
					Logging.Log(ex, "DepositsCamera2Fragment:ImageAvailableListener");
				}
				finally
				{
					if (image != null)
					{
						image.Close();
					}
				}
			}

			private void Save(byte[] bytes)
			{
				Java.IO.OutputStream output = null;

				try
				{
					if (File != null)
					{
						output = new Java.IO.FileOutputStream(File);
						output.Write(bytes);
					}
				}
				finally
				{
					if (output != null)
					{
						output.Close();
					}
				}
			}
		}

		private class CameraCaptureListener : CameraCaptureSession.CaptureCallback
		{
			public DepositsCamera2Fragment Fragment;
			public Java.IO.File File;

			public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
			{
				if (Fragment != null && File != null)
				{
					Activity activity = Fragment.Activity;

					if (activity != null)
					{
						Toast.MakeText(activity, "Saved: " + File, ToastLength.Short).Show();
						Fragment.StartPreview();
					}
				}
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