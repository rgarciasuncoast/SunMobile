using System;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.iOS.Common;
using SunMobile.iOS.Transfers;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public partial class SelectAccountViewController : BaseViewController
	{
		private AccountListTextViewModel _accountsViewModel;
		public event Action<ListViewItem> AccountSelected = delegate{};
		public event Action<AnyMemberInfo> AnyMemberSelected = delegate{};
		public AccountListTypes AccountListType { get; set; }
		public bool ShowAnyMember { get; set; }
		public bool ShowJoints { get; set; }
		public int ExcludeMemberId { get; set; }
		public string ExcludeSuffix { get; set; }
		private bool _isJointTransfer;
		private UIRefreshControl _refreshControl;

		public SelectAccountViewController(IntPtr handle) : base(handle)
		{
			AccountListType = AccountListTypes.AllAccounts;
			ExcludeSuffix = string.Empty;
		}		

		public override void ViewDidLoad()
		{
		    try
		    {
		        base.ViewDidLoad();

		        var leftButton = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
		        leftButton.TintColor = AppStyles.TitleBarItemTintColor;
		        NavigationItem.SetLeftBarButtonItem(leftButton, false);
		        leftButton.Clicked += (sender, e) => NavigationController.PopViewController(true);

		        _refreshControl = new UIRefreshControl();
		        tableViewAccounts.AddSubview(_refreshControl);
		        _refreshControl.ValueChanged += Refresh;

		        segmentAccountTypes.ValueChanged += AccountTypeChanged;

		        if (!ShowAnyMember && !ShowJoints)
		        {
		            segmentAccountTypes.Hidden = true;
		        }

		        if (!ShowAnyMember)
		        {
		            segmentAccountTypes.RemoveSegmentAtIndex(2, false);
		        }		        

		        _isJointTransfer = false;

		        LoadAccounts(false);
		    }
		    catch (Exception ex)
		    {
		        Logging.Log(ex, "SelectAccountViewController:ViewDidLoad");
		    }
		}

		public override void SetCultureConfiguration()
		{
            NavigationItem.Title = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "82BD4263-4826-46CA-8CBF-D415E58544E7", "Select an Account");

            if (ShowAnyMember || ShowJoints)
            {
                segmentAccountTypes.SetTitle(CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "435CF1F7-5FBB-4317-9D8F-C5ED125BEE17", "Primary"), 0);
                segmentAccountTypes.SetTitle(CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "CF221EDA-88A1-47E1-8624-7F9A447FB333", "Joints"), 1);

                if (ShowAnyMember)
                {
                    segmentAccountTypes.SetTitle(CultureTextProvider.GetMobileResourceText("91A8D274-E383-473D-87B7-EBD003F6AE44", "B2B8B00A-1852-48D3-BF0A-D3845FE3CAD9", "Other Member"), 2);
                }
            }
		}

		private void Refresh(object sender, EventArgs e)
		{
			_accountsViewModel = null;
			AccountTypeChanged(sender, e);
		}

		private void AccountTypeChanged(object sender, EventArgs e)
		{
			try
			{
				var selectedSegmentId = segmentAccountTypes.SelectedSegment;

				if (selectedSegmentId == 0)
				{
					_isJointTransfer = false;
					LoadAccounts(false);
				}

				if (selectedSegmentId == 1)
				{
					_isJointTransfer = true;
					LoadAccounts(true);
				}

				if (selectedSegmentId == 2)
				{
					var myStoryboard = AppDelegate.StoryBoard;
					var controller = myStoryboard.InstantiateViewController("TransferAnyMemberTableViewController") as TransferAnyMemberTableViewController;

					controller.AnyMemberSelected += AnyMemberInfo =>
					{
						if (AnyMemberSelected != null)
						{
							AnyMemberSelected(AnyMemberInfo);
						}
						NavigationController.PopViewController(true);
					};

					NavigationController.PushViewController(controller, true);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectAccountViewController:AccountTypeChanged");
			}
		}

        private async void LoadAccounts(bool includeJoints)
        {
            try
            {
                var methods = new AccountMethods();

                if (_accountsViewModel == null)
                {
                    if (!_refreshControl.Refreshing)
                    {
                        ShowActivityIndicator();
                    }

                    switch (AccountListType)
                    {
                        case AccountListTypes.BillPayAccounts:
                            var billPayAccountListRequest = new AccountListRequest();
                            _accountsViewModel = await methods.BillPaySourceAccountList(billPayAccountListRequest, View);
                            break;
                        case AccountListTypes.RemoteDepositAccounts:
                            var remoteDepositsAccountListRequest = new RemoteDepositsAccountListRequest();
                            remoteDepositsAccountListRequest.MemberId = SessionSettings.Instance.UserId;
                            _accountsViewModel = await methods.RemoteDepositsAccountList(remoteDepositsAccountListRequest, View);
                            break;
                        case AccountListTypes.TransferSourceAccounts:							
                            var transferSourceAccountListRequest = new TransferSourceAccountListRequest();
                            transferSourceAccountListRequest.ExcludeMemberId = ExcludeMemberId;
                            transferSourceAccountListRequest.ExcludeSuffix = ExcludeSuffix;
                            _accountsViewModel = await methods.TransferSourceAccountList(transferSourceAccountListRequest, View);
                            break;
                        case AccountListTypes.TransferTargetAccounts:
                            var transferTargetAccountListRequest = new TransferTargetAccountListRequest();
                            transferTargetAccountListRequest.ExcludeMemberId = ExcludeMemberId;
                            transferTargetAccountListRequest.ExcludeSuffix = ExcludeSuffix;
                            _accountsViewModel = await methods.TransferTargetAccountList(transferTargetAccountListRequest, View);
                            break;
                        case AccountListTypes.FundingAccounts:
                            var response = await methods.SubAccountsFundingAccountList(null, View);
                            _accountsViewModel = response.AccountListViewModel;
                            break;
                        default:
                            var accountListRequest = new AccountListRequest();
                            _accountsViewModel = await methods.AccountList(accountListRequest, View);
                            break;
                    }

                    if (!_refreshControl.Refreshing)
                    {
                        HideActivityIndicator();
                    }
                    else
                    {
                        _refreshControl.EndRefreshing();
                    }
                }

                if (_accountsViewModel != null && _accountsViewModel.ClientViewState != null &&
                    (_accountsViewModel.ClientViewState == "AccountList" ||
                        _accountsViewModel.ClientViewState == "BillPaySourceAccountList" ||
                        _accountsViewModel.ClientViewState == "RemoteDepositsAccountList" ||
                        _accountsViewModel.ClientViewState == "TransferSourceAccountList" ||
                       _accountsViewModel.ClientViewState == "SubAccountsFundingAccountList" ||
                        _accountsViewModel.ClientViewState == "TransferTargetAccountList"))
                {
                    var tableViewSource = new AccountsTableViewSource(_accountsViewModel, includeJoints);

                    tableViewSource.ItemSelected += item =>
                    {
                        item.Bool1Value = _isJointTransfer;
                        AccountSelected(item);
                        NavigationController.PopViewController(true);
                    };

                    tableViewAccounts.Source = tableViewSource;
                    tableViewAccounts.ReloadData();
                }
                else
                {
                    _accountsViewModel = null;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SelectAccountViewController:LoadAccounts");
            }
        }
	}
}