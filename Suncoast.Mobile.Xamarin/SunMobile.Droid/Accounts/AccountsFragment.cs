using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.Droid.Accounts.SubAccounts;
using SunMobile.Droid.Common;
using SunMobile.Droid.ExternalServices;
using SunMobile.Droid.Profile;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts
{
	public class AccountsFragment : BaseListFragment
	{
		private RadioButton btnPrimary;
		private RadioButton btnJoints;
		private SwipeRefreshLayout refresher;
		private SegmentedGroup segmentAccountType;
        private ImageButton btnRocketAccount;
        private Button btnDismissRocketAccount;
		private AccountListTextViewModel _accountsViewModel;
		private AccountListItemAdapter _groupedAdapter;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AccountListView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = JsonConvert.SerializeObject(_accountsViewModel);
			outState.PutString("Accounts", json);

			if (segmentAccountType != null)
			{
				string selected;
				if (segmentAccountType.CheckedRadioButtonId != btnPrimary.Id)
				{
					selected = "joints";
				}
				else
				{
					selected = "primary";
				}
				outState.PutString("selected", selected);
			}

			base.OnSaveInstanceState(outState);
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                var viewText = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "55E7CEFF-4826-4DFF-BF8C-5329FF4ECFF6", "Accounts");

                if (!string.IsNullOrEmpty(viewText))
                {
                    ((MainActivity)Activity).SetActionBarTitle(viewText);
                }

                btnPrimary.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "435CF1F7-5FBB-4317-9D8F-C5ED125BEE17", "Primary");
                btnJoints.Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "CF221EDA-88A1-47E1-8624-7F9A447FB333", "Joints");
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "AccountsFragment:SetCultureConfiguration");
            }
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("Accounts");
				_accountsViewModel = JsonConvert.DeserializeObject<AccountListTextViewModel>(json);
			}

			((MainActivity)Activity).SetActionBarTitle("Accounts");
            //((MainActivity)Activity).ShowAddActionButton(true);

			refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.accountsRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += ButtonClicked;

			btnPrimary = Activity.FindViewById<RadioButton>(Resource.Id.btnPrimary);
			btnPrimary.Selected = true;
			btnPrimary.Click += ButtonClicked;

			btnJoints = Activity.FindViewById<RadioButton>(Resource.Id.btnJoints);
			btnJoints.Click += ButtonClicked;

			segmentAccountType = Activity.FindViewById<SegmentedGroup>(Resource.Id.segmentAccountType);

            btnRocketAccount = Activity.FindViewById<ImageButton>(Resource.Id.btnRocketAccount);
            btnRocketAccount.Click += (sender, e) =>
            {
                if (_accountsViewModel.RocketEligibility.IsFundedEligible)
                {
                    OpenSlideout();
                }
                else
                {
                    StartSubAccounts();
                }
            };

            btnDismissRocketAccount = Activity.FindViewById<Button>(Resource.Id.btnDismissRocketAccount);
            btnDismissRocketAccount.Click += (sender, e) => DismissSubAccounts();

			if (SessionSettings.Instance.IsAuthenticated && savedInstanceState == null)
			{
				ButtonClicked(btnPrimary, null);
			}
			else
			{
				if (savedInstanceState != null)
				{
					if (savedInstanceState.GetString("selected") == "primary")
					{
						btnPrimary.Checked = true;
						ButtonClicked(btnPrimary, null);
					}
					else
					{
						btnJoints.Checked = true;
						ButtonClicked(btnJoints, null);
					}
				}
			}

			ListViewMain.ItemClick += (sender, e) =>
			{
				var item = _groupedAdapter.GetListViewItem(e.Position);
                var transactionsFragment = new TransactionsFragment();
                transactionsFragment.Account = (SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.Account)item.Data;

                NavigationService.NavigatePush(transactionsFragment, true, false, true);
			};			
		}

        public async void AccountOptions()
        {
            var response = await AlertMethods.ActionSheet(Activity, "Account Options", "Add a Savings Account", "Add a Money Market Account", "Add a Certificate", "Cancel");

            if (response != "Cancel")
            {
                var contactInfoFragment = new ContactInfoFragment();
                contactInfoFragment.EditMode = false;

                contactInfoFragment.Updated += (obj) =>
                {
                    var loanCenterFragment = new LoanCenterFragment();
                    loanCenterFragment.LoanType = LoanCenterTypes.ApplyForLoan;
                    NavigationService.NavigatePush(loanCenterFragment, false, true);;
                };

                NavigationService.NavigatePush(contactInfoFragment, true);
            }
        }

        private void OpenSlideout()
        {
            var slideoutDialog = new SubAccountsSlideoutDialog();
            slideoutDialog.StartSubAccounts += (obj) => StartSubAccounts();
            slideoutDialog.Show(Activity.FragmentManager, "dialog");
        }	

        private void StartSubAccounts()
        {
            var subAccountsContactFragment = new SubAccountsContactFragment();
            subAccountsContactFragment.Info = new SubAccountsInfo { IsFunded = true };
            subAccountsContactFragment.PageIndex = 0;
			NavigationService.NavigatePush(subAccountsContactFragment, true, false, true);
        }

		private async void DismissSubAccounts()
		{
            btnRocketAccount.Visibility = ViewStates.Gone;			
            btnDismissRocketAccount.Visibility = ViewStates.Gone;

			var flagOptions = new List<FlagOption>();
			var flagDeclined = new FlagOption { FlagType = FlagTypes.InstantCheckingMemberDeclined.ToString(), Value = true };
			flagOptions.Add(flagDeclined);
			var methods = new AuthenticationMethods();
			await methods.SetFlags(flagOptions, View);
		}

		private void ButtonClicked(object sender, EventArgs e)
		{
			LoadAccounts(segmentAccountType.CheckedRadioButtonId != Resource.Id.btnPrimary);
		}

		private async void LoadAccounts(bool includeJoints)
		{
			try 
			{
				var methods = new AccountMethods();
				var request = new AccountListRequest();

				if (refresher.Refreshing)
				{
					_accountsViewModel = null;
				}

				if (_accountsViewModel == null)
				{
					if (!refresher.Refreshing)
					{
						var loadingAccounts = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "42779641-A083-47CA-B14F-FEEBC6EFA9EA", "Loading Accounts...");
						ShowActivityIndicator(loadingAccounts);
					}

					_accountsViewModel = await methods.AccountList(request, Activity);
				}

				if (!refresher.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					refresher.Refreshing = false;
				}

				if (_accountsViewModel?.ClientViewState != null && _accountsViewModel.ClientViewState == "AccountList")
				{
					bool includeRocketAccounts = false;

                    if (_accountsViewModel.RocketEligibility != null && _accountsViewModel.RocketEligibility.IsCheckingEligible)
					{
						includeRocketAccounts = true;
                        btnRocketAccount.Visibility = ViewStates.Visible;
                        btnDismissRocketAccount.Visibility = ViewStates.Visible;
					}

                    _groupedAdapter = ViewUtilities.ConvertTextViewModelToGroupedListItemAdapter(Activity, _accountsViewModel, includeJoints, includeRocketAccounts);
					ListAdapter = _groupedAdapter;
				}
				else
				{
					_accountsViewModel = null;
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "AccountsFragment:LoadAccounts");
			}
		}

        public override void OnStop()
        {
            //((MainActivity)Activity).ShowAddActionButton(false);

            base.OnStop();
        }
	}
}