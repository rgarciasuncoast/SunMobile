using System;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid
{
	public class MainMenuFragment : BaseFragment
	{
		private TableRow _tableRowAccounts;
		private TextView _AccountsLabel;
		private TableRow _tableRowCards;
		private TextView _CardsLabel;
		private TableRow _tableRowTransfers;
		private TextView _TransfersLabel;
		private TableRow _tableRowDeposits;
		private TextView _DepositsLabel;
		private TableRow _tableRowBillPay;
		private TextView _BillPayLabel;
		private TableRow _tableRowSunMoney;
		private TextView _SunMoneyLabel;
		private TableRow _tableRowLocations;
		private TextView _LocationsLabel;
		private TableRow _tableRowMessaging;
		private TextView _MessagingLabel;
		private TableRow _tableRowDocuments;
		private TextView _DocumentsLabel;
		private TableRow _tableRowLoanCenter;
		private TextView _LoanCenterLabel;
		private TableRow _tableRowProfile;
		private TextView _ProfileLabel;
		private TableRow _tableRowAbout;
		private TextView _AboutLabel;
		private TableRow _tableRowSignOut;
		private TextView _SignOutLabel;
		private ViewGroup _container;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.mainmenuview, null);
			RetainInstance = true;

			_container = container;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			try
			{
				_tableRowAccounts = Activity.FindViewById<TableRow>(Resource.Id.rowAccounts);
				_tableRowAccounts.Click += (sender, e) => ListItemClicked(1, sender);
				_tableRowCards = Activity.FindViewById<TableRow>(Resource.Id.rowCards);
				_tableRowCards.Click += (sender, e) => ListItemClicked(2, sender);
				_tableRowTransfers = Activity.FindViewById<TableRow>(Resource.Id.rowTransfers);
				_tableRowTransfers.Click += (sender, e) => ListItemClicked(3, sender);
				_tableRowDeposits = Activity.FindViewById<TableRow>(Resource.Id.rowDeposits);
				_tableRowDeposits.Click += (sender, e) => ListItemClicked(4, sender);
				_tableRowBillPay = Activity.FindViewById<TableRow>(Resource.Id.rowBillPay);
				_tableRowBillPay.Click += (sender, e) => ListItemClicked(5, sender);
				_tableRowLoanCenter = Activity.FindViewById<TableRow>(Resource.Id.rowLoanCenter);
				_tableRowLoanCenter.Click += (sender, e) => ListItemClicked(6, sender);
				_tableRowSunMoney = Activity.FindViewById<TableRow>(Resource.Id.rowSunMoney);
				_tableRowSunMoney.Click += (sender, e) => ListItemClicked(7, sender);
				_tableRowLocations = Activity.FindViewById<TableRow>(Resource.Id.rowLocations);
				_tableRowLocations.Click += (sender, e) => ListItemClicked(8, sender);
				_tableRowMessaging = Activity.FindViewById<TableRow>(Resource.Id.rowMessaging);
				_tableRowMessaging.Click += (sender, e) => ListItemClicked(9, sender);
				_tableRowDocuments = Activity.FindViewById<TableRow>(Resource.Id.rowDocuments);
				_tableRowDocuments.Click += (sender, e) => ListItemClicked(10, sender);
				_tableRowProfile = Activity.FindViewById<TableRow>(Resource.Id.rowProfile);
				_tableRowProfile.Click += (sender, e) => ListItemClicked(11, sender);
				_tableRowAbout = Activity.FindViewById<TableRow>(Resource.Id.rowAbout);
				_tableRowAbout.Click += (sender, e) => ListItemClicked(12, sender);
				_tableRowSignOut = Activity.FindViewById<TableRow>(Resource.Id.rowSignOut);
				_tableRowSignOut.Click += (sender, e) => ListItemClicked(13, sender);

				_AccountsLabel = Activity.FindViewById<TextView>(Resource.Id.rowAccountsText);
				_CardsLabel = Activity.FindViewById<TextView>(Resource.Id.rowCardsText);
				_TransfersLabel = Activity.FindViewById<TextView>(Resource.Id.rowTransfersText);
				_DepositsLabel = Activity.FindViewById<TextView>(Resource.Id.rowDepositsText);
				_BillPayLabel = Activity.FindViewById<TextView>(Resource.Id.rowBillPayText);
				_SunMoneyLabel = Activity.FindViewById<TextView>(Resource.Id.rowSunMoneyText);
				_LocationsLabel = Activity.FindViewById<TextView>(Resource.Id.rowLocationsText);
				_MessagingLabel = Activity.FindViewById<TextView>(Resource.Id.rowMessagingText);
				_DocumentsLabel = Activity.FindViewById<TextView>(Resource.Id.rowDocumentsText);
				_LoanCenterLabel = Activity.FindViewById<TextView>(Resource.Id.rowLoanCenterText);
				_ProfileLabel = Activity.FindViewById<TextView>(Resource.Id.rowProfileText);
				_AboutLabel = Activity.FindViewById<TextView>(Resource.Id.rowAboutText);
				_SignOutLabel = Activity.FindViewById<TextView>(Resource.Id.rowSignOutText);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainMenuFragment:SetupView");
			}

			HighlightSelectedItem(_container, SessionSettings.Instance.LastMenuIndex);			
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                CultureTextProvider.SetMobileResourceText(_AccountsLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "956E2062-2EA2-44BD-884F-A92EFDECDF9F", "Accounts");
                CultureTextProvider.SetMobileResourceText(_CardsLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "2481D95B-D465-4B87-AA3A-3CC420B1C296", "Cards");
                CultureTextProvider.SetMobileResourceText(_TransfersLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "294EB33E-925F-42A6-8560-F7C3676530D3", "Transfer Funds");
                CultureTextProvider.SetMobileResourceText(_DepositsLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "99446298-1B9F-41BA-894A-875772D7CF4E", "Deposit Funds");
                CultureTextProvider.SetMobileResourceText(_BillPayLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "FC274687-8339-4618-8960-5C202AC14881", "Bill Pay");
                CultureTextProvider.SetMobileResourceText(_LoanCenterLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "72B7A071-14D7-40BB-B1F7-F6C5F94BF188", "Loan Center");
                CultureTextProvider.SetMobileResourceText(_SunMoneyLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "B888D315-F938-4283-BE4A-8624454218FB", "SunMoney");
                CultureTextProvider.SetMobileResourceText(_LocationsLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "2896CB59-E999-49A9-9059-4E12786FA1B5", "Find ATM/Branch");
                CultureTextProvider.SetMobileResourceText(_MessagingLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "26C013AC-ED93-406F-8DE9-DBB33579F8B5", "Message Center");
                CultureTextProvider.SetMobileResourceText(_DocumentsLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "9F23B326-B342-47F2-A4BC-D4578D17F9E2", "Documents");
                CultureTextProvider.SetMobileResourceText(_ProfileLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "91E519FD-02C1-479B-A770-AC20CE256EB2", "My Profile");
                CultureTextProvider.SetMobileResourceText(_AboutLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "9EC061A4-3B17-412A-87C7-FE119ECF96D5", "Contact Us");
                CultureTextProvider.SetMobileResourceText(_SignOutLabel, "D7C907BA-E9E8-454E-B87F-AAE7AE226022", "7096369F-20F4-4124-97DC-CC9F72F8BFFF", "Sign Out");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "MainMenuFragment:SetCultureConfiguration");
			}
		}

		private void ListItemClicked(int position, object sender)
		{
			try
			{
				((MainActivity)Activity).ListItemClicked(position);
				RemoveHighlightedItem(_container);
				HighlightSelectedItem(_container, position);

			}
			catch (Exception ex)
			{
				Logging.Log(ex, "MainMenuFragment:ListItemClicked");
			}
		}

		private void RemoveHighlightedItem(ViewGroup viewGroup)
		{
			int childCount = viewGroup.ChildCount;

			for (int i = 0; i < childCount; i++)
			{
				View view = viewGroup.GetChildAt(i);

				if (view is TableRow)
				{
					if (view.Id != Resource.Id.rowLogo)
					{
						((TableRow)view).SetBackgroundColor(Color.ParseColor("#eeeeee"));
					}
				}

				if (view is ViewGroup)
				{
					RemoveHighlightedItem((ViewGroup)view);
				}
			}
		}

		public void HighlightSelectedItem(ViewGroup viewGroup, int position)
		{
			if (position == 0)
			{
				position = 1;
			}

			if (viewGroup == null)
			{
				viewGroup = _container;
			}

			int childCount = viewGroup.ChildCount;
			int rowCount = 0;

			for (int i = 0; i < childCount; i++)
			{
				View view = viewGroup.GetChildAt(i);

				if (view is TableRow)
				{
					rowCount++;

					if (rowCount == (position + 1))
					{
						((TableRow)view).SetBackgroundColor(Color.ParseColor("#d1d1d1"));
					}
				}

				if (view is ViewGroup)
				{
					HighlightSelectedItem((ViewGroup)view, position);
				}
			}

			SessionSettings.Instance.LastMenuIndex = position;
		}
	}
}