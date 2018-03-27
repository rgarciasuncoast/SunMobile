using System;
using System.Globalization;
using Foundation;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile;
using SunMobile.iOS.Accounts;
using SunMobile.iOS.Authentication;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Transfers
{
    public partial class TransfersTableViewController : BaseTableViewController, IVerificationViewController
	{
		private TransferExecuteRequest _transferExecuteRequest;
        private SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account _account;
		private bool _sourceIsJointAccount;
		private bool _destinationIsJointAccount;
        private bool _saveToFavorites;
        private bool _isFavorite;
		private string textTransfers;
		private string confirmText1;
		private string confirmText2;
		private string confirmTransfer;
		private string transferFunds;
		private string noReview;
		private string transferStatus;
		private string transferFailed;

		public TransfersTableViewController(IntPtr handle) : base(handle)
		{
			_transferExecuteRequest = new TransferExecuteRequest();
			_transferExecuteRequest.Destination = new TransferTarget();
			_transferExecuteRequest.Source = new TransferTarget();
			_sourceIsJointAccount = false;
			_destinationIsJointAccount = false;
		}

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var transferText = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "28E5A933-BF24-42DE-ADC8-072C1C80D9A9", "Transfer");

            var rightButton = new UIBarButtonItem(transferText, UIBarButtonItemStyle.Plain, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            NavigationItem.RightBarButtonItem.Enabled = false;
            rightButton.Clicked += (sender, e) => ConfirmTransfer();

            txtAmount.EditingChanged += (sender, e) =>
            {
                txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((UITextField)sender).Text);
                ValidateTransfer();
            };

            txtAmount.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return replacementString.IsNumericOrEmpty() && newLength <= 15;
            };

            // Hides the remaining rows.
            tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            ClearAll();
        }

        public override void SetCultureConfiguration()
		{
            Title = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "9D59AD3D-BFC3-44EF-83F4-FEF687D9429F", "Transfers");
			confirmText1 = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "7DFA227D-4839-4513-A4EF-D81BF2C83D47", "Transfer Amount: {0}\nFrom: {1}\n{2}\nTo: {3}\n{4}");
			confirmText2 = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "4A387D12-F90A-465C-A0D6-CD0E76CF7980", "Transfer Amount: {0}\nFrom: {1} To: {2}-{3}");
			confirmTransfer = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "CFBEF5CF-F493-45D7-ABE7-3FF6F11751E3", "Confirm Transfer");
			transferFunds = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "6DFA2873-5A10-4612-A859-8EC161F4D7F0", "Transfer Funds");
			noReview = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "29C17C93-3637-4C9C-A45B-160C3DB2633A", "No, Review");
			transferStatus = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "0988AD6E-2471-4A5C-801F-00E94FA352A2", "Transfer Status");
			transferFailed = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "CF1531D7-4D7C-4AFD-A523-71DE4BAA0E9C", "Transfer failed.");
			textTransfers = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "9D59AD3D-BFC3-44EF-83F4-FEF687D9429F", "Transfers");
			CultureTextProvider.SetMobileResourceText(lblFrom, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "9CE821B3-1E22-4198-A09B-DC477E0E42AF", "Transfer Funds From");
			CultureTextProvider.SetMobileResourceText(lblTo, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "CB069902-A95D-464A-AF21-A076A9D91567", "Transfer Funds To");
			CultureTextProvider.SetMobileResourceText(lblAmount, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "9AE82098-98D2-4D0F-B2F2-113E01E00F43", "Amount");
			txtAmount.Placeholder = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "D30B1037-C08C-4F8E-914F-31976FBA2277", "Enter Amount");
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row)
			{
				case 0: // Source Account
					SelectAccount(0);
					break;
				case 1: // Destination Account
					SelectAccount(1);
					break;
                case 3: // Favorites
                    try
                    {
                        var transferFavoritesTableViewController = AppDelegate.StoryBoard.InstantiateViewController("TransferFavoritesTableViewController") as TransferFavoritesTableViewController;

                        transferFavoritesTableViewController.Completed += (favorite) =>
                        {
                            //Setup Source
                            _transferExecuteRequest.Source.MemberId = favorite.Source.MemberId;
                            _transferExecuteRequest.Source.Suffix = favorite.Source.Suffix;
                            _transferExecuteRequest.Source.AccountType = favorite.Source.AccountType;

                            //Setup Destination
                            _transferExecuteRequest.Destination.MemberId = favorite.Destination.MemberId;
                            _transferExecuteRequest.Destination.Suffix = favorite.Destination.Suffix;
                            _transferExecuteRequest.Destination.AccountType = favorite.Destination.AccountType;

                            //Setup Transfer
                            _transferExecuteRequest.Amount = favorite.Amount;
                            _transferExecuteRequest.RequestingMemberId = GeneralUtilities.GetMemberIdAsInt();
                            _transferExecuteRequest.Payload = RetainedSettings.Instance.Payload;

                            if (!string.IsNullOrEmpty(favorite.LastName))
                            {
                                _transferExecuteRequest.LastName = favorite.LastName;
                            }

                            if (!string.IsNullOrEmpty(favorite.LastEightOfAtmDebitCard))
                            {
                                _transferExecuteRequest.LastEightOfAtmDebitCard = favorite.LastEightOfAtmDebitCard;
                            }

                            _transferExecuteRequest.IsJointTransfer = favorite.IsJoint;

                            lblSource.Text = favorite.SourceAccountDescription;
                            lblSource2.Text = favorite.Source.MemberId + "-" + favorite.Source.Suffix;
                            lblSourceText1.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "805D4ECB-244C-4E98-B063-CC32706086DA", "Balance") + ":";
                            lblSourceText2.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "30EF1F3D-3F26-4B82-83C6-1A278F302D5A", "Available Balance") + ":";
                            lblSourceValue1.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.SourceBalance);
                            lblSourceValue2.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.SourceAvailableBalance);

                            if (string.IsNullOrEmpty(favorite.LastName) && string.IsNullOrEmpty(favorite.LastEightOfAtmDebitCard))
                            {
                                lblDestination.Text = favorite.DestinationAccountDescription;
                                lblDestination2.Text = favorite.Destination.MemberId + "-" + favorite.Destination.Suffix;
                                lblDestinationText1.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "805D4ECB-244C-4E98-B063-CC32706086DA", "Balance") + ":";
                                lblDestinationText2.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "30EF1F3D-3F26-4B82-83C6-1A278F302D5A", "Available Balance") + ":";
                                lblDestinationValue1.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.DestinationBalance);
                                lblDestinationValue2.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.DestinationAvailableBalance);
                            }
                            else
                            {
                                // Any Member
                                lblDestination.Text = (_transferExecuteRequest.Destination.AccountType == "CreditCards" ? "Credit Card" : _transferExecuteRequest.Destination.AccountType) + " Account " + favorite.Destination.MemberId + "-" + favorite.Destination.Suffix;
                                lblDestination2.Text = favorite.LastName;
                                _destinationIsJointAccount = false;
                                lblDestinationText1.Text = string.Empty;
                                lblDestinationText2.Text = string.Empty;
                                lblDestinationValue1.Text = string.Empty;
                                lblDestinationValue2.Text = string.Empty;
                            }

                            txtAmount.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.Amount);

                            ValidateTransfer();

                            _isFavorite = true;
                        };

                        NavigationController.PushViewController(transferFavoritesTableViewController, true);                  
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex, "TransfersTableViewController:RowSelected:Case:Favorites");
                    }
                    break;
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			txtAmount.ResignFirstResponder();
		}

		private void ClearAll()
		{
			lblSource.Text = string.Empty;
			lblSource2.Text = string.Empty;
			lblSourceText1.Text = string.Empty;
			lblSourceText2.Text = string.Empty;
			lblSourceValue1.Text = string.Empty;
			lblSourceValue2.Text = string.Empty;

			lblDestination.Text = string.Empty;
			lblDestination2.Text = string.Empty;
			lblDestinationText1.Text = string.Empty;
			lblDestinationText2.Text = string.Empty;
			lblDestinationValue1.Text = string.Empty;
			lblDestinationValue2.Text = string.Empty;

			txtAmount.Text = string.Empty;

            NavigationItem.RightBarButtonItem.Enabled = false;
		}

		private void SelectAccount(int row)
		{
			int excludeMemberId = 0;
			string excludeSuffix = string.Empty;

			var selectAccountViewController = AppDelegate.StoryBoard.InstantiateViewController("SelectAccountViewController") as SelectAccountViewController;

			if (row == 0)
			{
				if (_transferExecuteRequest.Destination.Suffix != string.Empty)
				{
					excludeMemberId = _transferExecuteRequest.Destination.MemberId;
					excludeSuffix = _transferExecuteRequest.Destination.Suffix;
				}

				selectAccountViewController.AccountListType = AccountListTypes.TransferSourceAccounts;
				selectAccountViewController.ShowJoints = true;
				selectAccountViewController.ShowAnyMember = false;
				selectAccountViewController.ExcludeMemberId = excludeMemberId;
				selectAccountViewController.ExcludeSuffix = excludeSuffix;
			}

			if (row == 1)
			{
				if (_transferExecuteRequest.Source.Suffix != string.Empty)
				{
					excludeMemberId = _transferExecuteRequest.Source.MemberId;
					excludeSuffix = _transferExecuteRequest.Source.Suffix;
				}

				selectAccountViewController.AccountListType = AccountListTypes.TransferTargetAccounts;
				selectAccountViewController.ShowJoints = true;
				selectAccountViewController.ShowAnyMember = true;
				selectAccountViewController.ExcludeMemberId = excludeMemberId;
				selectAccountViewController.ExcludeSuffix = excludeSuffix;
			}

			selectAccountViewController.AccountSelected += listViewItem =>
			{
				var account = (SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account)listViewItem.Data;

				if (row == 0)
				{
					lblSource.Text = listViewItem.HeaderText;
					lblSource2.Text = listViewItem.Header2Text;
					lblSourceText1.Text = listViewItem.Item1Text;
					lblSourceText2.Text = listViewItem.Item2Text;
					lblSourceValue1.Text = listViewItem.Value1Text;
					lblSourceValue2.Text = listViewItem.Value2Text;
                    lblSourceText1.AccessibilityLabel = listViewItem.Item1Text + listViewItem.Value1Text;
                    lblSourceValue1.AccessibilityLabel = string.Empty;
                    lblSourceText2.AccessibilityLabel = listViewItem.Item2Text + listViewItem.Value2Text;
                    lblSourceValue2.AccessibilityLabel = string.Empty;
					_transferExecuteRequest.Source.MemberId = account.MemberId;
					_transferExecuteRequest.Source.Suffix = account.Suffix;
					_transferExecuteRequest.Source.AccountType = account.AccountType;
					_sourceIsJointAccount = listViewItem.Bool1Value;
                    _account = account;
				}
				else
				{
					lblDestination.Text = listViewItem.HeaderText;
					lblDestination2.Text = listViewItem.Header2Text;
					lblDestinationText1.Text = listViewItem.Item1Text;
					lblDestinationText2.Text = listViewItem.Item2Text;
					lblDestinationValue1.Text = listViewItem.Value1Text;
					lblDestinationValue2.Text = listViewItem.Value2Text;
                    lblDestinationText1.AccessibilityLabel = listViewItem.Item1Text + listViewItem.Value1Text;
                    lblDestinationValue1.AccessibilityLabel = string.Empty;
                    lblDestinationText2.AccessibilityLabel = listViewItem.Item2Text + listViewItem.Value2Text;
                    lblDestinationValue2.AccessibilityLabel = string.Empty;
					_transferExecuteRequest.Destination.MemberId = account.MemberId;
					_transferExecuteRequest.Destination.Suffix = account.Suffix;
					_transferExecuteRequest.Destination.AccountType = account.AccountType;
					_destinationIsJointAccount = listViewItem.Bool1Value;
					_transferExecuteRequest.LastName = string.Empty;
				}

				_transferExecuteRequest.IsJointTransfer = _sourceIsJointAccount || _destinationIsJointAccount;

				ValidateTransfer();
			};

			selectAccountViewController.AnyMemberSelected += anyMemberInfo =>
			{
				lblDestination.Text = (anyMemberInfo.AccountType == "CreditCards" ? "Credit Card" : anyMemberInfo.AccountType) + " Account " + anyMemberInfo.Account + "-" + anyMemberInfo.Suffix;
				lblDestination2.Text = anyMemberInfo.LastName;
                lblDestinationText1.Text = string.Empty;
                lblDestinationText2.Text = string.Empty;
                lblDestinationValue1.Text = string.Empty;
                lblDestinationValue2.Text = string.Empty;
				_transferExecuteRequest.Destination.Suffix = anyMemberInfo.Suffix;
				_transferExecuteRequest.Destination.AccountType = anyMemberInfo.AccountType;
				int destinationMemberId;
				int.TryParse(anyMemberInfo.Account, out destinationMemberId);
				_transferExecuteRequest.Destination.MemberId = destinationMemberId;
				_transferExecuteRequest.IsJointTransfer = anyMemberInfo.IsJoint;
				_transferExecuteRequest.LastName = anyMemberInfo.LastName;
				_destinationIsJointAccount = false;

				_transferExecuteRequest.IsJointTransfer = _sourceIsJointAccount || _destinationIsJointAccount;

				ValidateTransfer();
			};

			NavigationController.PushViewController(selectAccountViewController, true);
		}

		private void PopulateTransferRequest()
		{
			var amountText = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(txtAmount.Text));
			double amount;
			double.TryParse(amountText, out amount);
			_transferExecuteRequest.Amount = (decimal)amount;
			_transferExecuteRequest.RequestingMemberId = GeneralUtilities.GetMemberIdAsInt();
			_transferExecuteRequest.Payload = RetainedSettings.Instance.Payload;
		}

		private void ValidateTransfer()
		{
            _isFavorite = false;

			PopulateTransferRequest();

			var methods = new TransferMethods();
			NavigationItem.RightBarButtonItem.Enabled = methods.ValidateTransferRequest(_transferExecuteRequest);
		}

		private void ConfirmTransfer()
		{
			PopulateTransferRequest();

			txtAmount.ResignFirstResponder();

			var confirmMessage = string.Format(confirmText1,
				StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text),
				lblSource.Text, lblSource2.Text, lblDestination.Text, lblDestination2.Text);
                                               
            var alertViewController = AppDelegate.StoryBoard.InstantiateViewController("CustomAlertViewController") as CustomAlertViewController;
            alertViewController.CheckBoxText = "Save to Favorites";

            if (_isFavorite)
            {
                confirmMessage += "\n\nThis transfer is saved in your list of favorites.";
                alertViewController.CheckBoxHidden = true;
                alertViewController.CheckBoxText = string.Empty;
            }

            alertViewController.Header = confirmTransfer;
            alertViewController.Message = confirmMessage;
            alertViewController.PositiveButtonText = CultureTextProvider.SUBMIT();
            alertViewController.NegativeButtonText = CultureTextProvider.NOREVIEW();
            alertViewController.MakeSwitchOptional = true;

            alertViewController.Completed += (shouldcontinue) =>
            {
                DismissModalViewController(true);

                if (shouldcontinue)
                {
                    _saveToFavorites = alertViewController.SwitchOn;
                    TransferFunds(_transferExecuteRequest);
                }
            };

            PresentModalViewController(alertViewController, true);
		}

		private async void TransferFunds(TransferExecuteRequest request)
		{
			ShowActivityIndicator();

			var methods = new TransferMethods();
			var response = await methods.Transfer(request, View, NavigationController);

			HideActivityIndicator();

			ProcessResult(response);
		}

		private async void ProcessResult(MobileStatusResponse response)
		{
			var logMessage = string.Format(confirmText2,
				StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text),
				lblSource2.Text, _transferExecuteRequest.Destination.MemberId, _transferExecuteRequest.Destination.Suffix);

			if (response != null && response.Success && !response.OutOfBandChallengeRequired)
			{
                if (_saveToFavorites)
                {
                    var newTransferFavorite = new TransferFavorite();
                    newTransferFavorite.Source = _transferExecuteRequest.Source;
                    newTransferFavorite.Destination = _transferExecuteRequest.Destination;
                    newTransferFavorite.Amount = _transferExecuteRequest.Amount;
                    newTransferFavorite.LastName = _transferExecuteRequest?.LastName;
                    newTransferFavorite.LastEightOfAtmDebitCard = _transferExecuteRequest?.LastEightOfAtmDebitCard;
                    newTransferFavorite.IsJoint = _transferExecuteRequest.IsJointTransfer;

                    var methods = new TransferMethods();

                    ShowActivityIndicator();

                    var favoritesResponse = await methods.GetTransferFavorites(null, View);

                    if (favoritesResponse != null && favoritesResponse.Result != null)
                    {
                        favoritesResponse.Result.Add(newTransferFavorite);
                        await methods.SetTransferFavorites(favoritesResponse.Result, View);
                    }

                    HideActivityIndicator();
                }

                var successMessage = string.Format("Successfully Transferred {0}\nFrom: {1}\n{2}\nTo: {3}\n{4}",
                StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text),
                lblSource.Text, lblSource2.Text, lblDestination.Text, lblDestination2.Text);

                var alertResponse = await AlertMethods.Alert(View, transferStatus, successMessage, "Make Another Transfer", "Go to Account Transactions");

				var controller = AppDelegate.StoryBoard.InstantiateViewController("TransactionsViewController") as TransactionsViewController;
                controller.Account = _account;								

				ClearAll();

				Logging.Track("Transfer Events", "Transfer Success", logMessage);
				Logging.Track("Transfer executed.");

                if (alertResponse == "Go to Account Transactions")
                {
                    AppDelegate.MenuNavigationController.PopBackAndRunController(controller);
                }
			}
			else if ((response != null && !response.Success && !response.OutOfBandChallengeRequired) || response == null)
			{
				await AlertMethods.Alert(View, textTransfers, response?.FailureMessage ?? transferFailed, "OK");
				Logging.Track("Transfer Events", "Transfer Failed", string.Format("{0} - {1}", logMessage, response?.FailureMessage ?? "Transfer failed."));
			}
		}

		public void OnAccountVerified()
		{
            TransferFunds(_transferExecuteRequest);
		}
	}
}