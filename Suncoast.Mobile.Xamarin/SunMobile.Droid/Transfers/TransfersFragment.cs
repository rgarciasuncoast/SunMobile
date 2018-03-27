using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.Mobile;
using SunMobile.Droid.Accounts;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Transfers
{
	public class TransfersFragment : BaseFragment
	{
		private TextView txtSourceHeaderText;
		private TextView txtSourceHeader2Text;
		private TextView txtSourceItem1Text;
		private TextView txtSourceValue1Text;
		private TextView txtSourceItem2Text;
		private TextView txtSourceValue2Text;

		private TextView txtDestinationHeaderText;
		private TextView txtDestinationHeader2Text;
		private TextView txtDestinationItem1Text;
		private TextView txtDestinationValue1Text;
		private TextView txtDestinationItem2Text;
		private TextView txtDestinationValue2Text;

		private EditText txtAmount;
		private Button btnTransfer;
		private TableRow rowSource;
		private TableRow rowDestination;
        private TableRow rowFavorites;

		private TransferExecuteRequest _transferExecuteRequest;
		private bool _sourceIsJointAccount;
		private bool _destinationIsJointAccount;
        private SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account _sourceAccount;

		private string textTransfers;
		private string confirmText1;
		private string confirmText2;
		private string confirmTransfer;
		private string transferFunds;
		private string noReview;
		private string transferringFunds;
		private string transferStatus;
		private string transferFailed;
        private bool _saveToFavorites;
        private bool _isFavorite;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AccountTransferView, null);
			RetainInstance = true;

			// This will make sure the toolbar stays visible on rotation.  
			// It was being hidden because of the keyboard open.
			Activity.Window.SetSoftInputMode(SoftInput.AdjustResize);

			_transferExecuteRequest = new TransferExecuteRequest();
			_transferExecuteRequest.Destination = new TransferTarget();
			_transferExecuteRequest.Source = new TransferTarget();
           	_sourceIsJointAccount = false;
			_destinationIsJointAccount = false;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			var json = JsonConvert.SerializeObject(_transferExecuteRequest);
			outState.PutString("transferExecuteRequest", json);
			outState.PutString("txtAmount", txtAmount.Text);

			outState.PutString("txtSourceHeaderText", txtSourceHeaderText.Text);
			outState.PutString("txtSourceHeader2Text", txtSourceHeader2Text.Text);
			outState.PutString("txtSourceItem1Text", txtSourceItem1Text.Text);
			outState.PutString("txtSourceItem2Text", txtSourceItem2Text.Text);
			outState.PutString("txtSourceValue1Text", txtSourceValue1Text.Text);
			outState.PutString("txtSourceValue2Text", txtSourceValue2Text.Text);

			outState.PutString("txtDestinationHeaderText", txtDestinationHeaderText.Text);
			outState.PutString("txtDestinationHeader2Text", txtDestinationHeader2Text.Text);
			outState.PutString("txtDestinationItem1Text", txtDestinationItem1Text.Text);
			outState.PutString("txtDestinationItem2Text", txtDestinationItem2Text.Text);
			outState.PutString("txtDestinationValue1Text", txtDestinationValue1Text.Text);
			outState.PutString("txtDestinationValue2Text", txtDestinationValue2Text.Text);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			textTransfers = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "9D59AD3D-BFC3-44EF-83F4-FEF687D9429F", "Transfers");

			((MainActivity)Activity).SetActionBarTitle(textTransfers);

			rowSource = Activity.FindViewById<TableRow>(Resource.Id.rowFrom);

			rowSource.Click += (sender, e) =>
			{				
				SelectAccount("Source");
			};

			rowDestination = Activity.FindViewById<TableRow>(Resource.Id.rowTo);

			rowDestination.Click += (sender, e) =>
			{				
				SelectAccount("Destination");
			};

            rowFavorites = Activity.FindViewById<TableRow>(Resource.Id.favoritesRow);

            rowFavorites.Click += (sender, e) =>  
            {
                var intent = new Intent(Activity, typeof(TransferFavoritesActivity));
                StartActivityForResult(intent, 400);
            };

			txtSourceHeaderText = Activity.FindViewById<TextView>(Resource.Id.txtSourceHeaderText);
			txtSourceHeader2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceHeader2Text);
			txtSourceItem1Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceItem1Text);
			txtSourceValue1Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceValue1Text);
			txtSourceItem2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceItem2Text);
			txtSourceValue2Text = Activity.FindViewById<TextView>(Resource.Id.txtSourceValue2Text);

			txtDestinationHeaderText = Activity.FindViewById<TextView>(Resource.Id.txtDestinationHeaderText);
			txtDestinationHeader2Text = Activity.FindViewById<TextView>(Resource.Id.txtDestinationHeader2Text);
			txtDestinationItem1Text = Activity.FindViewById<TextView>(Resource.Id.txtDestinationItem1Text);
			txtDestinationValue1Text = Activity.FindViewById<TextView>(Resource.Id.txtDestinationValue1Text);
			txtDestinationItem2Text = Activity.FindViewById<TextView>(Resource.Id.txtDestinationItem2Text);
			txtDestinationValue2Text = Activity.FindViewById<TextView>(Resource.Id.txtDestinationValue2Text);

			txtAmount = Activity.FindViewById<EditText>(Resource.Id.txtAmount);
			txtAmount.AfterTextChanged += OnTextChanged;

			btnTransfer = Activity.FindViewById<Button>(Resource.Id.btnTransfer);
			btnTransfer.Click += (sender, e) => ConfirmTransfer();

			ClearAll();

			if (savedInstanceState != null)
			{
				_transferExecuteRequest = JsonConvert.DeserializeObject<OutOfBandTransferExecuteRequest>(savedInstanceState.GetString("transferExecuteRequest"));
				txtAmount.Text = savedInstanceState.GetString("txtAmount");

				txtSourceHeaderText.Text = savedInstanceState.GetString("txtSourceHeaderText");
				txtSourceHeader2Text.Text = savedInstanceState.GetString("txtSourceHeader2Text");
				txtSourceItem1Text.Text = savedInstanceState.GetString("txtSourceItem1Text");
				txtSourceItem2Text.Text = savedInstanceState.GetString("txtSourceItem2Text");
				txtSourceValue1Text.Text = savedInstanceState.GetString("txtSourceValue1Text");
				txtSourceValue2Text.Text = savedInstanceState.GetString("txtSourceValue2Text");

				txtDestinationHeaderText.Text = savedInstanceState.GetString("txtDestinationHeaderText");
				txtDestinationHeader2Text.Text = savedInstanceState.GetString("txtDestinationHeader2Text");
				txtDestinationItem1Text.Text = savedInstanceState.GetString("txtDestinationItem1Text");
				txtDestinationItem2Text.Text = savedInstanceState.GetString("txtDestinationItem2Text");
				txtDestinationValue1Text.Text = savedInstanceState.GetString("txtDestinationValue1Text");
				txtDestinationValue2Text.Text = savedInstanceState.GetString("txtDestinationValue2Text");
			}
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                confirmText1 = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "7DFA227D-4839-4513-A4EF-D81BF2C83D47", "Transfer Amount: {0}\nFrom: {1}\n{2}\nTo: {3}\n{4}");
                confirmText2 = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "4A387D12-F90A-465C-A0D6-CD0E76CF7980", "Transfer Amount: {0}\nFrom: {1} To: {2}-{3}");
                confirmTransfer = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "CFBEF5CF-F493-45D7-ABE7-3FF6F11751E3", "Confirm Transfer");
                transferFunds = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "6DFA2873-5A10-4612-A859-8EC161F4D7F0", "Transfer Funds");
                noReview = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "29C17C93-3637-4C9C-A45B-160C3DB2633A", "No, Review");
                transferringFunds = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "BB196ED7-DF44-428C-8033-39F2965BD338", "Transferring funds...");
                transferStatus = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "0988AD6E-2471-4A5C-801F-00E94FA352A2", "Transfer Status");
                transferFailed = CultureTextProvider.GetMobileResourceText("B68A22E1-616C-4D4C-9A0B-68407B24583A", "CF1531D7-4D7C-4AFD-A523-71DE4BAA0E9C", "Transfer failed.");

                CultureTextProvider.SetMobileResourceText(Activity.FindViewById<TextView>(Resource.Id.lblFrom), "B68A22E1-616C-4D4C-9A0B-68407B24583A", "9CE821B3-1E22-4198-A09B-DC477E0E42AF", "Transfer Funds From");
                CultureTextProvider.SetMobileResourceText(Activity.FindViewById<TextView>(Resource.Id.lblTo), "B68A22E1-616C-4D4C-9A0B-68407B24583A", "CB069902-A95D-464A-AF21-A076A9D91567", "Transfer Funds To");
                CultureTextProvider.SetMobileResourceText(Activity.FindViewById<TextView>(Resource.Id.lblAmount), "B68A22E1-616C-4D4C-9A0B-68407B24583A", "9AE82098-98D2-4D0F-B2F2-113E01E00F43", "Amount");
                CultureTextProvider.SetMobileResourceText(btnTransfer, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "28E5A933-BF24-42DE-ADC8-072C1C80D9A9", "Transfer");
                CultureTextProvider.SetMobileResourceText(txtAmount, "B68A22E1-616C-4D4C-9A0B-68407B24583A", "D30B1037-C08C-4F8E-914F-31976FBA2277", "Enter Amount");
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "TransfersFragment:SetCultureConfiguration");
            }
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			txtAmount.AfterTextChanged -= OnTextChanged;

			txtAmount.Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((TextView)sender).Text);
			ValidateTransfer();

			txtAmount.SetSelection(txtAmount.Text.Length);

			txtAmount.AfterTextChanged += OnTextChanged;
		}

		private void ClearAll()
		{
			txtSourceHeaderText.Text = string.Empty;
			txtSourceHeader2Text.Text = string.Empty;
			txtSourceItem1Text.Text = string.Empty;
			txtSourceValue1Text.Text = string.Empty;
			txtSourceItem2Text.Text = string.Empty;
			txtSourceValue2Text.Text = string.Empty;

			txtDestinationHeaderText.Text = string.Empty;
			txtDestinationHeader2Text.Text = string.Empty;
			txtDestinationItem1Text.Text = string.Empty;
			txtDestinationValue1Text.Text = string.Empty;
			txtDestinationItem2Text.Text = string.Empty;
			txtDestinationValue2Text.Text = string.Empty;

			txtAmount.Text = string.Empty;

			btnTransfer.Enabled = false;
		}

		private void SelectAccount(string accountType)
		{
			string excludeSuffix = string.Empty;
			int excludeMemberId = 0;
			AccountListTypes accountListType;

			var intent = new Intent(Activity, typeof(SelectAccountActivity));

            if (accountType == "Source")
			{
				if (_transferExecuteRequest.Destination.Suffix != string.Empty)
				{
					excludeMemberId = _transferExecuteRequest.Destination.MemberId;
					excludeSuffix = _transferExecuteRequest.Destination.Suffix;
				}

				accountListType = AccountListTypes.TransferSourceAccounts;
				intent.PutExtra("ShowAnyMember", false);
			}
			else
			{
				if (_transferExecuteRequest.Source.Suffix != string.Empty)
				{
					excludeMemberId = _transferExecuteRequest.Source.MemberId;
					excludeSuffix = _transferExecuteRequest.Source.Suffix;
				}

				accountListType = AccountListTypes.TransferTargetAccounts;
				intent.PutExtra("ShowAnyMember", true);
			}

			var json = JsonConvert.SerializeObject(accountListType);
			intent.PutExtra("AccountListType", json);
			intent.PutExtra("ExcludeMemberId", excludeMemberId);
			intent.PutExtra("ExcludeSuffix", excludeSuffix);
			intent.PutExtra("ShowJoints", true);

			StartActivityForResult(intent, accountType == "Source" ? 100 : 200);
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
			btnTransfer.Enabled = methods.ValidateTransferRequest(_transferExecuteRequest);
		}

		private async void ConfirmTransfer()
		{
			GeneralUtilities.CloseKeyboard(Activity);

			PopulateTransferRequest();

            var confirmMessage = string.Format(confirmText1,
                StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text),
                txtSourceHeaderText.Text, txtSourceHeader2Text.Text, txtDestinationHeaderText.Text, txtDestinationHeader2Text.Text);

            if (_isFavorite)
            {
                confirmMessage += "\n\nThis transfer is saved in your list of favorites.";
            }

            var response = await AlertMethods.AlertWithCheckBox(Activity, confirmTransfer, confirmMessage, "Save to Favorites", false, _isFavorite, CultureTextProvider.SUBMIT(), CultureTextProvider.NOREVIEW());

            if (response != null && response.Item1 == CultureTextProvider.SUBMIT())
            {
                _saveToFavorites = response.Item2;
                TransferFunds(_transferExecuteRequest);
            }
        }       

		private async void TransferFunds(TransferExecuteRequest request)
		{
			try
			{
				ShowActivityIndicator(transferringFunds);

				var methods = new TransferMethods();
				var response = await methods.Transfer(request, Activity, null);

				HideActivityIndicator();

				ProcessResult(response);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransfersFragment:TransferFunds");
			}
		}

		private async void ProcessResult(MobileStatusResponse response)
		{
			try
			{
				var logMessage = string.Format(confirmText2,
					StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text),
					txtSourceHeader2Text.Text, _transferExecuteRequest.Destination.MemberId, _transferExecuteRequest.Destination.Suffix);

				if (response != null && response.Success && !response.OutOfBandChallengeRequired)
				{
                    if (_saveToFavorites)
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

                            var favoritesResponse = await methods.GetTransferFavorites(null, Activity);

                            if (favoritesResponse != null && favoritesResponse.Result != null)
                            {
                                favoritesResponse.Result.Add(newTransferFavorite);
                                await methods.SetTransferFavorites(favoritesResponse.Result, Activity);
                            }

                            HideActivityIndicator();
                        }
                    }

                    var successMessage = string.Format("Successfully Transferred {0}\nFrom: {1}\n{2}\nTo: {3}\n{4}",
                        StringUtilities.StripInvalidCurrencCharsAndFormat(txtAmount.Text),
                        txtSourceHeaderText.Text, txtSourceHeader2Text.Text, txtDestinationHeaderText.Text, txtDestinationHeader2Text.Text);

                    var alertResponse = await AlertMethods.Alert(Activity, transferStatus, successMessage, "Make Another Transfer", "Go to Account Transactions");					

					ClearAll();			

                    var transactionsFragment = new TransactionsFragment();
                    transactionsFragment.Account = _sourceAccount;

					Logging.Track("Transfer Events", "Transfer Success", logMessage);
					Logging.Track("Transfer executed.");

                    if (alertResponse == "Go to Account Transactions")
                    {
                        NavigationService.NavigatePush(transactionsFragment, false, true);
                    }
				}
				else if ((response != null && !response.Success && !response.OutOfBandChallengeRequired) || response == null)
				{
					await AlertMethods.Alert(Activity, textTransfers, response?.FailureMessage ?? transferFailed, "OK");
					Logging.Track("Transfer Events", "Transfer Failed", string.Format("{0} - {1}", logMessage, response?.FailureMessage ?? "Transfer failed."));
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransfersFragment:ProcessResult");
			}
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == (int)Result.Ok && requestCode == (int)ActivityResults.AccountVerification)
			{
				TransferFunds(_transferExecuteRequest);
			}

            if (resultCode == (int)Result.Ok && requestCode == 400)
            {
                var json = data.GetStringExtra("Favorite");
                var favorite = JsonConvert.DeserializeObject<TransferFavorite>(json);

                if (favorite != null)
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

                    txtSourceHeaderText.Text = favorite.SourceAccountDescription;
                    txtSourceHeader2Text.Text = favorite.Source.MemberId + "-" + favorite.Source.Suffix;
                    txtSourceItem1Text.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "805D4ECB-244C-4E98-B063-CC32706086DA", "Balance") + ":";
                    txtSourceItem2Text.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "30EF1F3D-3F26-4B82-83C6-1A278F302D5A", "Available Balance") + ":";
                    txtSourceValue1Text.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.SourceBalance);
                    txtSourceValue2Text.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.SourceAvailableBalance);

                    if (string.IsNullOrEmpty(favorite.LastName) && string.IsNullOrEmpty(favorite.LastEightOfAtmDebitCard))
                    {
                        txtDestinationHeaderText.Text = favorite.DestinationAccountDescription;
                        txtDestinationHeader2Text.Text = favorite.Destination.MemberId + "-" + favorite.Destination.Suffix;
                        txtDestinationItem1Text.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "805D4ECB-244C-4E98-B063-CC32706086DA", "Balance") + ":";
                        txtDestinationItem2Text.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "30EF1F3D-3F26-4B82-83C6-1A278F302D5A", "Available Balance") + ":";
                        txtDestinationValue1Text.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.DestinationBalance);
                        txtDestinationValue2Text.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.DestinationAvailableBalance);
                    }
                    else
                    {
                        // Any Member
                        txtDestinationHeaderText.Text = (_transferExecuteRequest.Destination.AccountType == "CreditCards" ? "Credit Card" : _transferExecuteRequest.Destination.AccountType) + " Account " + favorite.Destination.MemberId + "-" + favorite.Destination.Suffix;
                        txtDestinationHeader2Text.Text = favorite.LastName;
                        _destinationIsJointAccount = false;
                        txtDestinationItem1Text.Text = string.Empty;
                        txtDestinationItem2Text.Text = string.Empty;
                        txtDestinationValue1Text.Text = string.Empty;
                        txtDestinationValue2Text.Text = string.Empty;
                    }

                    txtAmount.Text = string.Format(new CultureInfo("en-US"), "{0:C}", favorite.Amount);

                    ValidateTransfer();

                    _isFavorite = true;
                }                
            } 

            if (resultCode == (int)Result.Ok && data != null)
            {
                if (requestCode == 100 || requestCode == 200)
                {
					var json = data.GetStringExtra("ListViewItem");

                    if (!string.IsNullOrEmpty(json))
                    {
                        var listViewItem = JsonConvert.DeserializeObject<ListViewItem>(json);
                        json = data.GetStringExtra("Account");
                        var account = JsonConvert.DeserializeObject<SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account>(json);
                        listViewItem.Data = account;

                        if (requestCode == 100)
                        {
                            txtSourceHeaderText.Text = listViewItem.HeaderText;
                            txtSourceHeader2Text.Text = listViewItem.Header2Text;
                            txtSourceItem1Text.Text = listViewItem.Item1Text;
                            txtSourceItem2Text.Text = listViewItem.Item2Text;
                            txtSourceValue1Text.Text = listViewItem.Value1Text;
                            txtSourceValue2Text.Text = listViewItem.Value2Text;
                            _transferExecuteRequest.Source.MemberId = account.MemberId;
                            _transferExecuteRequest.Source.Suffix = account.Suffix;
                            _transferExecuteRequest.Source.AccountType = account.AccountType;
                            _sourceIsJointAccount = listViewItem.Bool1Value;
                            _sourceAccount = account;
                        }
                        else
                        {
                            txtDestinationHeaderText.Text = listViewItem.HeaderText;
                            txtDestinationHeader2Text.Text = listViewItem.Header2Text;
                            txtDestinationItem1Text.Text = listViewItem.Item1Text;
                            txtDestinationItem2Text.Text = listViewItem.Item2Text;
                            txtDestinationValue1Text.Text = listViewItem.Value1Text;
                            txtDestinationValue2Text.Text = listViewItem.Value2Text;
                            _transferExecuteRequest.Destination.MemberId = account.MemberId;
                            _transferExecuteRequest.Destination.Suffix = account.Suffix;
                            _transferExecuteRequest.Destination.AccountType = account.AccountType;
                            _destinationIsJointAccount = listViewItem.Bool1Value;
                            _transferExecuteRequest.LastName = string.Empty;
                        }

                        _transferExecuteRequest.IsJointTransfer = _sourceIsJointAccount || _destinationIsJointAccount;
                        _transferExecuteRequest.LastName = string.Empty;
                    }
					else
					{
						json = data.GetStringExtra("AnyMemberInfo");

						if (!string.IsNullOrEmpty(json))
						{
							var anyMemberInfo = JsonConvert.DeserializeObject<AnyMemberInfo>(json);

							txtDestinationHeaderText.Text = (anyMemberInfo.AccountType == "CreditCards" ? "Credit Card" : anyMemberInfo.AccountType) + " Account " + anyMemberInfo.Account + "-" + anyMemberInfo.Suffix;
							txtDestinationHeader2Text.Text = anyMemberInfo.LastName;
							_transferExecuteRequest.Destination.Suffix = anyMemberInfo.Suffix;
							_transferExecuteRequest.Destination.AccountType = anyMemberInfo.AccountType;

							int memberId;
							if (int.TryParse(anyMemberInfo.Account, out memberId))
							{
								_transferExecuteRequest.Destination.MemberId = memberId;
							}

							_transferExecuteRequest.IsJointTransfer = anyMemberInfo.IsJoint;
							_transferExecuteRequest.LastName = anyMemberInfo.LastName;
							_destinationIsJointAccount = false;

							_transferExecuteRequest.IsJointTransfer = _sourceIsJointAccount || _destinationIsJointAccount;
						}
					}

					ValidateTransfer();
                }
            }
		}
	}
}