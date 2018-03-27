using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Extensions;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Droid.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts
{
    public class TransactionsFragment : BaseListFragment, AbsListView.IOnScrollListener
	{
        public Account Account { get; set; }

        private bool _isCreditCard;
		private bool _isLoading;
		private string _searchText;
		private int _previousTotalCount;
        private TransactionDisputeRestrictions _transactionDisputeRestrictions;
		private TransactionListTextViewModel _transactionsViewModel;
		private NextTransactionResponse _loadOnDemandViewModel;
		private TransactionsListViewAdapter _listAdapter;
		private List<ListViewItem> _listViewItems = new List<ListViewItem>();

        private TextView accountTitle;
        private SearchView searchView;
        private SwipeRefreshLayout refresher;
        private TextView txtAccountNumber;
        private TextView txtAccountNumberForACH;
        private TextView txtBalance;
        private TextView txtAvailableBalance;
        private TextView txtMinimumPayment;
        private TextView txtNextDueDate;
        private TextView txtRate;
        private ImageView btnExpand;
        private Button btnPayoff;
        private TableLayout accountDetailsTable;
        private TableRow rowMinimumPayment;
        private TableRow rowNextDueDate;
        private TableRow rowRate;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AccountTransactionListView, null);
			RetainInstance = true;
			
   			_isCreditCard = Account.Suffix.Length > 5;
			_searchText = string.Empty;

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("Transactions");
				_transactionsViewModel = JsonConvert.DeserializeObject<TransactionListTextViewModel>(json);

				json = savedInstanceState.GetString("LoadOnDemandTransactions");
				_loadOnDemandViewModel = JsonConvert.DeserializeObject<NextTransactionResponse>(json);
			}
			else
			{
				_loadOnDemandViewModel = new NextTransactionResponse { Transactions = new List<Transaction>(), MoreData = true };
			}

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			string json = JsonConvert.SerializeObject(_transactionsViewModel);
			outState.PutString("Transactions", json);

			json = JsonConvert.SerializeObject(_loadOnDemandViewModel);
			outState.PutString("LoadOnDemandTransactions", json);

			base.OnSaveInstanceState(outState);
		}

		public override async void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

            var accountMethods = new AccountMethods();

			refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.transactionsRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += RefreshTransactions;
                      

			accountTitle = Activity.FindViewById<TextView>(Resource.Id.transactionTitleText);
            accountTitle.Text = accountMethods.GetAccountDescription(Account);

            accountDetailsTable = Activity.FindViewById<TableLayout>(Resource.Id.tableLayout);
            accountDetailsTable.Visibility = ViewStates.Gone;

            txtAccountNumber = Activity.FindViewById<TextView>(Resource.Id.lblAccountNumber);
            txtAccountNumberForACH = Activity.FindViewById<TextView>(Resource.Id.lblAccountNumberACH);
            txtBalance = Activity.FindViewById<TextView>(Resource.Id.lblBalance);
            txtAvailableBalance=Activity.FindViewById<TextView>(Resource.Id.lblAvailableBalance);
            txtMinimumPayment =Activity.FindViewById<TextView>(Resource.Id.lblMinimumPayment);
            txtNextDueDate = Activity.FindViewById<TextView>(Resource.Id.lblNextDueDate);
            txtRate = Activity.FindViewById<TextView>(Resource.Id.lblRate);

            rowMinimumPayment = Activity.FindViewById<TableRow>(Resource.Id.rowMinimumPayment);
            rowMinimumPayment.Visibility = ViewStates.Gone;
            rowNextDueDate = Activity.FindViewById<TableRow>(Resource.Id.rowNextDueDate);
            rowNextDueDate.Visibility = ViewStates.Gone;
            rowRate = Activity.FindViewById<TableRow>(Resource.Id.rowRate);
            rowRate.Visibility = ViewStates.Gone;
            btnPayoff = Activity.FindViewById<Button>(Resource.Id.btnPayoff);
            btnPayoff.Visibility = ViewStates.Gone;
            btnPayoff.Click += (sender, e) => 
            {
                var payoffQuoteFragment = new PayoffQuoteFragment { Account = Account };
                NavigationService.NavigatePush(payoffQuoteFragment, true, false, true);
            };

            btnExpand = Activity.FindViewById<ImageView>(Resource.Id.btnExpand);
                                
            btnExpand.Click += (sender, e) => 
            {
                if (accountDetailsTable.Visibility == ViewStates.Gone)
                {
                    btnExpand.SetImageResource(Resource.Drawable.expand_up);
                    accountDetailsTable.Visibility = ViewStates.Visible;

                    if (Account.AccountCategory == "CreditCards" || Account.AccountCategory == "Loans")
                    {
                        rowMinimumPayment.Visibility = ViewStates.Visible;
                        rowNextDueDate.Visibility = ViewStates.Visible;
                        rowRate.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    btnExpand.SetImageResource(Resource.Drawable.expand_down);
                    accountDetailsTable.Visibility = ViewStates.Gone;

                    if (_isCreditCard)
                    {
                        rowMinimumPayment.Visibility = ViewStates.Gone;
                        rowNextDueDate.Visibility = ViewStates.Gone;
                        rowRate.Visibility = ViewStates.Gone;
                    }
                }
            };

			searchView = Activity.FindViewById<SearchView>(Resource.Id.searchBar);
			searchView.QueryTextChange += async (sender, e) =>
			{
				if (_searchText != e.NewText)
				{
					_searchText = e.NewText;

                    switch (Account.AccountCategory.Substring(0, 1).ToUpper())
					{
						case "S":
						case "C":
							await ReloadData(_loadOnDemandViewModel, e.NewText);
							break;
						default:
							ReloadData(_transactionsViewModel, e.NewText);
							break;
					}

					searchView.RequestFocus();
				}
			};

			ListViewMain.ItemClick += (sender, e) =>
			{
				try
				{
					var item = _listAdapter.GetListViewItem(e.Position);
					var transactionDetailFragment = new TransactionDetailFragment();
					transactionDetailFragment.SelectedTransaction = (Transaction)item.Data;
                    transactionDetailFragment.SelectedMemberId = Account.MemberId.ToString();
					transactionDetailFragment.IsCreditCard = _isCreditCard;
					transactionDetailFragment.ViewCheckSelected += ViewCheck;
					transactionDetailFragment.DisputeSelected += Dispute;
                    transactionDetailFragment.DisputeRestrictions = _transactionDisputeRestrictions;

					NavigationService.NavigatePush(transactionDetailFragment, true, false);
				}
				catch (Exception ex)
				{
					Logging.Log(ex, "TransactionsFragment:ItemClick");
				}
			};

			ListViewMain.SetOnScrollListener(this);			

			ListViewMain.RequestFocus();

            ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "9FC1A125-A95E-4E49-94D1-9E31908E7DDF", "Loading transactions..."));

            PopulateAccountDetails();

			await LoadTransactions();

            HideActivityIndicator();

            await GetDisputerInfo();
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
                        txtRate.Text = string.Empty;
                    }
                    else
                    {
                        txtRate.Text = Account.InterestRate.ToString();

                        if (Account.PayoffEligibility != null && Account.PayoffEligibility.IsPayoffEligible)
                        {
                            btnPayoff.Visibility = ViewStates.Visible;
                        }
                    }
                }
                else
                {                 
                    txtNextDueDate.Text = string.Empty;
                    txtMinimumPayment.Text = string.Empty;
                    txtRate.Text = string.Empty;
                }

                txtAccountNumber.Text = accountTitle.Text;
                txtAccountNumberForACH.Text = Account.MicrAccountNumber;
                txtBalance.Text = string.Format(new CultureInfo("en-US"), "{0:C}", Account.Balance);
                txtAvailableBalance.Text = string.Format(new CultureInfo("en-US"), "{0:C}", Account.AvailableBalance);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "TransactionsFragment:PopulateAccountDetails");
            }
        }

		public override void OnResume()
		{
			base.OnResume();
		}

        public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "553F5A57-27E8-42B8-AEE9-17368A95FC1B", "Transactions"));
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "TransactionsFragment:SetCultureConfiguration");
            }
		}

		private void ViewCheck(ListViewItem item)
		{
			try
			{
				if (item.MoreIconVisible)
				{
					var transaction = (Transaction)item.Data;

					if ((!string.IsNullOrEmpty(transaction.CheckNumber)) || (!string.IsNullOrEmpty(transaction.TraceNumber)))
					{
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

						var checkImageFragment = new CheckImageFragment
						{
							ImageRequest = imageRequest
						};

						NavigationService.NavigatePush(checkImageFragment, true, false);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsFragment:ViewCheck");
			}
		}

        private async Task GetDisputerInfo()
        {
            var accountMethods = new AccountMethods();
            var memberRequest = new MemberInformationRequest { MemberId = Account.MemberId };

            ShowActivityIndicator();

            var response = await accountMethods.GetMemberInformation(memberRequest, Activity);

            HideActivityIndicator();

            if (response?.TransactionDisputeRestrictions != null)
            {
                _transactionDisputeRestrictions = response.TransactionDisputeRestrictions;
            }
        }

		private async void Dispute(ListViewItem item)
		{
			try
			{
				var transaction = (Transaction)item.Data;
				var methods = new AccountMethods();

				ShowActivityIndicator();

                var alreadyDisputed = await methods.HasTransactionAlreadyBeenDisputed(transaction, Account.MemberId, this);

				HideActivityIndicator();

				if (!alreadyDisputed)
				{
					ShowActivityIndicator();

					var response = await methods.GetTransactionDisputeInformation(null, Activity);
                    var disputeType = methods.GetDisputeInfo(transaction, Account.MemberId.ToString(), Account.Suffix.Length > 5).DisputeType;

					HideActivityIndicator();

					var msgAreYouSureDefault = "Are you sure you want to submit a dispute?\nYou have selected to dispute the following transaction:\n\nTransaction Date\n{0:MM/dd/yyyy}\n\nTransaction Description\n{1}\n\nTransaction Amount\n${2}\n\n";
					var msgAreYouSure = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "3477A0EF-0DBB-4351-A5C9-57775E3725FC", msgAreYouSureDefault);
					var alertMessage = String.Format(msgAreYouSure, transaction.TransactionDate, transaction.Description, transaction.TransactionAmount);
					var responseContinue = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "8775F965-56D5-4116-B089-F97B34724A97", "Continue");
					var responseCancel = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "65696E69-45D5-418A-82A8-3D2755582498", "Cancel");
					var responseReportCard = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "A95405EA-C555-4B72-8854-CD3C11DB7FFC", "Report Lost/Stolen Card");
					var alertResponseOptions = new string[] { responseContinue, responseCancel };

					if (response?.Result != null)
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
					var alertResponse = await AlertMethods.Alert(Activity, alertTitle, alertMessage, alertResponseOptions);

					if (alertResponse == responseContinue)
					{
						var fragment = new TransactionDisputeFragment();
						fragment.SelectedTransaction = transaction;
                        fragment.SelectedMemberId = Account.MemberId.ToString();
                        fragment.SelectedAccountDescription = accountTitle.Text;
						fragment.SelectedSuffix = Account.Suffix;
						NavigationService.NavigatePush(fragment, true, false);
					}
					else if (alertResponse == responseReportCard)
					{
						var intent = new Intent(Activity, typeof(WebViewActivity));
						intent.PutExtra("Title", "Report Lost/Stolen Card");
						intent.PutExtra("Url", response?.Result?.ReportLostStolenUrl != null ? response.Result.ReportLostStolenUrl : string.Empty);
						StartActivity(intent);
					}
				}
				else
				{
					ListAdapter = _listAdapter;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsFragment:Dispute");
			}
		}

		private async void RefreshTransactions(object sender, EventArgs e)
		{
			// Clear Search
			searchView.SetQuery(string.Empty, false);
			_searchText = string.Empty;

            ListViewMain.Adapter = null;
            _listAdapter = null;
			_transactionsViewModel = null;
			_loadOnDemandViewModel = new NextTransactionResponse { Transactions = new List<Transaction>(), MoreData = true };

			await LoadTransactions();
		}

		private async Task<bool> LoadTransactions(int row = 0)
		{
			try
			{
				if (!_isLoading)
				{
					_isLoading = true;					

					var methods = new AccountMethods();

					if (!refresher.Refreshing)
					{
						//ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "9FC1A125-A95E-4E49-94D1-9E31908E7DDF", "Loading transactions..."));
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

								var loadOnDemandResponse = await methods.NextAccountTransactionList(loadOnDemandRequest, Activity);

								if (loadOnDemandResponse != null && loadOnDemandResponse.Transactions != null)
								{
									_loadOnDemandViewModel.Transactions.AddRange(loadOnDemandResponse.Transactions);
									_loadOnDemandViewModel.LastTransactionDate = loadOnDemandResponse.LastTransactionDate;
									_loadOnDemandViewModel.LastTransactionIdendtifier = loadOnDemandResponse.LastTransactionIdendtifier;
									_loadOnDemandViewModel.MoreData = loadOnDemandResponse.MoreData;									

									await ReloadData(_loadOnDemandViewModel, _searchText);
								}
							}
							break;
						default:
							if (_transactionsViewModel == null)
							{
								var request = new SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts.AccountTransactionListRequest
                                {
                                    MemberId = Account.MemberId.ToString(),
                                    Suffix = Account.AccountCategory.Substring(0, 1) + Account.Suffix,
									StartDate = DateTime.Now.AddDays(-90),
									EndDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59)
								};

								_transactionsViewModel = await methods.AccountTransactionList(request, Activity);

								if (_transactionsViewModel != null && _transactionsViewModel.ClientViewState != null && _transactionsViewModel.ClientViewState == "TransactionList")
								{
									ReloadData(_transactionsViewModel, null);
								}
							}
							break;
					}

					if (!refresher.Refreshing)
					{
						//HideActivityIndicator();
					}
					else
					{
						refresher.Refreshing = false;
					}

					searchView.ClearFocus();

					if (row != 0)
					{
						//ListViewMain.SetSelection(currentPosition);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsFragment:LoadTransactions");
			}
			finally
			{
				_isLoading = false;

				//HideActivityIndicator();
			}

			return true;
		}

		private async Task<bool> ReloadData(NextTransactionResponse model, string searchText)
		{
			if (model != null && model.Transactions != null)
			{
				_listViewItems = ViewUtilities.ConvertTransactionsToListViewItems(model.Transactions, _isCreditCard, searchText);

				// If we don't find anything in our search, we need to load more transactions
				while (_listViewItems.Count == 0 && model.MoreData)
				{
					await LoadTransactions(0);
					_listViewItems = ViewUtilities.ConvertTransactionsToListViewItems(model.Transactions, _isCreditCard, searchText);
				}				

                if (_listAdapter == null)
                {
					int[] resourceIds = { Resource.Id.lblItemTitle, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text, Resource.Id.lblItem3Text };
					string[] fields = { "HeaderText", "Item1Text", "Value1Text", "Value2Text" };
                    _listAdapter = new TransactionsListViewAdapter(Activity, Resource.Layout.AccountTransactionListViewItem, _listViewItems, resourceIds, fields, Account.MemberId.ToString());
                    ListAdapter = _listAdapter;
                    _listAdapter.ViewCheckSelected += ViewCheck;
                    _listAdapter.DisputeSelected += Dispute;
                }
                else
                {
					var currentPosition = ListViewMain.FirstVisiblePosition;
					var view = ListView.GetChildAt(0);
					int top = (view == null) ? 0 : view.Top;

                    _listAdapter.ReplaceItems(_listViewItems);

                    ListViewMain.SetSelectionFromTop(currentPosition, top);
                    //_isNotifyChangeCount = 3;
                    //_listAdapter.NotifyDataSetChanged();
                }
			}

			return true;
		}

		private void ReloadData(TransactionListTextViewModel model, string searchText)
		{
			if (model != null)
			{
				_listViewItems = ViewUtilities.ConvertTextViewModelToListViewItems(model, _isCreditCard, searchText);

				int[] resourceIds = { Resource.Id.lblItemTitle, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text, Resource.Id.lblItem3Text };
				string[] fields = { "HeaderText", "Item1Text", "Value1Text", "Value2Text" };
                _listAdapter = new TransactionsListViewAdapter(Activity, Resource.Layout.AccountTransactionListViewItem, _listViewItems, resourceIds, fields, Account.MemberId.ToString());
				ListAdapter = _listAdapter;

				_listAdapter.ViewCheckSelected += ViewCheck;
				_listAdapter.DisputeSelected += Dispute;
			}
		}

		public async void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
		{
            try
            {
                if (!_isLoading)
                {
                    var focusedView = Activity.Window.CurrentFocus;

                    if (focusedView == null || focusedView is AutoCompleteTextView)
                    {
                        return;
                    }

                    if (totalItemCount == 0 || _listAdapter == null)
                    {
                        return;
                    }

                    if (_previousTotalCount == totalItemCount)
                    {
                        return;
                    }

                    var loadMore = firstVisibleItem + visibleItemCount >= (totalItemCount - 6);  // Load more when they are 6 rows away from the bottom.

                    if (loadMore)
                    {
                        _previousTotalCount = totalItemCount;
                        await LoadTransactions(ListViewMain.FirstVisiblePosition);
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "TransactionsFragment:OnScroll");
            }
        }

		public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
		{
		}
	}
}