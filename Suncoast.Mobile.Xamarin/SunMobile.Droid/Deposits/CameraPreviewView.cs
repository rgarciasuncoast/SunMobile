using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid.Deposits
{
	[Activity(Label = "CameraPreviewView", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	#pragma warning disable CS0618 // Type or member is obsolete
	public class CameraPreviewView : SurfaceView, ISurfaceHolderCallback, Android.Hardware.Camera.IAutoFocusCallback
	#pragma warning restore CS0618 // Type or member is obsolete
	{
		#pragma warning disable CS0618 // Type or member is obsolete
		public event Action<bool> SurfaceIsCreated = delegate { };
		public enum FlashModes { Off, On, Auto }
		public enum FocusModes { Auto, Manual }
		public Android.Hardware.Camera.Size CameraPreviewSize;
		public Android.Hardware.Camera.Size CameraPictureSize;
		private ISurfaceHolder _holder;
		private Android.Hardware.Camera _camera;
		private FlashModes _currentFlashMode;
		private FocusModes _currentFocusMode = FocusModes.Auto;
		private Context _context;
		private const int MAX_IMAGE_WIDTH = 1280;
		private const int JPEG_QUALITY_PERCENT = 75;
		private Android.Hardware.Camera.IPictureCallback _pictureCallback;
		#pragma warning restore CS0618 // Type or member is obsolete

		public CameraPreviewView(Context context) : base(context)
		{
			_context = context;
			Init();
		}

		public CameraPreviewView(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
		{
			_context = context;
			Init();
		}

		private void Init()
		{
			_holder = Holder;
			_holder.AddCallback(this);
			#pragma warning disable 618
			_holder.SetType(SurfaceType.PushBuffers);
			#pragma warning restore 618
		}

		#pragma warning disable CS0618 // Type or member is obsolete
		public void SurfaceCreated(ISurfaceHolder holder)
		{
			try
			{
				_camera = Android.Hardware.Camera.Open();
				SetInitialCameraParameters();
				_camera.SetPreviewDisplay(holder);

				SurfaceIsCreated(true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CameraPreviewView:SurfaceCreated");
				ShutdownCamera();
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		#pragma warning disable CS0618 // Type or member is obsolete
		public async void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
		{
			int lastMax = 0;

			try
			{
				var parameters = _camera.GetParameters();
				// SetPreviewSize the max supported.
				IList<Android.Hardware.Camera.Size> previewSizes = parameters.SupportedPreviewSizes;

				foreach (var cs in previewSizes)
				{
					if (cs.Width > lastMax && cs.Width <= width)
					{
						CameraPreviewSize = cs;
						lastMax = cs.Width;
					}
				}

				try
				{
					parameters.SetPreviewSize(CameraPreviewSize.Width, CameraPreviewSize.Height);
					_camera.SetParameters(parameters);
				}
				catch (Exception ex)
				{
					Logging.Log(ex, "CameraPreviewView:SurfaceChanged", string.Format("SetPreviewSize - width {0}, height - {1}", width, height));
				}

				// SetPictureSize to the max supported without going over our max (1600)
				lastMax = 0;
				IList<Android.Hardware.Camera.Size> cameraSizes = parameters.SupportedPictureSizes;

				foreach (var cs in cameraSizes)
				{
					if (cs.Width > lastMax && cs.Width <= MAX_IMAGE_WIDTH)
					{
						CameraPictureSize = cs;
						lastMax = cs.Width;
					}
				}

				try
				{
					parameters.SetPictureSize(CameraPictureSize.Width, CameraPictureSize.Height);
				}
				catch (Exception ex)
				{
					Logging.Log(ex, "CameraPreviewView:SurfaceChanged", string.Format("SetPictureSize - width {0}, height - {1}", CameraPictureSize.Width, CameraPictureSize.Height));
				}

				parameters.JpegQuality = JPEG_QUALITY_PERCENT;

				_camera.SetParameters(parameters);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CameraPreviewView:SurfaceChanged Test 1");
			}

			try
			{
				_camera.StartPreview();
			}
			catch (Exception)
			{
				await AlertMethods.Alert(_context, "Deposits", "Error initializing camera.", "OK");
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			ShutdownCamera();
		}

		#pragma warning disable CS0618 // Type or member is obsolete
		private void SetInitialCameraParameters()
		{
			try
			{
				if (DoesCameraSupportAutoFocus())
				{
					var parameters = _camera.GetParameters();
					parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;
					_camera.SetParameters(parameters);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CameraPreviewView:SetCameraParameters");
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		#pragma warning disable CS0618 // Type or member is obsolete
		public void ToggleFlash()
		{
			try
			{
				FlashModes newFlashMode;

				var parameters = _camera.GetParameters();

				switch (_currentFlashMode)
				{
					case FlashModes.Off:
						newFlashMode = FlashModes.On;
						parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOn;
						break;
					case FlashModes.On:
						newFlashMode = FlashModes.Auto;
						parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeAuto;
						break;
					case FlashModes.Auto:
						newFlashMode = FlashModes.Off;
						parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
						break;
					default:
						newFlashMode = FlashModes.Off;
						parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
						break;
				}

				_camera.SetParameters(parameters);
				_currentFlashMode = newFlashMode;
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CameraPreviewView:ToggleFlash");
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		#pragma warning disable CS0618 // Type or member is obsolete
		public string GetFlashMode()
		{
			string returnValue = FlashModes.Off.ToString();

			try
			{
				var parameters = _camera.GetParameters();

				if (parameters?.FlashMode != null)
				{
					if (parameters.FlashMode == Android.Hardware.Camera.Parameters.FlashModeOn)
					{
						returnValue = FlashModes.On.ToString();
					}
					else if (parameters.FlashMode == Android.Hardware.Camera.Parameters.FlashModeAuto)
					{
						returnValue = FlashModes.Auto.ToString();
					}
				}
			}
			catch (Exception)
			{
				Logging.Log("CameraPreviewView:GetFlashMode");
			}

			return returnValue;
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		#pragma warning disable CS0618 // Type or member is obsolete
		public void ToggleFocus()
		{
			try
			{
				FocusModes newFocusMode;

				var parameters = _camera.GetParameters();

				switch (_currentFocusMode)
				{
					case FocusModes.Manual:
						newFocusMode = FocusModes.Auto;
						parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;
						break;
					default:
						newFocusMode = FocusModes.Manual;
						parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeMacro;
						break;
				}

				_camera.SetParameters(parameters);
				_currentFocusMode = newFocusMode;
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CameraPreviewView:ToggleFocus");
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		public string GetFocusMode()
		{
			return _currentFocusMode.ToString();
		}

		public void CameraAutoFocus()
		{
			if (_currentFocusMode == FocusModes.Auto)
			{
				AutoFocus(this);
			}
		}

		#pragma warning disable CS0618 // Type or member is obsolete
		private void AutoFocus(Android.Hardware.Camera.IAutoFocusCallback autoFocusCallback)
		{
			if (DoesCameraSupportAutoFocus())
			{
				var parameters = _camera.GetParameters();
				parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeAuto;
				_camera.SetParameters(parameters);

				_camera.AutoFocus(autoFocusCallback);
				Logging.Log(string.Format("Camera autofocus. {0}", DateTime.Now));
			}
		}

		#pragma warning disable CS0618 // Type or member is obsolete
		public void OnAutoFocus(bool success, Android.Hardware.Camera camera)
		{
			if (_pictureCallback != null)
			{
				_camera.TakePicture(null, null, _pictureCallback);
				_pictureCallback = null;
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		public void ShutdownCamera()
		{
			if (_camera != null)
			{
				_camera.SetPreviewCallback(null);
				_camera.StopPreview();
				_camera.Release();
				_camera = null;
			}
		}

		#pragma warning disable CS0618 // Type or member is obsolete
		public bool DoesCameraSupportFlash()
		{
			bool returnValue = false;

			try
			{
				var parameters = _camera.GetParameters();
				var flashModes = parameters.SupportedFlashModes;

				if (flashModes != null && flashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeOn))
				{
					returnValue = true;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DoesCameraSupportAutoFocus:DoesCameraSupportFlash");
			}

			return returnValue;
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		#pragma warning disable CS0618 // Type or member is obsolete
		public bool DoesCameraSupportAutoFocus()
		{
			bool returnValue = false;

			try
			{
				var parameters = _camera.GetParameters();
				var focusModes = parameters.SupportedFocusModes;

				if (focusModes != null && focusModes.Contains(Android.Hardware.Camera.Parameters.FocusModeAuto) && focusModes.Contains(Android.Hardware.Camera.Parameters.FocusModeContinuousPicture))
				{
					returnValue = true;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DoesCameraSupportAutoFocus:DoesCameraSupportAutoFocus");
			}

			return returnValue;
		}
		#pragma warning restore CS0618 // Type or member is obsolete

		#pragma warning disable CS0618 // Type or member is obsolete
		public void TakePicture(Android.Hardware.Camera.IPictureCallback callBack)
		{
			try
			{
				if (callBack != null)
				{
					if (_currentFocusMode == FocusModes.Auto && DoesCameraSupportAutoFocus())
					{
						_pictureCallback = callBack;
						AutoFocus(this);
					}
					else
					{
						_camera.TakePicture(null, null, callBack);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CameraPreviewView:TakePicture");
			}
		}
		#pragma warning restore CS0618 // Type or member is obsolete
	}
}