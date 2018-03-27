using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public partial class TransactionsViewController : BaseViewController
	{			
        public SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account Account { get; set; }

		private TransactionListTextViewModel _transactionsViewModel;
		private NextTransactionResponse _loadOnDemandViewModel;
		private UISearchBar _searchBar;
		private UIRefreshControl _refreshControl;
		private bool _isCreditCard;
		private bool _isLoading;
		private string _searchText;
		private int _lastRowCount;
        private int _accountInfoLength;
        private TransactionDisputeRestrictions _disputeRestrictions;

		public TransactionsViewController(IntPtr handle) : base(handle)
		{
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			_refreshControl = new UIRefreshControl();
			transactionTableView.AddSubview(_refreshControl);

			_refreshControl.ValueChanged += RefreshTransactions;

			_isCreditCard = Account.Suffix.Length > 5;

            var accountMethods = new AccountMethods();
			lblHeader.BackgroundColor = AppStyles.BarTintColor;
            lblHeader.Text = accountMethods.GetAccountDescription(Account);

            var tapGesture = new UITapGestureRecognizer();
            tapGesture.AddTarget(() => ShowAccountInfo());
            imgExpand.AddGestureRecognizer(tapGesture);

			// Configure the search bar
			_searchBar = new UISearchBar(new CGRect(0, 0, 320, 44))
			{
				ShowsScopeBar = false
			};

			_searchBar.TextChanged += async (sender, e) =>
			{
				_searchText = e.SearchText;

                await LoadOnDemandTransactions(15);

				_searchBar.BecomeFirstResponder();
			};

			_searchBar.SearchButtonClicked += (sender, e) => GeneralUtilities.CloseKeyboard(View);

			transactionTableView.TableHeaderView = _searchBar;

			_loadOnDemandViewModel = new NextTransactionResponse { Transactions = new List<Transaction>(), MoreData = true };			

            ClearAll();

            PopulateAccountDetails();

            ShowActivityIndicator();

			await LoadOnDemandTransactions();

            HideActivityIndicator();

            await GetDisputerInfo();
		}

        private void ShowAccountInfo()
        {
            if (viewPayoffInfo.Hidden)
            {
                OpenAccountInfo();
            }
            else
            {
                CloseAccountInfo();
            }
        }

        private void ClearAll()
        {
            txtAccountNumber.Text = string.Empty;
            txtAccountNumberForACH.Text = string.Empty;
            txtBalance.Text = string.Empty;
            txtAvailableBalance.Text = string.Empty;
            txtMinimumPayment.Text = string.Empty;
            txtNextDueDate.Text = string.Empty;
            txtRate.Text = string.Empty;
            lblNextDueDate.Text = "Next Due Date:";
            lblMinimumPayment.Text = "Minimum Payment:";
            lblRate.Text = "Rate:";
            btnRequestPayoff.Hidden = true;
            _accountInfoLength = 0;
            transactionTableView.Frame = new CGRect(lblHeader.Frame.Left, lblHeader.Frame.Bottom, lblHeader.Frame.Width, View.Bounds.Height - lblHeader.Frame.Height);
            viewPayoffInfo.Hidden = true;
            imgExpand.Image = UIImage.FromBundle("expand_down.png");
        }

        private async Task GetDisputerInfo()
        {
            var accountMethods = new AccountMethods();
            var memberRequest = new MemberInformationRequest { MemberId = Account.MemberId };

            var response = await accountMethods.GetMemberInformation(memberRequest, View);
                    
            if (response?.TransactionDisputeRestrictions != null)
            {
                _disputeRestrictions = response.TransactionDisputeRestrictions;
            }
        }

        private void PopulateAccountDetails()
        {
            try
            {
                if (Account.AccountCategory == "CreditCards" || Account.AccountCategory == "Loans")
                {
                    txtNextDueDate.Text = string.Format("{0:MM/dd/yyyy}", Account.PaymentDueDate.UtcToEastern());
                    txtMinimumPayment.Text = string.Format(new CultureInfo("en-US"), "{0:C}", Account.PaymentAmount);    

                    if (Account.InterestRate == 0)
                    {                    
                        lblRate.Text = string.Empty;
                        _accountInfoLength = 1;
                    }
                    else
                    {
                        txtRate.Text = Account.InterestRate.ToString();
                        _accountInfoLength = 2;

                        if (Account.PayoffEligibility != null && Account.PayoffEligibility.IsPayoffEligible)
                        {
                            _accountInfoLength = 3;

                            btnRequestPayoff.Hidden = false;
                            btnRequestPayoff.TouchUpInside += (sender, e) =>
                            {
                                var controller = AppDelegate.StoryBoard.InstantiateViewController("PayoffViewController") as PayoffViewController;
                                controller.Account = Account;

                                NavigationController.PushViewController(controller, true);
                            };
                        }
                    }
                }
                else
                {                 
                    lblNextDueDate.Text = string.Empty;
                    lblMinimumPayment.Text = string.Empty;
                    lblRate.Text = string.Empty;
                }

                txtAccountNumber.Text = lblHeader.Text;
                txtAccountNumberForACH.Text = Account.MicrAccountNumber;
                txtBalance.Text = string.Format(new CultureInfo("en-US"), "{0:C}", Account.Balance);
                txtAvailableBalance.Text = string.Format(new CultureInfo("en-US"), "{0:C}", Account.AvailableBalance);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TransactionsViewControler:PopulateAccountDetails");
            }
        }

        public override void SetCultureConfiguration()
		{
            Title = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "553F5A57-27E8-42B8-AEE9-17368A95FC1B", "Transactions");
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			GeneralUtilities.CloseKeyboard(View);
		}

		private void SetTableViewSource(TransactionsTableViewSource tableViewSource)
		{
			tableViewSource.ItemSelected += ViewTransactionDetails;
			tableViewSource.ViewCheckSelected += ViewCheck;
			tableViewSource.DisputeSelected += Dispute;
			tableViewSource.ScrolledToBottom += (obj) => ScrolledToBottom(obj);
			transactionTableView.Source = tableViewSource;
			transactionTableView.ReloadData();
		}

		private async void ScrolledToBottom(int row)
		{
			if (row != _lastRowCount)
			{
				_lastRowCount = row;
				await LoadOnDemandTransactions();
			}
		}

		private void ViewTransactionDetails(ListViewItem item)
		{
			try
			{
				if (item != null)
				{
					var controller = AppDelegate.StoryBoard.InstantiateViewController("TransactionDetailsTableViewController") as TransactionDetailsTableViewController;
					controller.SelectedTransaction = (Transaction)item.Data;
                    controller.SelectedMemberId = Account.MemberId.ToString();
					controller.IsCreditCard = _isCreditCard;
					controller.ViewCheckSelected += ViewCheck;
					controller.DisputeSelected += Dispute;
                    controller.DisputRestrictions = _disputeRestrictions;

					NavigationController.PushViewController(controller, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsViewControler:ViewCheck");
			}
		}

		private void ViewCheck(ListViewItem item)
		{
			try
			{
				if (item != null && item.MoreIconVisible)
				{
					var transaction = (Transaction)item.Data;
					var controller = AppDelegate.StoryBoard.InstantiateViewController("CheckImageViewController") as CheckImageViewController;

					var imageRequest = new CheckImagesRequest();
					int checkNumber;
					int.TryParse(transaction.CheckNumber, out checkNumber);
					imageRequest.CheckNumber = checkNumber;
					imageRequest.RemoteDepositTraceNumber = (!string.IsNullOrEmpty(transaction.TraceNumber)) ? transaction.TraceNumber : "0";					
                    imageRequest.MemberId = Account.MemberId;
                    imageRequest.Suffix = Account.AccountCategory.Substring(0, 1) + Account.Suffix;
					imageRequest.ReturnAllChecks = true;
					imageRequest.ExcludeCheckImages = false;

					if (!string.IsNullOrEmpty(imageRequest.Suffix) && char.IsNumber(imageRequest.Suffix[0]))
					{
						imageRequest.Suffix = imageRequest.Suffix.Substring(1);
					}

					controller.ImageRequest = imageRequest;
					NavigationController.PushViewController(controller, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsViewControler:ViewCheck");
			}
		}

		private async void Dispute(ListViewItem item)
		{
			try
			{
				if (item != null)
				{
					var transaction = (Transaction)item.Data;
					var methods = new AccountMethods();

					ShowActivityIndicator();

                    var alreadyDisputed = await methods.HasTransactionAlreadyBeenDisputed(transaction, Account.MemberId, this);

					HideActivityIndicator();

					if (!alreadyDisputed)
					{
						ShowActivityIndicator();

						var response = await methods.GetTransactionDisputeInformation(null, View);
                        var disputeType = methods.GetDisputeInfo(transaction, Account.MemberId.ToString(), Account.Suffix.Length > 5).DisputeType;

						HideActivityIndicator();

						var msgAreYouSureDefault = "Are you sure you want to submit a dispute?\nYou have selected to dispute the following transaction:\n\nTransaction Date\n{0:MM/dd/yyyy}\n\nTransaction Description\n{1}\n\nTransaction Amount\n${2}\n\n";
						var msgAreYouSure = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "3477A0EF-0DBB-4351-A5C9-57775E3725FC", msgAreYouSureDefault);
						var alertMessage = string.Format(msgAreYouSure, transaction.TransactionDate, transaction.Description, transaction.TransactionAmount);
						var responseContinue = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "8775F965-56D5-4116-B089-F97B34724A97", "Continue");
						var responseCancel = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "65696E69-45D5-418A-82A8-3D2755582498", "Cancel");
						var responseReportCard = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "A95405EA-C555-4B72-8854-CD3C11DB7FFC", "Report Lost/Stolen Card");
						var alertResponseOptions = new string[] { responseContinue, responseCancel };

						if (response?.Result.DisputeInstructions != null)
						{
							if (disputeType.ToUpper() == "ACH")
							{
								alertMessage += response.Result.AchDisputeInstructions.Replace("\\n", "\n");
							}
							else if (disputeType.ToUpper() == "ATM")
							{
								alertMessage += response.Result.AtmDisputeInstructions.Replace("\\n", "\n");
							}
							else
							{
								alertMessage += response.Result.DisputeInstructions.Replace("\\n", "\n");
								alertResponseOptions = new string[] { responseContinue, responseCancel, responseReportCard };
							}
						}

						var alertTitle = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "E538481D-2295-497A-A7E3-B3963A1E4615", "Transaction Dispute");
						var alertResponse = await AlertMethods.Alert(View, alertTitle, alertMessage, alertResponseOptions);

						if (alertResponse == responseContinue)
						{
							var controller = AppDelegate.StoryBoard.InstantiateViewController("TransactionDisputesTableViewController") as TransactionDisputesTableViewController;
							controller.SelectedTransaction = transaction;
                            controller.SelectedMemberId = Account.MemberId.ToString();
							controller.SelectedSuffix = Account.Suffix;
                            controller.AccountDescription = lblHeader.Text;
							NavigationController.PushViewController(controller, true);
						}
						else if (alertResponse == responseReportCard)
						{
							var websiteViewController = AppDelegate.StoryBoard.InstantiateViewController("WebViewController") as WebViewController;
							websiteViewController.Title = responseReportCard;
							websiteViewController.Url = response?.Result?.ReportLostStolenUrl != null ? response.Result.ReportLostStolenUrl : string.Empty;
							NavigationController.PushViewController(websiteViewController, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsViewControler:Dispute");
			}
		}

		private async void RefreshTransactions(object sender, EventArgs e)
		{
            if (!_isLoading)
            {
                // Clear the table
                var tableViewSource = new TransactionsTableViewSource(new TextViewTableSource { Items = new List<ListViewItem>() }, Account.MemberId.ToString());
                transactionTableView.Source = tableViewSource;
                transactionTableView.ReloadData();

                // Clear Search
                _searchBar.Text = string.Empty;
                _searchText = string.Empty;

                _loadOnDemandViewModel = new NextTransactionResponse { Transactions = new List<Transaction>(), MoreData = true };
                _transactionsViewModel = null;

                await LoadOnDemandTransactions();
            }
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			transactionTableView.ReloadData();
		}

		private async Task LoadOnDemandTransactions(int minTransactions = 0)
		{
			var textViewTableSource = new TextViewTableSource { Items = new List<ListViewItem>() };

			if (string.IsNullOrEmpty(_searchText))
			{
				await LoadTransactions();
			}

            switch (Account.AccountCategory.Substring(0, 1).ToUpper())
			{
				case "S":
				case "C":
					textViewTableSource = ViewUtilities.ConvertTransactionsToTextViewTableSource(_loadOnDemandViewModel.Transactions, _isCreditCard, _searchText);

					// We need to make sure that we have at least one transaction coming back.
					while (textViewTableSource.Items.Count <= minTransactions && _loadOnDemandViewModel.MoreData)
					{
						await LoadTransactions(false);
						textViewTableSource = ViewUtilities.ConvertTransactionsToTextViewTableSource(_loadOnDemandViewModel.Transactions, _isCreditCard, _searchText);
					}					
					break;
                default:
                    textViewTableSource = ViewUtilities.ConvertTextViewModelToTextViewTableSource(_transactionsViewModel, _isCreditCard, _searchText);
                    break;
			}

            var tableViewSource = new TransactionsTableViewSource(textViewTableSource, Account.MemberId.ToString());
            SetTableViewSource(tableViewSource);
		}

        private void OpenAccountInfo()
        {
            viewPayoffInfo.Hidden = false;
            int heightOffset = 0;

            switch (_accountInfoLength)
            {
                case 0:
                    heightOffset = 84;
                    break;
                case 1:
                    heightOffset = 120;
                    break;
                case 2:
                    heightOffset = 138;
                    break;
                case 3:
                    heightOffset = 168;
                    break;
            }

            UIView.Animate(.25f, 0f, UIViewAnimationOptions.CurveLinear, () => { transactionTableView.Frame = new CGRect(lblHeader.Frame.Left, lblHeader.Frame.Bottom + heightOffset, View.Frame.Width, View.Bounds.Height - lblHeader.Frame.Height - heightOffset); }, () => { imgExpand.Image = UIImage.FromBundle("expand_up.png"); });
        }

        private void CloseAccountInfo()
        {
            UIView.Animate(.25f, 0f, UIViewAnimationOptions.CurveLinear, () => { transactionTableView.Frame = new CGRect(lblHeader.Frame.Left, lblHeader.Frame.Bottom, lblHeader.Frame.Width, View.Bounds.Height - lblHeader.Frame.Height - 44);}, () => { viewPayoffInfo.Hidden = true; imgExpand.Image = UIImage.FromBundle("expand_down.png");});
        }

		private async Task<bool> LoadTransactions(bool setTableViewSource = true)
		{
			try
			{
				if (!_isLoading)
				{
					_isLoading = true;

					var methods = new AccountMethods();

					if (!_refreshControl.Refreshing)
					{
						//ShowActivityIndicator();
					}

                    switch (Account.AccountCategory.Substring(0, 1).ToUpper())
					{
						case "S":
						case "C":
							if (_loadOnDemandViewModel.MoreData)
							{
								var loadOnDemandRequest = new NextTransactionRequest
								{
                                    UserName = Account.MemberId.ToString().PadLeft(10, '0'),
									Suffix = Account.Suffix,
									LastTransactionDate = _loadOnDemandViewModel.LastTransactionDate,
									LastTransactionIdentifier = _loadOnDemandViewModel.LastTransactionIdendtifier,
									TransactionAccountType = _isCreditCard ? TransactionAccountTypes.CreditCard.ToString() : TransactionAccountTypes.Share.ToString()
								};

								var loadOnDemandResponse = await methods.NextAccountTransactionList(loadOnDemandRequest, View);

								if (loadOnDemandResponse != null && loadOnDemandResponse.Transactions != null)
								{
									_loadOnDemandViewModel.Transactions.AddRange(loadOnDemandResponse.Transactions);
									_loadOnDemandViewModel.LastTransactionDate = loadOnDemandResponse.LastTransactionDate;
									_loadOnDemandViewModel.LastTransactionIdendtifier = loadOnDemandResponse.LastTransactionIdendtifier;
									_loadOnDemandViewModel.MoreData = loadOnDemandResponse.MoreData;

									if (setTableViewSource)
									{
										var textViewTableSource = ViewUtilities.ConvertTransactionsToTextViewTableSource(_loadOnDemandViewModel.Transactions, _isCreditCard, _searchText);
                                        var tableViewSource = new TransactionsTableViewSource(textViewTableSource, Account.MemberId.ToString());
										SetTableViewSource(tableViewSource);
									}
								}
                                else
                                {
                                    // We received an invalid response.  Turn off the loading.
                                    var tableViewSource = new TextViewTableSource();
                                    tableViewSource.Items = new List<ListViewItem>();
									transactionTableView.ReloadData();
                                }
							}
							break;
						default:
							if (_transactionsViewModel == null)
							{
								var request = new AccountTransactionListRequest
								{
                                    MemberId = Account.MemberId.ToString(),
                                    Suffix = Account.AccountCategory.Substring(0, 1) + Account.Suffix,						
									StartDate = DateTime.Now.AddDays(-90),
									EndDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59)
								};

								_transactionsViewModel = await methods.AccountTransactionList(request, View);

								if (_transactionsViewModel != null && _transactionsViewModel.ClientViewState != null && _transactionsViewModel.ClientViewState == "TransactionList")
								{
									if (setTableViewSource)
									{
										var textViewTableSource = ViewUtilities.ConvertTextViewModelToTextViewTableSource(_transactionsViewModel, _isCreditCard, _searchText);
                                        var tableViewSource = new TransactionsTableViewSource(textViewTableSource, Account.MemberId.ToString());
										SetTableViewSource(tableViewSource);
									}
								}
							}
							break;
					}

					if (!_refreshControl.Refreshing)
					{
						//HideActivityIndicator();
					}
					else
					{
						_refreshControl.EndRefreshing();
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsViewController:LoadTransactions");
			}
			finally
			{
				_isLoading = false;
			}

			return true;
		}
	}
}