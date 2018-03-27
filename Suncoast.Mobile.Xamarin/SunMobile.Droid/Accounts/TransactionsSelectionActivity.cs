using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.Droid.Common;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "TransactionsSelectionActivity", Theme = "@style/CustomHoloLightTheme")]
	public class TransactionsSelectionActivity : BaseListActivity
	{
		private TextView txtTitle;
		private ImageButton btnCloseWindow;
		private TextView txtDone;
		private TextView accountTitle;
		private ImageButton btnPrevious;
		private ImageButton btnNext;
		private TextView txtRangeDisplay;
		private SearchView searchView;
		private SwipeRefreshLayout refresher;

		// Options
		private Transaction DisputedTransaction;
		private string AccountDescription;
		private string Suffix;
		private string MemberId;
		private int MaxSelectedTransactions;
		private bool LimitToDisputedMerchant;

		private int _month;
		private int _year;
		private bool _isCreditCard;

		private TransactionListTextViewModel _transactionsViewModel;
		private MobileStatusResponse<List<TransactionDisputeHistoryItem>> _transactionDispluteHistoryViewModel;
		private TransactionsSelectionListViewAdapter _listAdapter;
		private List<ListViewItem> _listViewItems = new List<ListViewItem>();
		private List<Transaction> _transactions = new List<Transaction>();
		private const string MAX_TRANSACTIONS_MESSAGE = "You have selected the maximum amount of transactions";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("DisputedTransaction");
				if (json != null)
				{
					DisputedTransaction = JsonConvert.DeserializeObject<Transaction>(json);
				}
				json = savedInstanceState.GetString("Transactions");
				_transactionsViewModel = JsonConvert.DeserializeObject<TransactionListTextViewModel>(json);
				json = savedInstanceState.GetString("TransactionDisputeHistory");
				_transactionDispluteHistoryViewModel = JsonConvert.DeserializeObject<MobileStatusResponse<List<TransactionDisputeHistoryItem>>>(json);
				json = savedInstanceState.GetString("AdditionalTransactions");
				_transactions = JsonConvert.DeserializeObject<List<Transaction>>(json);
				_month = savedInstanceState.GetInt("Month");
				_year = savedInstanceState.GetInt("Year");
			}
			else
			{
				var json = Intent.GetStringExtra("AdditionalTransactions");
				_transactions = JsonConvert.DeserializeObject<List<Transaction>>(json);
			}

			var jsonTransaction = Intent.GetStringExtra("DisputedTransaction");
			DisputedTransaction = JsonConvert.DeserializeObject<Transaction>(jsonTransaction);
			AccountDescription = Intent.GetStringExtra("AccountDescription");
			Suffix = Intent.GetStringExtra("Suffix");
			MemberId = Intent.GetStringExtra("MemberId");
			MaxSelectedTransactions = Intent.GetIntExtra("MaxSelectedTransactions", 0);
			LimitToDisputedMerchant = Intent.GetBooleanExtra("LimitToDisputedMerchant", false);

			_isCreditCard = Suffix.Length > 5;

			SetupView(Resource.Layout.AccountTransactionsSelectListView);

			refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.transactionsRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += RefreshTransactions;

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
			txtTitle.Text = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "A730D8E8-57FA-4E36-B702-01FED8244CB7", "Select Transactions");

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			txtDone = FindViewById<TextView>(Resource.Id.txtDone);
			txtDone.Click += (sender, e) => Submit();

			accountTitle = FindViewById<TextView>(Resource.Id.transactionTitleText);
			accountTitle.Text = AccountDescription;

			if (!string.IsNullOrEmpty(DisputedTransaction.CardNumber))
			{
				accountTitle.Text = AccountDescription + " > " + DisputedTransaction.CardNumber;
			}

			btnNext = FindViewById<ImageButton>(Resource.Id.btnNextTransactions);
			btnNext.Click += RefreshTransactions;
			btnNext.Enabled = false;

			btnPrevious = FindViewById<ImageButton>(Resource.Id.btnPreviousTransactions);
			btnPrevious.Click += RefreshTransactions;
			btnPrevious.Enabled = true;

			txtRangeDisplay = FindViewById<TextView>(Resource.Id.txtTransactionRangeDisplay);

			searchView = FindViewById<SearchView>(Resource.Id.searchBar);
			searchView.QueryTextChange += (sender, e) => ReloadData(_transactionsViewModel, _transactionDispluteHistoryViewModel, e.NewText);

			if (_month == 0)
			{
				var today = DateTime.Today;
				_month = today.Month;
				_year = today.Year;
			}

			LoadTransactions();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			string json = JsonConvert.SerializeObject(_transactionsViewModel);
			outState.PutString("Transactions", json);
			json = JsonConvert.SerializeObject(_transactionDispluteHistoryViewModel);
			outState.PutString("TransactionDisputeHistory", json);
			json = JsonConvert.SerializeObject(_transactions);
			outState.PutString("AdditionalTransactions", json);
			outState.PutInt("Month", _month);
			outState.PutInt("Year", _year);
			json = JsonConvert.SerializeObject(DisputedTransaction);
			outState.PutString("DisputedTransaction", json);

			base.OnSaveInstanceState(outState);
		}

        public override void SetCultureConfiguration()
        {
            txtDone.Text = CultureTextProvider.DONE();
        }

		private void RefreshTransactions(object sender, EventArgs e)
		{
			// Clear Search
			searchView.SetQuery(string.Empty, false);

			if (sender == btnPrevious)
			{
				_month = _month - 1;

				if (_month <= 0)
				{
					_month = 12;
					_year = _year - 1;
				}
			}
			else if (sender == btnNext)
			{
				_month = _month + 1;

				if (_month > 12)
				{
					_month = 1;
					_year = _year + 1;
				}
			}

			_transactionsViewModel = null;

			LoadTransactions();
		}

		private async void LoadTransactions()
		{
			var methods = new AccountMethods();

			var today = DateTime.Now;
			var startDate = new DateTime(_year, _month, 1);
			var endDate = new DateTime(_year, _month, DateTime.DaysInMonth(_year, _month));
			var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

			btnPrevious.Enabled = startDate > DateTime.Now.AddMonths(-3);
			btnNext.Enabled = startDate < firstDayOfMonth;

			txtRangeDisplay.Text = DateHelper.GetMonthsArray()[_month] + " " + _year;

			var request = new AccountTransactionListRequest
			{
				MemberId = MemberId,
				Suffix = Suffix,
				StartDate = startDate,
				EndDate = endDate.AddHours(23).AddMinutes(59).AddSeconds(59)
			};

			var getTransactionDisputeHistoryRequest = new GetTransactionDisputeHistoryRequest
			{
				MemberId = int.Parse(MemberId),
				BeginDate = startDate.ToString("yyyyMMdd"),
				EndDate = endDate.ToString("yyyyMMdd")
			};

			if (_transactionsViewModel == null)
			{
				if (!refresher.Refreshing)
				{
					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "9FC1A125-A95E-4E49-94D1-9E31908E7DDF", "Loading transactions..."));
				}

				_transactionsViewModel = await methods.AccountTransactionList(request, this);
				_transactionDispluteHistoryViewModel = await methods.GetTransactionDisputeHistory(getTransactionDisputeHistoryRequest, this);

				if (!refresher.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					refresher.Refreshing = false;
				}
			}

			if (_transactionsViewModel?.ClientViewState != null && _transactionsViewModel.ClientViewState == "TransactionList")
			{
				ReloadData(_transactionsViewModel, _transactionDispluteHistoryViewModel, null);
			}

			searchView.ClearFocus();
		}

		private void ReloadData(TransactionListTextViewModel model, MobileStatusResponse<List<TransactionDisputeHistoryItem>> disputeHistory, string searchText)
		{
			if (model != null)
			{
				var accountMethods = new AccountMethods();
				_listViewItems = ViewUtilities.ConvertTextViewModelToListViewItems(model, disputeHistory, _isCreditCard, searchText);
				_listViewItems = accountMethods.FilterSelectDisputeTransactions(_listViewItems, DisputedTransaction, MemberId, LimitToDisputedMerchant);
				_listAdapter = new TransactionsSelectionListViewAdapter(this, Resource.Layout.AccountTransactionsSelectionListViewItem, _listViewItems, Resource.Id.lblItemTitle, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text, Resource.Id.chkBox);
				_listAdapter.AddRemove += AddRemoveTransactions;
				_listAdapter.IsInList += IsInList;
				_listAdapter.AtMaximumTransactions += AtMaximumTransactions;
				_listAdapter.MaxSelectedTransactionsMessage = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E8DBFBF4-1282-417D-A086-481040A23496", MAX_TRANSACTIONS_MESSAGE);
				ListAdapter = _listAdapter;
			}
		}

		private bool AtMaximumTransactions()
		{
			return (_transactions.Count >= MaxSelectedTransactions);
		}

		private void AddRemoveTransactions(bool add, Transaction transaction)
		{
			int doAdd = 0;

			var accountMethods = new AccountMethods();

			for (int i = 0; i < _transactions.Count; i++)
			{
				if (accountMethods.DoTransactionsMatch(transaction, _transactions[i]))
				{
					if (add)
					{
						doAdd++;
					}
					else
					{
						_transactions.RemoveAt(i);
					}
				}
			}

			if (add && doAdd == 0)
			{
				_transactions.Add(transaction);
			}
		}

		private bool IsInList(Transaction transaction)
		{
			bool inList = false;

			var accountMethods = new AccountMethods();

			for (int i = 0; i < _transactions.Count; i++)
			{
				inList |= accountMethods.DoTransactionsMatch(transaction, _transactions[i]);
			}

			return inList;
		}

		private void Submit()
		{
			var intent = new Intent();

			var json = JsonConvert.SerializeObject(_transactions);
			intent.PutExtra("SelectedTransations", json);

			SetResult(Result.Ok, intent);
			Finish();
		}
	}
}
