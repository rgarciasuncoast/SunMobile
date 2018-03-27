using System;
using CoreGraphics;
using Foundation;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Images;
using UIKit;

namespace SunMobile.iOS.Deposits
{
	public partial class CameraViewController : BaseViewController, ICultureConfigurationProvider
	{
		private CaptureSessionManager _captureSessionManager;
		private CameraOverlayView _cameraOverlayView;
		public event Action<UIImage> PictureTakenDelegate = delegate{};
		public string HelpText { get; set; }
		private string _currentOrientation;

		public CameraViewController(IntPtr handle) : base(handle)
		{			
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();	

			NavigationController.NavigationBarHidden = true;
			UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.None);

			_currentOrientation = UIDevice.CurrentDevice.ValueForKey(new NSString("orientation")).ToString();

			// Force orientation to portrait
			UIDevice.CurrentDevice.SetValueForKey(new NSNumber(1), new NSString("orientation"));  // Portrait

			switch (_currentOrientation)
			{
				case "1": // Portrait
					lblHelpText.Transform = CGAffineTransform.MakeRotation((float)(Math.PI / 2));
					break;
				case "2": // Portrait upside down
					lblHelpText.Transform = CGAffineTransform.MakeRotation((float)(Math.PI / 2));
					break;
				case "3": // Landscape left
					lblHelpText.Transform = CGAffineTransform.MakeRotation((float)(Math.PI / 2));
					break;
				case "4": // Landscape right
					lblHelpText.Transform = CGAffineTransform.MakeRotation((float)(Math.PI * 1.5));
					break;
			}		

			lblHelpText.Text = HelpText;
		
			btnShutter.Clicked += (sender, e) => TakePicture();
            btnShutter.AccessibilityLabel = "Take Picture";
			btnRetake.Clicked += (sender, e) => RetakePicture();
			btnUse.Clicked += (sender, e) => UsePicture();
			btnCancel.Clicked += (sender, e) => Done();

			toolbar.SetItems(new [] 
			{
				btnCancel, spacerLeft, btnShutter, spacerRight
			}, false);		
		}

		public override void SetCultureConfiguration()
		{
			CultureTextProvider.SetMobileResourceText(btnUse, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "610b507b-f5c2-44f0-965b-73ffc1e4029d");
			CultureTextProvider.SetMobileResourceText(btnCancel, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "18aed375-2518-4118-843a-0c9007d70043");
			CultureTextProvider.SetMobileResourceText(btnRetake, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "e8b3bbd4-a25e-44c8-86eb-f90364daa99f");
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(false);

			SetupCaptureSession();

			_cameraOverlayView = new CameraOverlayView(new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height - toolbar.Bounds.Height));
			View.AddSubview(_cameraOverlayView);

			_captureSessionManager.CaptureSession.StartRunning();
		}

		public override bool ShouldAutorotate()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations() 
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		private void TakePicture()
		{
			_captureSessionManager.CaptureStillImage(_cameraOverlayView.ParentFrame, _cameraOverlayView.PreviewRect);

			toolbar.SetItems(new [] 
			{
				btnRetake, spacerLeft, btnUse
			}, false);
		}

		private void RetakePicture()
		{
			_captureSessionManager.CaptureSession.StartRunning();

			toolbar.SetItems(new [] 
			{
				btnCancel, spacerLeft, btnShutter, spacerRight
			}, false);
		}

		private void UsePicture()
		{
			var image = imageView.Image;

			if (image != null)
			{	
				if (_currentOrientation == "4") // Landscape right
				{
					// We need to rotate 180 degrees
					image = Images.ScaleAndRotateImage(image, UIImageOrientation.Down);
				}

				image = Images.ScaleImage(image, 1600, true);

				PictureTakenDelegate(image);
			}

			Done();
		}

		private void Done()
		{
			try 
			{
				if (_captureSessionManager != null && _captureSessionManager.CaptureSession.Running)
				{
					_captureSessionManager.CaptureSession.StopRunning();
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "CameraViewController:Done:StopRunning");
			}

			NavigationController.NavigationBarHidden = false;
			UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.None);
			NavigationController.PopViewController(true);

			UIDevice.CurrentDevice.SetValueForKey(new NSString(_currentOrientation), new NSString("orientation"));
		}

		private void SetupCaptureSession()
		{
			try 
			{
				_captureSessionManager = new CaptureSessionManager();

				_captureSessionManager.StillImageTakenDelegate  += image =>
				{
					imageView.Image = image;
					_captureSessionManager.CaptureSession.StopRunning();
				};

				_captureSessionManager.AddVideoInput(true);
				_captureSessionManager.AddStillImageOutput();
				_captureSessionManager.AddVideoPreviewLayer();

				var layerRect = imageView.Layer.Bounds;
				_captureSessionManager.PreviewLayer.Bounds = layerRect;
				_captureSessionManager.PreviewLayer.Position = new CGPoint(imageView.Bounds.GetMidX(), imageView.Bounds.GetMidY());
				imageView.Layer.AddSublayer(_captureSessionManager.PreviewLayer);
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "CameraViewController:SetupCaptureSession");
			}
		}
	}
}