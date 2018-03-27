using System;
using UIKit;
using SunMobile.iOS.Common;
using SunMobile.Shared.Methods;
using SunBlock.DataTransferObjects.OnBase;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Sharing;
using SunMobile.Shared.Culture;

namespace SunMobile.iOS.Accounts
{
	public partial class CheckImageViewController : BaseViewController
	{
		public CheckImagesRequest ImageRequest { get; set; }
		private UIImage _frontUIImage;
		private UIImage _backUIImage;
		private byte[] _frontImageBytes;
		private byte[] _backImageBytes;
		private bool _isFront;

		public CheckImageViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			segmentControl.ValueChanged += SelectImage;
			btnShare.Clicked += (sender, e) => Share();
			btnPrint.Clicked += (sender, e) => Print();

			scrollView.MaximumZoomScale = 3f;
			scrollView.MinimumZoomScale = .1f;
			scrollView.ViewForZoomingInScrollView += sv => imageView;

			_isFront = true;

			GetCheckImages();			
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("78CCD94D-20A2-459A-851E-EC341D943816", "A0F6C137-6611-4AAD-AA84-56814E2525A4", "Check Image");
			segmentControl.SetTitle(CultureTextProvider.GetMobileResourceText("78CCD94D-20A2-459A-851E-EC341D943816", "036ED2F6-13E8-4FD2-996E-678A74A7BFFF", "Check Back"), 1);
			segmentControl.SetTitle(CultureTextProvider.GetMobileResourceText("78CCD94D-20A2-459A-851E-EC341D943816", "8C412080-4045-4FD7-939C-275F41023E55", "Check Front"), 0);
		}

		private void SelectImage(object sender, EventArgs e)
		{
			var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;

			if (selectedSegmentId == 0 && _frontUIImage != null)
			{
				imageView.Image = _frontUIImage;
				_isFront = true;
			}

			if (selectedSegmentId == 1 && _backUIImage != null)
			{
				imageView.Image = _backUIImage;
				_isFront = false;
			}
		}

		private async void GetCheckImages()
		{
			try
			{
				if (ImageRequest.CheckNumber != 0)
				{
					ShowActivityIndicator();

					var onBaseMethods = new OnBaseMethods();

					HideActivityIndicator();

					var response = await onBaseMethods.GetCheckImages(ImageRequest, null);

					if (response != null && response.CheckImageDocuments != null && response.CheckImageDocuments.Count >= 1)
					{
						_frontImageBytes = response.CheckImageDocuments[0].FrontImage;
						_backImageBytes = response.CheckImageDocuments[0].BackImage;
					}
				}
				else
				{
					var rdcRequest = new RemoteDepositTransactionRequest();
					int traceNumber = 0;
					int.TryParse(ImageRequest.RemoteDepositTraceNumber, out traceNumber);

					if (traceNumber > 0)
					{
						rdcRequest.TransactionId = traceNumber;

						ShowActivityIndicator();

						var depositMethods = new DepositMethods();
						var rdtResponse = await depositMethods.RetrieveRemoteDepositTransactions(rdcRequest, null);

						HideActivityIndicator();

						if (rdtResponse.RemoteDepositTransactions.Count >= 1)
						{
							_frontImageBytes = rdtResponse.RemoteDepositTransactions[0].CheckFront;
							_backImageBytes = rdtResponse.RemoteDepositTransactions[0].CheckBack;
						}
					}
				}

				if (_frontImageBytes != null)
				{
					_frontUIImage = Images.ConvertByteArrayToUIImage(_frontImageBytes);

					if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
					{
						_frontUIImage = Images.ScaleAndRotateImage(_frontUIImage, UIImageOrientation.Left);
					}

					imageView.Image = _frontUIImage;
				}

				if (_backImageBytes != null)
				{
					_backUIImage = Images.ConvertByteArrayToUIImage(_backImageBytes);

					if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
					{
						_backUIImage = Images.ScaleAndRotateImage(_backUIImage, UIImageOrientation.Left);
					}
				}

				if (imageView.Image != null)
				{
					scrollView.ContentSize = imageView.Image.Size;
				}		
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CheckImageViewController:GetCheckImages");
			}
		}

		private void Share()
		{
			Sharing.Share(this, _isFront ? _frontImageBytes : _backImageBytes, btnShare);
		}

		private void Print()
		{
			Sharing.Print(_isFront ? _frontUIImage : _backUIImage);
		}
	}
}