using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
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
	public partial class TransactionSelectionViewController : BaseViewController
	{
		public event Action<List<Transaction>> TransactionsSelected = delegate { };
		public string MemberId { get; set; }
		public string Suffix { get; set; }
		public string AccountDescription { get; set; }
		public List<Transaction> SelectedTransactions { get; set; }
		public int MaxSelectedTransactions { get; set; }
		public Transaction DisputedTransaction { get; set; }
		public bool LimitToDisputedMerchant { get; set; }

		private TransactionListTextViewModel _transactionsViewModel;
		private MobileStatusResponse<List<TransactionDisputeHistoryItem>> _transactionDispluteHistoryViewModel;
		private UISearchBar _searchBar;
		private UIRefreshControl _refreshControl;
		private int _month;
		private int _year;
		private bool _isCreditCard;
		private string MAX_TRANSACTIONS_MESSAGE;

		public TransactionSelectionViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (SelectedTransactions == null)
			{
				SelectedTransactions = new List<Transaction>();
			}

            var leftButton = new UIBarButtonItem(CultureTextProvider.BACK(), UIBarButtonItemStyle.Plain, null);
			leftButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetLeftBarButtonItem(leftButton, false);
			leftButton.Clicked += (sender, e) => NavigationController.PopViewController(true);

            var rightButton = new UIBarButtonItem(CultureTextProvider.DONE(), UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			rightButton.Clicked += (sender, e) => Submit();

			_refreshControl = new UIRefreshControl();
			TransactionSelectionTableView.AddSubview(_refreshControl);

			_refreshControl.ValueChanged += RefreshTransactions;

			_isCreditCard = Suffix.Length > 5;

			btnNext.TouchUpInside += RefreshTransactions;
			btnNext.Enabled = false;
			btnPrevious.TouchUpInside += RefreshTransactions;

			lblHeader.BackgroundColor = AppStyles.BarTintColor;
			lblHeader.Text = AccountDescription;

			if (!string.IsNullOrEmpty(DisputedTransaction.CardNumber))
			{
				lblHeader.Text = AccountDescription + " > " + DisputedTransaction.CardNumber;
			}

			var today = DateTime.Today;
			_month = today.Month;
			_year = today.Year;

			// Configure the search bar
			_searchBar = new UISearchBar(new CGRect(0, 0, 320, 44))
			{
				ShowsScopeBar = false
			};

			_searchBar.TextChanged += (sender, e) =>
			{
				var textViewTableSource = ViewUtilities.ConvertTextViewModelToTextViewTableSource(_transactionsViewModel, _transactionDispluteHistoryViewModel, _isCreditCard, e.SearchText);
				var tableViewSource = new TransactionSelectionTableViewSource(textViewTableSource);
				SetTableViewSource(tableViewSource);
			};

			_searchBar.SearchButtonClicked += (sender, e) => GeneralUtilities.CloseKeyboard(View);

			TransactionSelectionTableView.TableHeaderView = _searchBar;

			LoadTransactions();			
		}

		public override void SetCultureConfiguration()
		{
			NavigationItem.Title = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "A730D8E8-57FA-4E36-B702-01FED8244CB7", "Select Transactions");
			MAX_TRANSACTIONS_MESSAGE = CultureTextProvider.GetMobileResourceText("44B28933-4B58-4B6B-AC1A-B6F057847900", "E8DBFBF4-1282-417D-A086-481040A23496", "You have selected the maximum amount of transactions");
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			GeneralUtilities.CloseKeyboard(View);
		}

		private void SetTableViewSource(TransactionSelectionTableViewSource tableViewSource)
		{
			tableViewSource.IsInList += IsInList;
			tableViewSource.AddRemove += AddRemoveTransactions;
			tableViewSource.ItemSelected += SelectTransaction;
			TransactionSelectionTableView.Source = tableViewSource;
			TransactionSelectionTableView.ReloadData();
		}

		private void SelectTransaction(ListViewItem item)
		{
			try
			{
				if (item != null)
				{
					if (item.IsChecked || IsInList((Transaction)item.Data))
					{
						item.IsChecked = false;
						AddRemoveTransactions(false, (Transaction)item.Data);
					}
					else
					{
						if (SelectedTransactions.Count < MaxSelectedTransactions)
						{
							item.IsChecked = true;
							AddRemoveTransactions(true, (Transaction)item.Data);
						}
						else
						{
                            AlertMethods.Alert(View, "Transaction Selection", MAX_TRANSACTIONS_MESSAGE, CultureTextProvider.OK());
						}
					}
					TransactionSelectionTableView.ReloadData();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionsViewControler:ViewCheck");
			}
		}

		private void RefreshTransactions(object sender, EventArgs e)
		{
			// Clear the table
			var tableViewSource = new TransactionsTableViewSource(new TextViewTableSource { Items = new List<ListViewItem>() }, MemberId);
			TransactionSelectionTableView.Source = tableViewSource;
			TransactionSelectionTableView.ReloadData();

			// Clear Search
			_searchBar.Text = string.Empty;

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

			LoadTransactions();
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			TransactionSelectionTableView.ReloadData();
		}

		private async void LoadTransactions()
		{
			var methods = new AccountMethods();

			var today = DateTime.Now;
			var startDate = new DateTime(_year, _month, 1);

			if (startDate < today.Date.AddDays(-90))
			{
				startDate = today.Date.AddDays(-90);
			}

			var endDate = new DateTime(_year, _month, DateTime.DaysInMonth(_year, _month));
			var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

			btnPrevious.Enabled = startDate > DateTime.Now.AddDays(-90);
			btnNext.Enabled = startDate < firstDayOfMonth;

			lblDate.Text = DateHelper.GetMonthsArray()[_month] + " " + _year;

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

			if (!_refreshControl.Refreshing)
			{
				ShowActivityIndicator();
			}

			_transactionsViewModel = await methods.AccountTransactionList(request, View);
			_transactionDispluteHistoryViewModel = await methods.GetTransactionDisputeHistory(getTransactionDisputeHistoryRequest, View);

			if (!_refreshControl.Refreshing)
			{
				HideActivityIndicator();
			}
			else
			{
				_refreshControl.EndRefreshing();
			}

			if (_transactionsViewModel != null && _transactionsViewModel.ClientViewState != null && _transactionsViewModel.ClientViewState == "TransactionList")
			{
				var accountMethods = new AccountMethods();
				var textViewTableSource = ViewUtilities.ConvertTextViewModelToTextViewTableSource(_transactionsViewModel, _transactionDispluteHistoryViewModel, _isCreditCard, null);
				textViewTableSource.Items = accountMethods.FilterSelectDisputeTransactions(textViewTableSource.Items, DisputedTransaction, MemberId, LimitToDisputedMerchant);
				var tableViewSource = new TransactionSelectionTableViewSource(textViewTableSource);
				SetTableViewSource(tableViewSource);
			}
		}

		private void AddRemoveTransactions(bool add, Transaction transaction)
		{
			int doAdd = 0;

			var accountMethods = new AccountMethods();

			for (int i = 0; i < SelectedTransactions.Count; i++)
			{
				if (accountMethods.DoTransactionsMatch(transaction, SelectedTransactions[i]))
				{
					if (add && SelectedTransactions.Count < 19)
					{
						doAdd++;
					}
					else
					{
						SelectedTransactions.RemoveAt(i);
					}
				}
			}

			if (add && doAdd == 0)
			{
				SelectedTransactions.Add(transaction);
			}
		}

		private bool IsInList(Transaction transaction)
		{
			bool inList = false;

			var accountMethods = new AccountMethods();

			for (int i = 0; i < SelectedTransactions.Count; i++)
			{
				inList |= accountMethods.DoTransactionsMatch(transaction, SelectedTransactions[i]);
			}

			return inList;
		}

		private void Submit()
		{
			if (SelectedTransactions != null)
			{
				TransactionsSelected(SelectedTransactions);
			}
		}
	}
}