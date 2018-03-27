using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Culture;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.Droid.Accounts;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Permissions;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;
using Xamarin.Media;

namespace SunMobile.Droid.Deposits
{
	public class DepositsFragment : BaseFragment, ICultureConfigurationProvider
	{
		private TextView lblDepositTo;
		private TextView lblAmount;
		private TextView lblFrontOfCheck;
		private TextView lblBackOfCheck;
		private TextView txtSourceHeaderText;
		private TextView txtSourceHeader2Text;
		private TextView txtSourceItem1Text;
		private TextView txtSourceValue1Text;
		private TextView txtSourceItem2Text;
		private TextView txtSourceValue2Text;

		private TableRow accountRow; 
		private EditText txtAmount;
		private TextView txtAmountLimit;
		private TableRow rowImageFront;
		private ImageView imageFront;
		private TableRow rowImageBack;
		private ImageView imageBack;
		private Button btnSubmit;
		
		private string _frontCheckImageBase64String;
		private string _backCheckImageBase64String;
		private Bitmap _frontImage;
		private Bitmap _backImage;
        private Account _account;				
		private GetMemberRemoteDepositsInfoResponse _memberRemoteDepositsInfo;


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.RemoteDepositsView, null);
			RetainInstance = true;

			return view;
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits"));
                CultureTextProvider.SetMobileResourceText(lblDepositTo, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "ab612024-2fba-4a3e-9150-a3a6a2c9ce74");
                CultureTextProvider.SetMobileResourceText(lblAmount, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "bcea972a-2cfa-4778-946d-d5389f3df813");
                CultureTextProvider.SetMobileResourceText(txtAmount, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "e31ad74c-c279-4971-bda4-53950950d563");
                CultureTextProvider.SetMobileResourceText(lblFrontOfCheck, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "0b673660-3953-4041-808c-60f139f0ff20");
                CultureTextProvider.SetMobileResourceText(lblBackOfCheck, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "a935ac63-d7b1-4160-8fc1-0d5aa436337d");
                CultureTextProvider.SetMobileResourceText(btnSubmit, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "d17b1b5f-cdd5-4fd8-96cc-4b467d4fbb6b");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositFragment:SetCultureConfiguration");
			}
		}

		public override void OnSaveInstanceState(Bundle outState)
		{			
			outState.PutString("FrontCheckImage", _frontCheckImageBase64String);
			outState.PutString("BackCheckImage", _backCheckImageBase64String);
			outState.PutString("SourceHeader", txtSourceHeaderText.Text);
			outState.PutString("SourceHeader2", txtSourceHeader2Text.Text);
			outState.PutString("SourceItem", txtSourceItem1Text.Text);
			outState.PutString("SourceValue", txtSourceValue1Text.Text);
			outState.PutString("SourceItem2", txtSourceItem2Text.Text);
			outState.PutString("SourceValue2", txtSourceValue2Text.Text);
			outState.PutString("Amount", txtAmount.Text);
			var json = JsonConvert.SerializeObject(_memberRemoteDepositsInfo);
			outState.PutString("MemberRemoteDepositsInfo", json);
            json = JsonConvert.SerializeObject(_account);
            outState.PutString("Account", json);

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState); 

			base.SetupView();

			accountRow = Activity.FindViewById<TableRow>(Resource.Id.accountRow);
			accountRow.Click += SelectAccount;

			lblDepositTo = Activity.FindViewById<TextView>(Resource.Id.lblDepositTo);
			lblAmount = Activity.FindViewById<TextView>(Resource.Id.lblAmount);
			lblFrontOfCheck = Activity.FindViewById<TextView>(Resource.Id.lblFrontOfCheck);
			lblBackOfCheck = Activity.FindViewById<TextView>(Resource.Id.lblBackOfCheck);
			txtSourceHeaderText = Activity.FindViewById<TextView>(Resource.Id.txtSourceHeaderText);
			txtSourceHeader2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceHeader2Text);
			txtSourceItem1Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceItem1Text);
			txtSourceValue1Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceValue1Text);
			txtSourceItem2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceItem2Text);
			txtSourceValue2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceValue2Text);

			txtAmount = Activity.FindViewById<EditText>(Resource.Id.txtAmount);
			txtAmount.AfterTextChanged += OnTextChanged;

			txtAmountLimit = Activity.FindViewById<TextView>(Resource.Id.txtAmountLimit);

			rowImageFront = Activity.FindViewById<TableRow>(Resource.Id.imageFrontRow);
			rowImageFront.Click += (sender, e) => 
			{				
				GetCheckImage(true);
			};

			imageFront = Activity.FindViewById<ImageView>(Resource.Id.imageFront);

			rowImageBack = Activity.FindViewById<TableRow>(Resource.Id.imageBackRow);
			rowImageBack.Click += (sender, e) => 
			{				
				GetCheckImage(false);
			};

			imageBack = Activity.FindViewById<ImageView>(Resource.Id.imageBack);

			if (_backImage != null)
			{            
				imageBack.SetImageBitmap(_backImage);
			}

			btnSubmit = Activity.FindViewById<Button>(Resource.Id.btnSubmit);
			btnSubmit.Click += (sender, e) => ConfirmDeposit();

			//#if !DEBUG
			CheckForCamera();
			//#endif

			ClearAll();

			if (savedInstanceState != null)
			{				
				_frontCheckImageBase64String = savedInstanceState.GetString("FrontCheckImage");
				_backCheckImageBase64String = savedInstanceState.GetString("BackCheckImage");
				txtSourceHeaderText.Text = savedInstanceState.GetString("SourceHeader");
				txtSourceHeader2Text.Text = savedInstanceState.GetString("SourceHeader2");
				txtSourceItem1Text.Text = savedInstanceState.GetString("SourceItem");
				txtSourceValue1Text.Text = savedInstanceState.GetString("SourceValue");
				txtSourceItem2Text.Text = savedInstanceState.GetString("SourceItem2");
				txtSourceValue2Text.Text = savedInstanceState.GetString("SourceValue2");
				txtAmount.Text = savedInstanceState.GetString("Amount");
				var json = savedInstanceState.GetString("MemberRemoteDepositsInfo");
				_memberRemoteDepositsInfo = JsonConvert.DeserializeObject<GetMemberRemoteDepositsInfoResponse>(json);
                json = savedInstanceState.GetString("Account");
                _account = JsonConvert.DeserializeObject<Account>(json);

				if (!string.IsNullOrEmpty(_frontCheckImageBase64String))
				{
					_frontImage = Images.ConvertBase64StringToBitmap(_frontCheckImageBase64String);
					imageFront.SetImageBitmap(_frontImage);
				}

				if (!string.IsNullOrEmpty(_backCheckImageBase64String))
				{
					_backImage = Images.ConvertBase64StringToBitmap(_frontCheckImageBase64String);
					imageBack.SetImageBitmap(_backImage);
				}
			}			

			GetMemberRemoteDepositsInfo();
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			txtAmount.AfterTextChanged -= OnTextChanged;

			txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((TextView)sender).Text);
			ValidateDeposit();

			txtAmount.SetSelection(txtAmount.Text.Length);

			txtAmount.AfterTextChanged += OnTextChanged;
		}

		private async void GetMemberRemoteDepositsInfo()
		{
			try
			{
				ShowActivityIndicator();

				var request = new GetMemberRemoteDepositsInfoRequest
				{
					MemberId = SessionSettings.Instance.UserId
				};

				if (_memberRemoteDepositsInfo == null)
				{
					var methods = new DepositMethods();
					_memberRemoteDepositsInfo = await methods.GetMemberRemoteDepositsInfo(request, Activity);
				}

				HideActivityIndicator();

				if (_memberRemoteDepositsInfo != null)
				{
					var limitText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "c8374953-6d3c-4742-a27b-8822212caca0", "Daily Limit");
					txtAmountLimit.Text = string.Format("{0}: {1:C}", limitText, _memberRemoteDepositsInfo.DailyLimitAmount);

					if (_memberRemoteDepositsInfo.IsMemberQualified)				
					{		
						if (!_memberRemoteDepositsInfo.IsMemberEnrolled)
						{
							RemoteDepositEnrollment(_memberRemoteDepositsInfo.EnrollmentAgreement);
						}
					}
					else
					{
						GeneralUtilities.DisableView((ViewGroup)View, true);
                        var ok = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "f6f4da34-f12d-400d-9083-58df24aa8de6", "OK");
						await AlertMethods.Alert(Activity, "Deposits", _memberRemoteDepositsInfo.QualificationMessage, ok);
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "DepositsFragment:GetMemberRemoteDepositsInfo");
			}
		}

		private void RemoteDepositEnrollment(string enrollmentAgreement)
		{
			var intent = new Intent(Activity, typeof(DepositEnrollmentActivity));
			intent.PutExtra("AgreementText", enrollmentAgreement);
			StartActivityForResult(intent, 0);
		}

		private DepositCheckRequest PopulateDepositCheckRequest()
		{
			decimal result = 0;
			decimal.TryParse(StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(txtAmount.Text)), out result);

            var request = new DepositCheckRequest
            {
                AmountInCents = (long)(result * 100),
                DepositAccountNumber = _account.Suffix,
				DepositRoutingNumber = string.Empty,
				FrontImageBase64 = _frontCheckImageBase64String,
				BackImageBase64 = _backCheckImageBase64String,
				PhoneDescription = "AndroidPhone",
				ReturnBackImage = false,
				ReturnFrontImage = false,
				ReturnCheckData = true,
				UserNameToken = SessionSettings.Instance.UserId
			};

			return request;
		}

		private void ValidateDeposit()
		{
            if (_account == null || _account.Suffix == null || _frontImage == null || _backImage == null) 
			{
				btnSubmit.Enabled = false;
				return;
			}

			var request = PopulateDepositCheckRequest();

			var methods = new DepositMethods();
			btnSubmit.Enabled = methods.ValidateDepositRequest(request);
		}

		private async void ConfirmDeposit()
		{
			var request = PopulateDepositCheckRequest();

			var msgDepositFunds = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "de1dff59-ed40-4835-94ac-91d57952dbd9", "Deposit Funds");
			var msgReview = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "cd884841-576b-4c64-8b6f-3e791913e7d6", "No, Review");
            var confirmDeposit = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "18960ED7-C564-4A91-A888-82FCA724F2AD", "Confirm Deposit");
            var wouldYouLikeTo = "Would you like to deposit the selected check in the amount of {0:C} into your {1} suffix?";
            var response = await AlertMethods.Alert(Activity, confirmDeposit, string.Format(wouldYouLikeTo, ((decimal)request.AmountInCents / 100), request.DepositAccountNumber), 
                msgDepositFunds, msgReview);

			if (response == msgDepositFunds)
			{
				DepositCheck(request);
			}
		}

		private async void DepositCheck(DepositCheckRequest depositCheckRequest)
		{
			try
			{
				var request = new MobileDeviceVerificationRequest<DepositCheckRequest> { Payload = RetainedSettings.Instance.Payload.Payload, Request = depositCheckRequest };
                var depositingFunds = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "3D515B59-13FD-4BDA-B8B6-DCD7A4AA888A", "Depositing funds...");
                ShowActivityIndicator(depositingFunds);

				var methods = new DepositMethods();
				var response = await methods.DepositCheck(request, Activity, null);

				HideActivityIndicator();

				var logMessage = string.Format("Member Id - {0}, Amount - {1}.", SessionSettings.Instance.UserId, StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text));
				var deposits = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits");
                var ok = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "f6f4da34-f12d-400d-9083-58df24aa8de6", "OK");

				if (response?.Result?.TransactionId > 0)
				{
					var depositSuccessful = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "c4cdfd57-4767-467b-8938-bd28d8b357bd", "Deposit Successful.");
                    var transactionId = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "B89D1618-76BE-494D-9CD4-56DFAFB4A817", "Transaction ID");
                    await AlertMethods.Alert(Activity, deposits, string.Format(depositSuccessful + "\n" + transactionId + ": {0}\n{1}", response.Result?.TransactionId, response?.Result?.CheckStatus?.CheckStatusInfo?.CheckStatusDetail ?? string.Empty), ok);

					ClearAll();
					Logging.Track("Check deposited.");
					Logging.Track("Camera Events", "Check deposited.", SessionSettings.Instance.UserId);					

                    var transactionsFragment = new TransactionsFragment();
                    transactionsFragment.Account = _account;

					NavigationService.NavigatePush(transactionsFragment, false, true);
				}
				else if ((response != null && !response.OutOfBandChallengeRequired) || response == null)
				{
                    var depositFailed = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "bf5f0599-7713-4328-867a-cd2f08905f9d", "Deposit Failed");
                    await AlertMethods.Alert(Activity, deposits, string.Format(depositFailed + "\n{0}", response?.Result?.RejectReason ?? string.Empty), ok);
					Logging.Track("Deposit Events", "Deposit Failed", string.Format("{0}\n{1}", logMessage, response?.Result?.RejectReason ?? string.Empty));
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsFragment:DepositCheck");
			}
		}

		private async void CheckForCamera()
		{
			try
			{
				var isCameraAvailable = await Permissions.GetCameraPermission(Activity);

				var mediaPicker = new MediaPicker(Activity);

				if (!mediaPicker.IsCameraAvailable || !isCameraAvailable)
				{
                    var deposits = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits");
					var message = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "74452122-7b90-49e0-b620-c497ae490300", "Remote Deposits requires a camera.");
                    var ok = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "f6f4da34-f12d-400d-9083-58df24aa8de6", "OK");
                    await AlertMethods.Alert(Activity, deposits, message, ok);
					GeneralUtilities.DisableView((ViewGroup)View, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositsFragment:CheckForCamera");
			}
		}

		private void ClearAll()
		{
			txtSourceHeaderText.Text = string.Empty;
			txtSourceHeader2Text.Text = string.Empty;
			txtSourceItem1Text.Text = string.Empty;
			txtSourceValue1Text.Text = string.Empty;
			txtSourceItem2Text.Text = string.Empty;
			txtSourceValue2Text.Text = string.Empty;
			txtAmount.Text = string.Empty;
		}

		private void SelectAccount(object sender, EventArgs e)
		{
			var intent = new Intent(Activity, typeof(SelectAccountActivity));
			const AccountListTypes accountListType = AccountListTypes.RemoteDepositAccounts;
			var json = JsonConvert.SerializeObject(accountListType);            
			intent.PutExtra("AccountListType", json);

			StartActivityForResult(intent, 0);
		}

		private void GetCheckImage(bool isFrontImage)
		{
			var frontText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "0b673660-3953-4041-808c-60f139f0ff20", "FRONT OF CHECK");
			var backText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "a935ac63-d7b1-4160-8fc1-0d5aa436337d", "BACK OF CHECK");			

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop && !_memberRemoteDepositsInfo.ExcludedCamera2Devices.Contains(Build.Model.ToLower()))			
			{
				var excludedDevices = string.Empty;

				foreach (var device in _memberRemoteDepositsInfo.ExcludedCamera2Devices)
				{
					excludedDevices += device + ",";
				}

                var version = GeneralUtilities.GetAppVersion(Activity);
				
				Logging.Track("Camera Events", $"Using Camera2 API.  Device Model: {Build.Model}", SessionSettings.Instance.UserId);
                Logging.Track("Camera Events", $"Excluded Devices: {excludedDevices}.", SessionSettings.Instance.UserId);
                Logging.Track("Camera Events", $"SunMobile Version: {version}.", SessionSettings.Instance.UserId);

				var intent = new Intent(Activity, typeof(DepositsTakePictureActivity));
				intent.PutExtra("helptext", isFrontImage ? frontText : backText);
				StartActivityForResult(intent, isFrontImage ? 100 : 200);
			}
			else
			{
				var intent = new Intent(Activity, typeof(DepositsScanCheckActivity));
				intent.PutExtra("helptext", isFrontImage ? frontText : backText);
				StartActivityForResult(intent, isFrontImage ? 100 : 200);
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == (int)Result.Ok && requestCode == (int)ActivityResults.AccountVerification)
			{
				var request = PopulateDepositCheckRequest();
				DepositCheck(request);
			}

			if (resultCode == (int)Result.Ok)
			{
				var className = data.GetStringExtra("ClassName");

				switch (className)
				{
					case "DepositEnrollmentActivity":						
						var success = data.GetBooleanExtra("Success", false);
						if (!success)
						{
							GeneralUtilities.DisableView((ViewGroup)View, true);
						}
						break;
					case "SelectAccountActivity":				
						var json = data.GetStringExtra("ListViewItem");
						var listViewItem = JsonConvert.DeserializeObject<ListViewItem>(json);
						json = data.GetStringExtra("Account");
						var account = JsonConvert.DeserializeObject<Account>(json);
						listViewItem.Data = account;
                        _account = account;
						txtSourceHeaderText.Text = listViewItem.HeaderText;
						txtSourceHeader2Text.Text = listViewItem.Header2Text;
						txtSourceItem1Text.Text = listViewItem.Item1Text;
						txtSourceItem2Text.Text = listViewItem.Item2Text;
						txtSourceValue1Text.Text = listViewItem.Value1Text;
						txtSourceValue2Text.Text = listViewItem.Value2Text;												
						ValidateDeposit();
						break;
					case "DepositsScanCheckActivity":
						string base64String = RetainedSettings.Instance.CheckImage;	
						
                        if (requestCode == 100)
						{
							_frontImage = Images.ConvertBase64StringToBitmap(base64String);
							imageFront.SetImageBitmap(_frontImage);
							_frontCheckImageBase64String = Images.ConvertBitmapToBase64String(_frontImage, false);							
						}
						else
						{
							_backImage = Images.ConvertBase64StringToBitmap(base64String);
							imageBack.SetImageBitmap(_backImage);
							_backCheckImageBase64String = Images.ConvertBitmapToBase64String(_backImage, false);							
						}
						ValidateDeposit();
						break;
					case "DepositsTakePictureActivity":
						base64String = RetainedSettings.Instance.CheckImage;	
						
                        if (requestCode == 100)
						{
							_frontImage = Images.ConvertBase64StringToBitmap(base64String);
							imageFront.SetImageBitmap(_frontImage);
							_frontCheckImageBase64String = Images.ConvertBitmapToBase64String(_frontImage, false);							
						}
						else
						{
							_backImage = Images.ConvertBase64StringToBitmap(base64String);
							imageBack.SetImageBitmap(_backImage);
							_backCheckImageBase64String = Images.ConvertBitmapToBase64String(_backImage, false);							
						}
						ValidateDeposit();
						break;
				}
			}
		}		
	}
}