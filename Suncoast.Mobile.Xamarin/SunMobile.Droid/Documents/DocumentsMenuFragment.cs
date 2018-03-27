using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Documents
{
	public class DocumentsMenuFragment : BaseFragment
	{
		private TableRow _tableRowDocumentCenter;
		private TableRow _tableRowAccounteStatements;
		private TableRow _tableRowTaxDocuments;
		private TableRow _tableRowENotices;
        private TableRow _tableRowCCStatements;
        private TextView lblDocumentCenter;
        private TextView lblAccountEStatements;
        private TextView lblTaxDocuments;
        private TextView lblENotices;
        private TextView lblCCStatements;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.DocumentsMenuView, null);
			RetainInstance = true;

			return view;
		}

		public override void SetCultureConfiguration()
		{
			try
			{
				((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "4455C2A9-0A4E-456C-A852-A023032B3C52", "Documents"));
                CultureTextProvider.SetMobileResourceText(lblDocumentCenter, "441F87A2-7C3B-4296-A017-999BDB2BE512", "EFC795FA-785C-4C01-9A1D-EC4A6627754F", "Document Center");
                CultureTextProvider.SetMobileResourceText(lblAccountEStatements, "441F87A2-7C3B-4296-A017-999BDB2BE512", "4258CDF2-5D12-47A8-B912-8B5A8E1B310A", "Account eStatements");
                CultureTextProvider.SetMobileResourceText(lblTaxDocuments, "441F87A2-7C3B-4296-A017-999BDB2BE512", "46651E04-01C9-4B98-9333-21EFC980813B", "Tax Documents");
                CultureTextProvider.SetMobileResourceText(lblENotices, "441F87A2-7C3B-4296-A017-999BDB2BE512", "EABA0759-FF1A-4D9F-96F5-1AC83303C541", "eNotices");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DocumentsMenuFragment:SetCultureConfiguration");
			}
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();			
		}

		public override void SetupView()
		{
			base.SetupView();

			_tableRowDocumentCenter = Activity.FindViewById<TableRow>(Resource.Id.rowDocumentCenter);
			_tableRowDocumentCenter.Click += (sender, e) => ListItemClicked(0);
			_tableRowAccounteStatements = Activity.FindViewById<TableRow>(Resource.Id.rowAccounteStatements);
			_tableRowAccounteStatements.Click += (sender, e) => ListItemClicked(1);
            _tableRowCCStatements = Activity.FindViewById<TableRow>(Resource.Id.rowCCStatements);
            _tableRowCCStatements.Click += (sender, e) => ListItemClicked(2);
            _tableRowENotices = Activity.FindViewById<TableRow>(Resource.Id.rowENotices);
            _tableRowENotices.Click += (sender, e) => ListItemClicked(3);
			_tableRowTaxDocuments = Activity.FindViewById<TableRow>(Resource.Id.rowTaxDocuments);
			_tableRowTaxDocuments.Click += (sender, e) => ListItemClicked(4);

            lblDocumentCenter = Activity.FindViewById<TextView>(Resource.Id.lblDocumentMenuDocumentCenter);
            lblAccountEStatements = Activity.FindViewById<TextView>(Resource.Id.lblDocumentMenuAccountEStatements);
            lblTaxDocuments = Activity.FindViewById<TextView>(Resource.Id.lblDocumentMenuTaxDocuments);
            lblENotices = Activity.FindViewById<TextView>(Resource.Id.lblDocumentMenuENotices);
            lblCCStatements = Activity.FindViewById<TextView>(Resource.Id.lblCreditCardStatements);
		}

		public void ListItemClicked(int position)
		{
			Android.Support.V4.App.Fragment fragment = null;

			switch (position)
			{
				case 0:
					fragment = new DocumentCenterFragment();
					break;
				case 1:
					fragment = new EStatementsFragment();
					((EStatementsFragment)fragment).Header = "Account eStatements";
					((EStatementsFragment)fragment).DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.AccountEStatements;
					break;
                case 2:
                    fragment = new EStatementsFragment();
                    ((EStatementsFragment)fragment).Header = "Credit Card eStatements";
                    ((EStatementsFragment)fragment).DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.CreditCardAnnualEStatements;
                    break;
                case 3:
                    fragment = new EStatementsFragment();
                    ((EStatementsFragment)fragment).Header = "eNotices";
                    ((EStatementsFragment)fragment).DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.ENotices;
                    break;
				case 4:
					fragment = new EStatementsFragment();
					((EStatementsFragment)fragment).Header = "Tax Documents";
					((EStatementsFragment)fragment).DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.TaxDocuments;
				    break;
            }

			if (fragment != null)
			{
				NavigationService.NavigatePush(fragment, true, false);
			}
		}
	}
}