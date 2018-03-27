using System;
using Foundation;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Culture;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Authentication;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;
using UIKit;
using Xamarin.Media;

namespace SunMobile.iOS.Deposits
{
	public partial class DepositsTableViewController : BaseTableViewController, IVerificationViewController, ICultureConfigurationProvider
	{
		public bool IsFrontImage { get; set; }
        private Account _account;
		private string _frontCheckImageBase64String;
		private string _backCheckImageBase64String;
		private UIImage _imgCaptureCheck;

		public DepositsTableViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_imgCaptureCheck = imgCheckFront.Image;

            var depositText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "D17B1B5F-CDD5-4FD8-96CC-4B467D4FBB6B", "Deposit");
			var rightButton = new UIBarButtonItem(depositText, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => ConfirmDeposit();

			txtAmount.EditingChanged += (sender, e) => 
			{
				txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((UITextField)sender).Text);
				ValidateDeposit();
			};

			txtAmount.ShouldChangeCharacters = (textField, range, replacementString) => 
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 15;
			};

			// Hides the remaining rows.
			tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);	

			ClearAll();

			#if !DEBUG
			CheckForCamera();
			#else
			GetMemberRemoteDepositsInfo();
			#endif
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "EA21E6DE-7F1E-495D-874F-0F8B5BFBD2B4", "Deposits");
			CultureTextProvider.SetMobileResourceText(lblDepositToHeader, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "ab612024-2fba-4a3e-9150-a3a6a2c9ce74");
			CultureTextProvider.SetMobileResourceText(lblAmount, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "bcea972a-2cfa-4778-946d-d5389f3df813");
			CultureTextProvider.SetMobileResourceText(txtAmount, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "e31ad74c-c279-4971-bda4-53950950d563");
			CultureTextProvider.SetMobileResourceText(lblFrontOfCheck, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "0b673660-3953-4041-808c-60f139f0ff20");
			CultureTextProvider.SetMobileResourceText(lblBackOfCheck, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "a935ac63-d7b1-4160-8fc1-0d5aa436337d");
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row)
			{
				case 0: // Account
					SelectAccount();
					break;
				case 2: // Check Front
					IsFrontImage = true;
					GetCheckImage();
					break;
				case 3: // Check Back
					IsFrontImage = false;
					GetCheckImage();
					break;
			}
		}

		private async void GetMemberRemoteDepositsInfo()
		{
			try 
			{
				var request = new GetMemberRemoteDepositsInfoRequest
				{
					MemberId = SessionSettings.Instance.UserId
				};

				ShowActivityIndicator();

				var methods = new DepositMethods();
				var response = await methods.GetMemberRemoteDepositsInfo(request, null);

				HideActivityIndicator();

				if (response != null)
				{
					var limitText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "c8374953-6d3c-4742-a27b-8822212caca0", "Daily Limit");
					lblDailyLimit.Text = string.Format("{0}: {1:C}", limitText, response.DailyLimitAmount);

					if (response.IsMemberQualified)
					{
						if (!response.IsMemberEnrolled)                    					
						{
							RemoteDepositEnrollment(response.EnrollmentAgreement);
						}
					}
					else
					{
                        var ok = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "f6f4da34-f12d-400d-9083-58df24aa8de6", "OK");
						await AlertMethods.Alert(View, "Deposits", response.QualificationMessage, ok);
						View.UserInteractionEnabled = false;
						View.Alpha = 0.3f;
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "DepositsTableViewController:GetMemberRemoteDepositsInfo");
			}
		}

		private void RemoteDepositEnrollment(string enrollmentAgreement)
		{
			var myStoryboard = AppDelegate.StoryBoard;
			var controller = myStoryboard.InstantiateViewController("DepositsEnrollmentViewController") as DepositsEnrollmentViewController;
			controller.AgreementText = enrollmentAgreement;

			controller.EnrollmentFinished += isComplete => 
			{
				if (!isComplete) 
				{
					View.UserInteractionEnabled = false;
					View.Alpha = 0.3f;
				}
			};

			NavigationController.PushViewController(controller, true);
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
				PhoneDescription = "iPhone",
				ReturnBackImage = false,
				ReturnFrontImage = false,
				ReturnCheckData = true,
				UserNameToken = SessionSettings.Instance.UserId
			};

			return request;
		}

		private void ValidateDeposit()
		{
			var request = PopulateDepositCheckRequest();

			var methods = new DepositMethods();
			NavigationItem.RightBarButtonItem.Enabled = methods.ValidateDepositRequest(request);
		}

		private async void ConfirmDeposit()
		{
			txtAmount.ResignFirstResponder();

			var request = PopulateDepositCheckRequest();

			var txtDepositFunds = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "de1dff59-ed40-4835-94ac-91d57952dbd9", "Deposit Funds");
			var txtReview = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "cd884841-576b-4c64-8b6f-3e791913e7d6", "No, Review");
			var confirmDeposit = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "18960ED7-C564-4A91-A888-82FCA724F2AD", "Confirm Deposit");
			var wouldYouLikeTo = "Would you like to deposit the selected check in the amount of {0:C} into your {1} suffix?";
			var response = await AlertMethods.Alert(View, confirmDeposit, string.Format(wouldYouLikeTo, ((decimal)request.AmountInCents / 100), request.DepositAccountNumber), 
                txtDepositFunds, txtReview);

			if (response == txtDepositFunds)
			{
				DepositCheck(request);
			}
		}

		private async void DepositCheck(DepositCheckRequest depositCheckRequest)
		{
		    try
		    {
				var request = new MobileDeviceVerificationRequest<DepositCheckRequest> { Payload = RetainedSettings.Instance.Payload.Payload, Request = depositCheckRequest };

		        ShowActivityIndicator();

		        var methods = new DepositMethods();
				var response = await methods.DepositCheck(request, View, NavigationController);

		        HideActivityIndicator();

				var logMessage = string.Format("Member Id - {0}, Amount - {1}.", SessionSettings.Instance.UserId, StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text)); 
                var deposits = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits");
                var ok = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "f6f4da34-f12d-400d-9083-58df24aa8de6", "OK");

				if (response?.Result?.TransactionId > 0)
                {
					var depositSuccessful = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "c4cdfd57-4767-467b-8938-bd28d8b357bd", "Deposit Successful.");
					var transactionId = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "B89D1618-76BE-494D-9CD4-56DFAFB4A817", "Transaction ID");
                    await AlertMethods.Alert(View, deposits, string.Format(depositSuccessful + "\n" + transactionId + ": {0}\n{1}", response?.Result?.TransactionId, response?.Result?.CheckStatus?.CheckStatusInfo?.CheckStatusDetail ?? string.Empty), ok);

		            var myStoryboard = AppDelegate.StoryBoard;
		            var controller = myStoryboard.InstantiateViewController("TransactionsViewController") as TransactionsViewController;
                    controller.Account = _account;		            

		            ClearAll();

					Logging.Track("Deposit Events", "Deposit Success", logMessage);
		            Logging.Track("Check deposited.");

		            AppDelegate.MenuNavigationController.PopBackAndRunController(controller);
		        }
                else if ((response != null && !response.OutOfBandChallengeRequired) || response == null)
                {
					var depositFailed = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "bf5f0599-7713-4328-867a-cd2f08905f9d", "Deposit Failed");
                    await AlertMethods.Alert(View, deposits, string.Format(depositFailed + "\n{0}", response?.Result?.RejectReason ?? string.Empty), ok);
					Logging.Track("Deposit Events", "Deposit Failed", string.Format("{0}\n{1}", logMessage, response?.Result?.RejectReason ?? string.Empty));
                }
            }
		    catch (Exception ex)
		    {
		        Logging.Log(ex, "DepositsTableViewController:DepositCheck");
		    }
		}

		private async void CheckForCamera()
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

			if (status != PermissionStatus.Granted)
			{
				var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });

				if (results != null)
				{
					status = results[Permission.Camera];
				}
			}

			var mediaPicker = new MediaPicker();

			if (!mediaPicker.IsCameraAvailable || status != PermissionStatus.Granted)
			{
                var deposits = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits");
				var message = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "74452122-7b90-49e0-b620-c497ae490300", "Remote Deposits requires a camera.");
                var ok = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "f6f4da34-f12d-400d-9083-58df24aa8de6", "OK");
                await AlertMethods.Alert(View, deposits, message, ok);
				NavigationController.PopViewController(true);
			}
			else
			{
				GetMemberRemoteDepositsInfo();
			}
		}

		private void ClearAll()
		{
			lblDepositTo.Text = string.Empty;
			lblDepositTo2.Text = string.Empty;
			lblText1.Text = string.Empty;
			lblText2.Text = string.Empty;
			lblValue1.Text = string.Empty;
			lblValue2.Text = string.Empty;
			txtAmount.Text = string.Empty;
			imgCheckFront.Image = _imgCaptureCheck;
			imgCheckBack.Image = _imgCaptureCheck;
		}

		private void SelectAccount()
		{
			try 
			{
				txtAmount.ResignFirstResponder();

				var selectAccountViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectAccountViewController") as SelectAccountViewController;
				selectAccountViewController.AccountListType = AccountListTypes.RemoteDepositAccounts;
				selectAccountViewController.ShowJoints = false;
				selectAccountViewController.ShowAnyMember = false;

				selectAccountViewController.AccountSelected += listViewItem =>
				{
					lblDepositTo.Text = listViewItem.HeaderText;
					lblDepositTo2.Text = listViewItem.Header2Text;
					lblText1.Text = listViewItem.Item1Text;
					lblText2.Text = listViewItem.Item2Text;
					lblValue1.Text = listViewItem.Value1Text;
					lblValue2.Text = listViewItem.Value2Text;
                    _account = (Account)listViewItem.Data;
                    lblText1.AccessibilityLabel = listViewItem.Item1Text + listViewItem.Value1Text;
                    lblValue1.AccessibilityLabel = string.Empty;
                    lblText2.AccessibilityLabel = listViewItem.Item2Text + listViewItem.Value2Text;
                    lblValue2.AccessibilityLabel = string.Empty;

					ValidateDeposit();
				};

				NavigationController.PushViewController(selectAccountViewController, true);
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "DepositsTableViewController:SelectAccount");
			}
		}

		private void GetCheckImage()
		{
			try 
			{
				txtAmount.ResignFirstResponder();

				var cameraViewController = AppDelegate.StoryBoard.InstantiateViewController("CameraViewController") as CameraViewController;

				var frontText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "0b673660-3953-4041-808c-60f139f0ff20", "FRONT OF CHECK");
				var backText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "a935ac63-d7b1-4160-8fc1-0d5aa436337d", "BACK OF CHECK");

				cameraViewController.HelpText = IsFrontImage ? frontText : backText;

				cameraViewController.PictureTakenDelegate += image =>
				{
					if (IsFrontImage)
					{
						_frontCheckImageBase64String = Images.ConvertUIImageToBase64String(image);
						SetImage(image, true);
					}
					else
					{
						_backCheckImageBase64String = Images.ConvertUIImageToBase64String(image);
						SetImage(image, false);
					}
				};

				NavigationController.PushViewController(cameraViewController, true);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void SetImage(UIImage image, bool isFront)
		{
			if (isFront)
			{
				imgCheckFront.Image = image;
			}
			else
			{
				imgCheckBack.Image = image;
			}

			ValidateDeposit();
		}        

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			txtAmount.ResignFirstResponder();
		}

		public void OnAccountVerified()
		{
			var request = PopulateDepositCheckRequest();
			DepositCheck(request);
		}
	}
}