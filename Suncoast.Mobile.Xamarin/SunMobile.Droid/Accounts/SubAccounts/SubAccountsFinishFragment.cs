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
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts.SubAccounts
{
    public class SubAccountsFinishFragment : SubAccountsBaseContentFragment, ISubAccountsView
    {
        private ListView listViewMain;
        private TextView labelNextSteps;
        private TextView txtAccountFormsHeading;
        private GenericListAdapter _listAdapter;
        private StatusResponse<List<ImageDocument>> _eDocumentsResponse;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SubAccountsFinishView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            labelNextSteps = Activity.FindViewById<TextView>(Resource.Id.labelNextSteps);
            txtAccountFormsHeading = Activity.FindViewById<TextView>(Resource.Id.txtAccountFormsHeading);
            listViewMain = Activity.FindViewById<ListView>(Resource.Id.listViewMain);
            listViewMain.ItemClick += (sender, e) =>
            {
                var item = _listAdapter.GetListViewItem(e.Position);
                ItemSelected(item);
            };

            if (savedInstanceState != null)
            {
                var json = savedInstanceState.GetString("EDocumentsResponse");
                _eDocumentsResponse = JsonConvert.DeserializeObject<StatusResponse<List<ImageDocument>>>(json);
            }

            ClearAll();

            CreateAndDisplayRocketCheckingResult();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            var json = JsonConvert.SerializeObject(_eDocumentsResponse);
            outState.PutString("EDocumentsResponse", json);

            base.OnSaveInstanceState(outState);
        }

        private void ClearAll()
        {
            labelNextSteps.Text = string.Empty;
        }

        public async void CreateAndDisplayRocketCheckingResult()
        {
            await CreateRocketAccount(false);

            var createRocketCheckingResponse = SessionSettings.Instance.CreateRocketCheckingResponse;

            CultureTextProvider.SetMobileResourceText(txtAccountFormsHeading, cultureViewId, "EAB263F7-AFC1-4855-AE33-9C444956D86C", "Account Forms");

            if (createRocketCheckingResponse != null && createRocketCheckingResponse.Success && createRocketCheckingResponse.Result != null && createRocketCheckingResponse.Result.Success)
            {
                CultureTextProvider.SetMobileResourceText(labelNextSteps, cultureViewId, "001A085D-CC88-48C8-8546-A407ED44A4CF", "We have opened your new Smart Checking™ account.  Congratulations.\n\nThere are a few features that we offer you with your new product.  If you wish to add a joint owner, direct deposit, overdraft protection, order checks or a beneficiary to your new checking account, please contact the credit union at 800-999-5887 and we will send you the necessary paperwork to complete.  You can also request this paperwork via Message Center.  Thank you for being our Member!");
                GetDocuments();

                Logging.Track("Rocket Checking created.");
            }
            else
            {
                CultureTextProvider.SetMobileResourceText(labelNextSteps, cultureViewId, "001A085D-CC88-48C8-8546-A407ED44A4CF", "There was a problem opening your Smart Checking™ account.");
                Logging.Track("Rocket Checking creation failed.");
            }
        }

        private async void GetDocuments()
        {
            if (_eDocumentsResponse == null)
            {
                var methods = new DocumentMethods();
                var request = new EDocumentRequest
                {
                    DocumentType = EDocumentTypes.SignatureCardForm.ToString()
                };

                ShowActivityIndicator();

                _eDocumentsResponse = await methods.GetEDocuments(request, Activity);

                HideActivityIndicator();
            }

            if (_eDocumentsResponse != null && _eDocumentsResponse.Success)
            {
                // Display the disclosures              
                var listViewItems = ViewUtilities.GetSubAccountsNextStepsDocuments(_eDocumentsResponse.Result);
                int[] resourceIds = { Resource.Id.lblItem1Text };
                string[] fields = { "Item1Text" };
                _listAdapter = new GenericListAdapter(Activity, Resource.Layout.StringListViewItem, listViewItems, resourceIds, fields);
                listViewMain.Adapter = _listAdapter;
            }
        }

        private void ItemSelected(ListViewItem item)
        {
            var documentViewer = new DocumentViewerViewPagerFragment();

            // OnBase Document
            if (item.Data is ImageDocument)
            {
                var document = item.Data as ImageDocument;

                if (null != document)
                {
                    documentViewer.Files = new List<DocumentCenterFile> { new DocumentCenterFile { FileId = document.DocumentId.ToString(), OnBaseImageDocumentType = EDocumentTypes.SignatureCardForm.ToString() } };
                }
            }

            // Direct Deposit Document
            if (item.Data is DocumentCenterFile)
            {
                var document = item.Data as DocumentCenterFile;

                if (null != document)
                {
                    documentViewer.Files = new List<DocumentCenterFile> { new DocumentCenterFile { URL = document.URL } };
                }
            }

            if (documentViewer.Files != null)
            {
                NavigationService.NavigatePush(documentViewer, true, false);
            }
        }

        public string Validate()
        {
            return string.Empty;
        }
    }
}