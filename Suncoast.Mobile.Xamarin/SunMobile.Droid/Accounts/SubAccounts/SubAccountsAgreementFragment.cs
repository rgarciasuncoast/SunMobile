using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Droid.Common;
using SunMobile.Droid.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts.SubAccounts
{
    public class SubAccountsAgreementFragment : SubAccountsBaseContentFragment, ISubAccountsView
    {
        private ListView listViewMain;
        private TableRow rowEStatements;
        private TextView lblEnroll;
        private Switch switchEnrollInEStatements;
        private TextView lblAgree;
        private Switch switchAgree;
        private GenericListAdapter _listAdapter;
        private StatusResponse<bool> _eDocumentEnrolledResponse;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SubAccountsAgreementView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            rowEStatements = Activity.FindViewById<TableRow>(Resource.Id.rowEStatements);
            switchEnrollInEStatements = Activity.FindViewById<Switch>(Resource.Id.switchEnrollInEStatements);
            switchAgree = Activity.FindViewById<Switch>(Resource.Id.switchAgree);
            lblEnroll = Activity.FindViewById<TextView>(Resource.Id.lblEnrollInEstatements);
            lblAgree = Activity.FindViewById<TextView>(Resource.Id.lblIAgree);

            rowEStatements.Visibility = ViewStates.Gone;

            listViewMain = Activity.FindViewById<ListView>(Resource.Id.listViewMain);
            listViewMain.ItemClick += (sender, e) =>
            {
                try
                {
                    var item = _listAdapter.GetListViewItem(e.Position);
                    var documentViewerViewPagerFragment = new DocumentViewerViewPagerFragment();
                    documentViewerViewPagerFragment.Files = new List<DocumentCenterFile> { new DocumentCenterFile { URL = item.Item2Text } };
                    NavigationService.NavigatePush(documentViewerViewPagerFragment, true, false, true);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, "SubAccountsAgreementFragment:LoadDisclosures");
                }
            };

            if (savedInstanceState != null)
            {
                var json = savedInstanceState.GetString("EDocumentEnrollment");
                _eDocumentEnrolledResponse = JsonConvert.DeserializeObject<StatusResponse<bool>>(json);
            }

            LoadDisclosures();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            var json = JsonConvert.SerializeObject(_eDocumentEnrolledResponse);
            outState.PutString("EDocumentEnrollment", json);

            base.OnSaveInstanceState(outState);
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(lblEnroll, cultureViewId, "69D5A8BE-ADF1-4016-B169-23E818D447E6", "Enroll in eStatements");
                CultureTextProvider.SetMobileResourceText(lblAgree, cultureViewId, "F392F568-8E18-4912-AA99-A050CBD75BA2", "I agree to the terms and" +
                " conditions of the above agreements");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsAgreementFragment:SetCultureConfiguration");
            }

        }


        private async void LoadDisclosures()
        {
            // See if the member is enrolled in eStatements.  If so, hide the enrollment switch
            if (_eDocumentEnrolledResponse == null)
            {
                var methods = new DocumentMethods();
                var request = new EDocumentIsEnrolledRequest { DocumentType = EDocumentTypes.AccountEStatements.ToString() };

                ShowActivityIndicator();

                _eDocumentEnrolledResponse = await methods.IsEDocumentEnrolled(request, Activity);

                HideActivityIndicator();
            }

            bool isEnrolledInEStatements = false;

            if (_eDocumentEnrolledResponse != null && _eDocumentEnrolledResponse.Result)
            {
                isEnrolledInEStatements = true;
            }
            else
            {
                rowEStatements.Visibility = ViewStates.Visible;
            }

            // Display the disclosures          
            var listViewItems = ViewUtilities.GetSubAccountsDisclosures(isEnrolledInEStatements);

            int[] resourceIds = { Resource.Id.lblItem1Text };
            string[] fields = { "Item1Text" };
            _listAdapter = new GenericListAdapter(Activity, Resource.Layout.StringListViewItem, listViewItems, resourceIds, fields);
            listViewMain.Adapter = _listAdapter;
        }

        public string Validate()
        {
            var returnValue = string.Empty;

            if (!switchAgree.Checked)
            {
                returnValue = CultureTextProvider.GetMobileResourceText(cultureViewId, "DD526FEB-D214-453D-9994-4BECC9C1156F", "You must agree to the terms and conditions before continuing.");
            }

            Info.EnrollInEstatements = switchEnrollInEStatements.Checked;

            return returnValue;
        }
    }
}