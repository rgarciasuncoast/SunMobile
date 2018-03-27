using System;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using SunBlock.DataTransferObjects.OnBase;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.Droid.Documents;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Sharing;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid
{
	public class CheckImageFragment : BaseFragment
	{
		private WebView webView;
		private Button btnFront;
		private Button btnBack;
		private ImageView btnShare;
		private ImageView btnPrint;
		private Bitmap _frontBitmap;
		private Bitmap _backBitmap;
		private bool _isFront;
		public CheckImagesRequest ImageRequest { get; set; }
		private string _tempFullFileName;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.CheckImageView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutBoolean("IsFront", _isFront);

			base.OnSaveInstanceState(outState);
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("78CCD94D-20A2-459A-851E-EC341D943816", "A0F6C137-6611-4AAD-AA84-56814E2525A4", "Check Image"));
                CultureTextProvider.SetMobileResourceText(btnBack, "78CCD94D-20A2-459A-851E-EC341D943816", "036ED2F6-13E8-4FD2-996E-678A74A7BFFF", "Check Back");
                CultureTextProvider.SetMobileResourceText(btnFront, "78CCD94D-20A2-459A-851E-EC341D943816", "8C412080-4045-4FD7-939C-275F41023E55", "Check Front");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "CheckImageFragment:SetCultureConfiguration");
            }
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			btnFront = Activity.FindViewById<Button>(Resource.Id.btnFront);
			btnFront.Click += SelectImage;
			btnFront.Enabled = false;

			btnBack = Activity.FindViewById<Button>(Resource.Id.btnBack);
			btnBack.Click += SelectImage;

			btnShare = Activity.FindViewById<ImageView>(Resource.Id.btnShare);
			btnShare.Click += (sender, e) =>
			{
				Share();
			};
			btnPrint = Activity.FindViewById<ImageView>(Resource.Id.btnPrint);
			btnPrint.Click += (sender, e) =>
			{
				Print();
			};

			webView = Activity.FindViewById<WebView>(Resource.Id.webView);
			webView.SetWebViewClient(new CustomWebViewClient());
			webView.Settings.JavaScriptEnabled = true;
			webView.Settings.AllowFileAccess = true;
			webView.Settings.AllowUniversalAccessFromFileURLs = true;
			webView.Settings.BuiltInZoomControls = true;
			webView.Settings.DisplayZoomControls = false;
			webView.Settings.SetPluginState(WebSettings.PluginState.On);
			webView.Settings.LoadWithOverviewMode = true;
			webView.Settings.UseWideViewPort = true;

			if (savedInstanceState != null)
			{
				_isFront = savedInstanceState.GetBoolean("IsFront");
			}

			GetCheckImages();			
		}	

		private void SelectImage(object sender, EventArgs e)
		{
			if (sender == btnFront && _frontBitmap != null)
			{
				string base64Image = Images.ConvertBitmapToBase64String(_frontBitmap, false);
				string dataURL = "data:image/png;base64," + base64Image;
				webView.LoadUrl(dataURL);
				webView.Settings.LoadWithOverviewMode = true;
				webView.Settings.UseWideViewPort = true;

				btnFront.Enabled = false;
				btnBack.Enabled = true;
				_isFront = true;
			}

			if (sender == btnBack && _backBitmap != null)
			{
				string base64Image = Images.ConvertBitmapToBase64String(_backBitmap, false);
				string dataURL = "data:image/png;base64," + base64Image;
				webView.LoadUrl(dataURL);
				btnBack.Enabled = false;
				btnFront.Enabled = true;
				_isFront = false;
			}
		}

		private void Print()
		{
			Sharing.Print(Activity, webView);
		}

		private void Share()
		{
			byte[] fileBytes;

			if (_isFront)
			{
				fileBytes = Images.ConvertBitmapToByteArray(_frontBitmap, false);
			}
			else
			{
				fileBytes = Images.ConvertBitmapToByteArray(_backBitmap, false);
			}

			_tempFullFileName = Sharing.Share(Activity, "check.jpg", fileBytes);
		}

		public override void OnStop()
		{
			IsolatedStorage.DeleteFile(_tempFullFileName);

			base.OnStop();
		}

		private async void GetCheckImages()
		{
			try
			{
				if (_frontBitmap == null)
				{
					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("78CCD94D-20A2-459A-851E-EC341D943816", "9DF5BC1A-7FC3-4DAC-8C64-0B501F79FE95", "Loading check images..."));

					byte[] frontImage = null;
					byte[] backImage = null;

					if (ImageRequest.CheckNumber != 0)
					{
						var onBaseMethods = new OnBaseMethods();
						var response = await onBaseMethods.GetCheckImages(ImageRequest, this);

						if (response != null && response.CheckImageDocuments != null && response.CheckImageDocuments.Count >= 1)
						{
							frontImage = response.CheckImageDocuments[0].FrontImage;
							backImage = response.CheckImageDocuments[0].BackImage;
						}
					}
					else
					{
						var rdtRequest = new RemoteDepositTransactionRequest();
						int transactionId;
						int.TryParse(ImageRequest.RemoteDepositTraceNumber, out transactionId);
						rdtRequest.TransactionId = transactionId;

						var depositMethods = new DepositMethods();
						var rdtResponse = await depositMethods.RetrieveRemoteDepositTransactions(rdtRequest, this);

						if (rdtResponse.RemoteDepositTransactions.Count >= 1)
						{
							frontImage = rdtResponse.RemoteDepositTransactions[0].CheckFront;
							backImage = rdtResponse.RemoteDepositTransactions[0].CheckBack;
						}
					}

					if (frontImage != null)
					{
						_frontBitmap = Images.ConvertByteArrayToBitmap(frontImage);

						if (GeneralUtilities.IsOrientationPortrait(Activity))
						{
							_frontBitmap = Images.RotateBitmap90Degrees(_frontBitmap);
						}

						SelectImage(null, null);
					}

					if (backImage != null)
					{
						_backBitmap = Images.ConvertByteArrayToBitmap(backImage);

						if (GeneralUtilities.IsOrientationPortrait(Activity))
						{
							_backBitmap = Images.RotateBitmap90Degrees(_backBitmap);
						}
					}

					_isFront = true;

					HideActivityIndicator();
				}

				SelectImage(_isFront ? btnFront : btnBack, null);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "CheckImageFragment:GetCheckImages");
			}
		}
	}
}