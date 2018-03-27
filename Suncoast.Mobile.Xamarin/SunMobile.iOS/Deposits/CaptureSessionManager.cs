using System;
using System.Drawing;
using AVFoundation;
using CoreGraphics;
using Foundation;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using UIKit;

namespace SunMobile.iOS.Deposits
{
	public class CaptureSessionManager
	{
		private AVCaptureStillImageOutput _stillImageOutput;
		private UIImage _stillImage;

		public AVCaptureSession CaptureSession { get; set; }
		public AVCaptureVideoPreviewLayer PreviewLayer { get; set; }
		public event Action<UIImage> StillImageTakenDelegate = delegate{};

		public CaptureSessionManager()
		{
			CaptureSession = new AVCaptureSession();
		}

		public void AddVideoPreviewLayer()
		{
		    PreviewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
		    {
		        VideoGravity = AVLayerVideoGravity.ResizeAspectFill
		    };
		}

		public void AddVideoInput(bool isBackCamera)
		{
			AVCaptureDevice camera = null;
		    AVCaptureDeviceInput cameraDeviceInput = null;

			foreach (var device in AVCaptureDevice.Devices)
			{
				if (device.HasMediaType(AVMediaType.Video))
				{
					if ((device.Position == AVCaptureDevicePosition.Back && isBackCamera) ||
						(device.Position == AVCaptureDevicePosition.Front && !isBackCamera))
					{
						camera = device;
						break;
					}
				}
			}

		    if (camera != null)
		    {
		        cameraDeviceInput = AVCaptureDeviceInput.FromDevice(camera);

		        if (cameraDeviceInput != null)
		        {
		            if (CaptureSession.CanAddInput(cameraDeviceInput))
		            {
		                CaptureSession.AddInput(cameraDeviceInput);
		            }
		        }
		    }

		    if (camera == null || cameraDeviceInput == null)
		    {
		        AlertMethods.Alert(AppDelegate.MenuNavigationController.View, "SunMobile", "Unable to open camera.", "OK");
		    }
		}

		public void AddStillImageOutput()
		{
		    _stillImageOutput = new AVCaptureStillImageOutput
		    {
		        OutputSettings = NSDictionary.FromObjectAndKey(new NSString("AVVideoCodecKey"), new NSString("AVVideoCodecJPEG"))
		    };

		    CaptureSession.AddOutput(_stillImageOutput);
		}

		public void CaptureStillImage(CGRect parentFrame, RectangleF rect)
		{
			try 
			{
				AVCaptureConnection videoConnection = null;

				foreach (var connection in _stillImageOutput.Connections)
				{
					foreach (var port in connection.InputPorts)
					{
						if (port.MediaType == AVMediaType.Video)
						{
							videoConnection = connection;
							break;
						}
					}

					if (videoConnection != null)
					{
						break;
					}
				}

				if (videoConnection != null)
				{
					_stillImageOutput.CaptureStillImageAsynchronously(videoConnection, (imageDataSampleBuffer, error) =>
					{
						NSData imageData = AVCaptureStillImageOutput.JpegStillToNSData(imageDataSampleBuffer);
						_stillImage = new UIImage(imageData);

						// Crop Image
						var xRatio = _stillImage.Size.Height / parentFrame.Width;
						var yRatio = _stillImage.Size.Width / parentFrame.Height;

						var resizedRect = new RectangleF((float)(rect.X * xRatio), (float)(rect.Y * yRatio), (float)(rect.Width * xRatio), (float)(rect.Height * yRatio));

						var croppedImage = Images.CropImage(_stillImage, resizedRect);

						StillImageTakenDelegate(croppedImage);

						// If we need metadata
						/*
						var image = CIImage.FromData(imageData);
						var metaData = image.Properties.Dictionary.MutableCopy() as NSMutableDictionary;
						//  metaData ["Orientation"]  // this is the metadata you are looking for
						*/
					});
				}
			}
			catch(Exception ex)
			{				
				Logging.Log(ex, "CaptureSessionManager:CaptureStillImage");
			}
		}
	}
}