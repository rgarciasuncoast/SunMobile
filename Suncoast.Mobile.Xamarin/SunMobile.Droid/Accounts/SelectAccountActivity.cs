using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunMobile.Droid.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;
using SunMobile.Shared.Culture;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "SelectAccountActivity", Theme = "@style/CustomHoloLightTheme")]
	public class SelectAccountActivity : BaseListActivity
	{
		private ImageButton btnCloseWindow;
		private SegmentedGroup segmentedGroup;
		private RadioButton btnPrimary;
		private RadioButton btnJoints;
		private RadioButton btnAnyMember;
		private SwipeRefreshLayout refresher;
        private TextView txtTitle;

		private AccountListTextViewModel _accountsViewModel;
		private AccountListTypes _accountListType = AccountListTypes.AllAccounts;
		public bool _showAnyMember;
		public bool _showJoints;
		public int _excludeMemberId;
		private string _excludeSuffix = string.Empty;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("accounts");
				_accountsViewModel = JsonConvert.DeserializeObject<AccountListTextViewModel>(json);
			}

			base.SetupView(Resource.Layout.AccountSelectListView);

			refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.selectListViewRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += ButtonClicked;

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			segmentedGroup = FindViewById<SegmentedGroup>(Resource.Id.AccountGroup);

			btnPrimary = FindViewById<RadioButton>(Resource.Id.btnPrimary);
			btnPrimary.Click += ButtonClicked;

			btnJoints = FindViewById<RadioButton>(Resource.Id.btnJoints);
			btnJoints.Click += ButtonClicked;

			btnAnyMember = FindViewById<RadioButton>(Resource.Id.btnAnyMember);
			btnAnyMember.Click += ButtonClicked;

            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

			string accountListType = Intent.GetStringExtra("AccountListType");
			_excludeSuffix = Intent.GetStringExtra("ExcludeSuffix");
			_excludeMemberId = Intent.GetIntExtra("ExcludeMemberId", 0);
			_showJoints = Intent.GetBooleanExtra("ShowJoints", false);
			_showAnyMember = Intent.GetBooleanExtra("ShowAnyMember", false);

			if (!_showAnyMember && !_showJoints)
			{
				segmentedGroup.Visibility = ViewStates.Gone;
			}

			if (!_showAnyMember)
			{
				segmentedGroup.RemoveView(btnAnyMember);
				segmentedGroup.UpdateBackgroundExternal();
			}

			if (!string.IsNullOrEmpty(accountListType))
			{
				_accountListType = JsonConvert.DeserializeObject<AccountListTypes>(accountListType);
			}

			if (SessionSettings.Instance.IsAuthenticated && savedInstanceState == null)
			{
				ButtonClicked(btnPrimary, null);
			}
			else
			{
				if (savedInstanceState != null)
				{
					if (savedInstanceState.GetString("selected") != "primary")
					{
						btnJoints.Checked = true;
						ButtonClicked(btnJoints, null);
					}
					else
					{
						btnPrimary.Checked = true;
						ButtonClicked(btnPrimary, null);
					}
				}
			}			
		}

		public override void SetCultureConfiguration()
		{
			try
			{
				CultureTextProvider.SetMobileResourceText(txtTitle, "BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "82BD4263-4826-46CA-8CBF-D415E58544E7", "Select an Account");
				CultureTextProvider.SetMobileResourceText(btnPrimary, "BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "435CF1F7-5FBB-4317-9D8F-C5ED125BEE17", "Primary");
				CultureTextProvider.SetMobileResourceText(btnJoints, "BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "CF221EDA-88A1-47E1-8624-7F9A447FB333", "Joints");
				CultureTextProvider.SetMobileResourceText(btnAnyMember, "91A8D274-E383-473D-87B7-EBD003F6AE44", "B2B8B00A-1852-48D3-BF0A-D3845FE3CAD9", "Other Member");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectAccountActivity:SetCultureConfiguration");
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			string json = JsonConvert.SerializeObject(_accountsViewModel);
			outState.PutString("accounts", json);

			string selected;
			
            if (segmentedGroup.CheckedRadioButtonId == btnPrimary.Id)
			{
				selected = "primary";
			}
			else
			{
				selected = "joints";
			}

			outState.PutString("selected", selected);

			base.OnSaveInstanceState(outState);
		}

		private void ButtonClicked(object sender, EventArgs e)
		{
			if (sender == btnAnyMember)
			{
				var intent = new Intent(this, typeof(TransferAnyMemberActivity));
				StartActivityForResult(intent, 0);
			}
			else
			{
				LoadAccounts(segmentedGroup.CheckedRadioButtonId != Resource.Id.btnPrimary);
			}
		}

		private async void LoadAccounts(bool includeJoints)
		{
			var methods = new AccountMethods();

			if (refresher.Refreshing)
			{
				_accountsViewModel = null;
			}
			else
			{
				ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "42779641-A083-47CA-B14F-FEEBC6EFA9EA", "Loading accounts..."));
			}

			if (_accountsViewModel == null)
			{
				switch (_accountListType)
				{
					case AccountListTypes.BillPayAccounts:
						var billPayAccountListRequest = new AccountListRequest();
						_accountsViewModel = await methods.BillPaySourceAccountList(billPayAccountListRequest, this);
						break;
					case AccountListTypes.RemoteDepositAccounts:
						var remoteDepositsAccountListRequest = new RemoteDepositsAccountListRequest();
						remoteDepositsAccountListRequest.MemberId = SessionSettings.Instance.UserId;
						_accountsViewModel = await methods.RemoteDepositsAccountList(remoteDepositsAccountListRequest, this);
						break;
					case AccountListTypes.TransferSourceAccounts:
						var transferSourceAccountListRequest = new TransferSourceAccountListRequest();
						transferSourceAccountListRequest.ExcludeMemberId = _excludeMemberId;
						transferSourceAccountListRequest.ExcludeSuffix = _excludeSuffix;
						_accountsViewModel = await methods.TransferSourceAccountList(transferSourceAccountListRequest, this);
						break;
					case AccountListTypes.TransferTargetAccounts:
						var transferTargetAccountListRequest = new TransferTargetAccountListRequest();
						transferTargetAccountListRequest.ExcludeMemberId = _excludeMemberId;
						transferTargetAccountListRequest.ExcludeSuffix = _excludeSuffix;
						_accountsViewModel = await methods.TransferTargetAccountList(transferTargetAccountListRequest, this);
						break;
					default:
						var accountListRequest = new AccountListRequest();
						_accountsViewModel = await methods.AccountList(accountListRequest, this);
						break;
				}
			}

			if (!refresher.Refreshing)
			{
				HideActivityIndicator();
			}
			else
			{
				refresher.Refreshing = false;
			}

			if (_accountsViewModel != null && _accountsViewModel.ClientViewState != null &&
				(_accountsViewModel.ClientViewState == "AccountList" ||
					_accountsViewModel.ClientViewState == "BillPaySourceAccountList" ||
					_accountsViewModel.ClientViewState == "RemoteDepositsAccountList" ||
					_accountsViewModel.ClientViewState == "TransferSourceAccountList" ||
					_accountsViewModel.ClientViewState == "TransferTargetAccountList"))
			{
				var groupedAdapter = ViewUtilities.ConvertTextViewModelToGroupedListItemAdapter(this, _accountsViewModel, includeJoints);

				ListAdapter = groupedAdapter;
			}
			else
			{
				_accountsViewModel = null;
			}
		}

		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);

			try
			{
				var listViewItem = ((AccountListItemAdapter)l.Adapter).GetListViewItem(position);

				var intent = new Intent();
				intent.PutExtra("ClassName", "SelectAccountActivity");
				var json = JsonConvert.SerializeObject(listViewItem.Data);
				intent.PutExtra("Account", json);
				listViewItem.Data = null;
				json = JsonConvert.SerializeObject(listViewItem);
				intent.PutExtra("ListViewItem", json);
				SetResult(Result.Ok, intent);
				Finish();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectAccountActivity:OnListItemClick");
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == Result.Ok && data != null)
			{
				if (data.Component == null)
				{
					var json = data.GetStringExtra("AnyMemberInfo");
					var intent = new Intent();
					intent.PutExtra("AnyMemberInfo", json);
					SetResult(Result.Ok, intent);
					Finish();
				}
			}
		}
	}
}
