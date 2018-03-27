using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.Droid.Common;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Documents
{
    public class DocumentCenterFragment : BaseListFragment
    {
        public bool ShowDownloads { get; set; }
        public List<DocumentCenterFile> Documents { get; set; }
        private RadioButton btnUploads;
        private RadioButton btnDownloads;
        private SwipeRefreshLayout refresher;
        private SegmentedGroup segmentDocumentType;
        private MemberDocumentCenter _viewModel;
        private DocumentsListAdapter _documentsListAdapter;
        private bool _downloadsSelected;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DocumentsCenterListView, null);
            RetainInstance = true;

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "EFC795FA-785C-4C01-9A1D-EC4A6627754F", "Document Center"));
                CultureTextProvider.SetMobileResourceText(btnUploads, "441F87A2-7C3B-4296-A017-999BDB2BE512", "5EB188EB-0A6F-49CF-9ABD-09602DA1617D", "Sent");
                CultureTextProvider.SetMobileResourceText(btnDownloads, "441F87A2-7C3B-4296-A017-999BDB2BE512", "E45FC456-B7D3-4EEA-B1B9-9EA4D8D9B943", "Received");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "DocumentCenterFragment:SetCultureConfiguration");
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            base.SetupView();

            refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.documentsRefresher);
            refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
            refresher.Refresh += (sender, e) => { LoadDocuments(); };

            btnUploads = Activity.FindViewById<RadioButton>(Resource.Id.btnUploads);
            btnDownloads = Activity.FindViewById<RadioButton>(Resource.Id.btnDownloads);
            if (ShowDownloads)
            {
                btnDownloads.Selected = true;
                _downloadsSelected = true;
                btnDownloads.PerformClick();
            }
            else
            {
                btnUploads.Selected = true;
            }

            btnUploads.Click += (sender, e) => { LoadDocuments(); };
            btnDownloads.Click += (sender, e) => { LoadDocuments(); };

            segmentDocumentType = Activity.FindViewById<SegmentedGroup>(Resource.Id.segmentDocumentType);
            segmentDocumentType.CheckedChange += (sender, e) =>
            {
                _downloadsSelected = segmentDocumentType.CheckedRadioButtonId == Resource.Id.btnDownloads;
            };

            ListViewMain.ItemClick += (sender, e) =>
            {
                var item = _documentsListAdapter.GetListViewItem(e.Position).Data;

                if (item is DocumentUpload)
                {
                    var document = (DocumentUpload)item;

                    if (document.StatusType == DocumentUploadStatusTypes.Accepted.ToString() ||
                        document.StatusType == DocumentUploadStatusTypes.AwaitingApproval.ToString())
                    {
                        var documentViewerViewPagerFragment = new DocumentViewerViewPagerFragment();
                        documentViewerViewPagerFragment.Files = document.Files;
                        NavigationService.NavigatePush(documentViewerViewPagerFragment, true, false, true);
                    }
                    else
                    {
                        var documentUploadFragment = new DocumentUploadFragment();
                        documentUploadFragment.MaxNumberOfFiles = 2;

                        documentUploadFragment.Completed += (files) =>
                        {
                            UploadFiles(files, ((DocumentUpload)item).Id);
                        };

                        NavigationService.NavigatePush(documentUploadFragment, true, false, true);
                    }
                }
                else if (item is DocumentDownload)
                {
                    var document = (DocumentDownload)item;

                    var documentViewerViewPagerFragment = new DocumentViewerViewPagerFragment();
                    documentViewerViewPagerFragment.Files = new List<DocumentCenterFile> { document.File };
                    NavigationService.NavigatePush(documentViewerViewPagerFragment, true, false, true);
                }
            };

            LoadDocuments();

            if (Documents != null && Documents.Count > 0)
            {
                var documentViewerViewPagerFragment = new DocumentViewerViewPagerFragment();
                documentViewerViewPagerFragment.Files = Documents;
                NavigationService.NavigatePush(documentViewerViewPagerFragment, true, false, true);

                Documents = null;
            }
        }

        private void Refresh()
        {
            _viewModel = null;
            LoadDocuments();
        }

        private async void LoadDocuments()
        {
            try
            {
                if (refresher.Refreshing)
                {
                    _viewModel = null;
                }

                if (_viewModel == null)
                {
                    if (!refresher.Refreshing)
                    {
                        ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "B5B08F6D-3B69-472A-B5C5-19948065BCC3", "Loading Documents . . ."));
                    }

                    var methods = new DocumentMethods();
                    var request = new MemberDocumentCenterRequest();
                    var response = await methods.GetMemberDocumentCenter(request, Activity);

                    if (!refresher.Refreshing)
                    {
                        HideActivityIndicator();
                    }
                    else
                    {
                        refresher.Refreshing = false;
                    }

                    if (response != null && response.Success)
                    {
                        _viewModel = response.Result;
                    }
                }

                if (_viewModel == null)
                {
                    _viewModel = new MemberDocumentCenter();
                }

                if (_viewModel != null)
                {
                    var documentsActionRequired = new List<DocumentUpload>();
                    var documentsAwaitingApproval = new List<DocumentUpload>();
                    var documentsCompleted = new List<DocumentUpload>();
                    var documentsDownloaded = new List<DocumentDownload>();

                    if (_viewModel.UploadDocuments != null)
                    {
                        documentsActionRequired.AddRange(_viewModel.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.Requested.ToString()));
                        documentsActionRequired.AddRange(_viewModel.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.Rejected.ToString()));
                        documentsAwaitingApproval.AddRange(_viewModel.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.AwaitingApproval.ToString()));
                        documentsCompleted.AddRange(_viewModel.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.Accepted.ToString()));
                    }

                    if (_viewModel.DownloadDocuments != null)
                    {
                        documentsDownloaded.AddRange(_viewModel.DownloadDocuments);
                    }

                    var listViews = new List<ListViewItem>();

                    var isUpload = !_downloadsSelected;

                    var actionRequired = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "08f39e56-5ea4-49db-8cab-d0a8bc1d9783", "Action Required ");
                    var awaitingFileReview = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "58aacc79-d0f0-4636-b308-26a1f1bb4fc5", "Awaiting File Review");
                    var completedUploads = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "2a42206d-80cb-4b5f-a4fd-d0e9285d6112", "Completed Uploads");
                    var downloadDocuments = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "AB7E03D0-7C58-432E-A784-5CE530DB45DE", "Download Documents");

                    if (isUpload)
                    {
                        var lists = new List<List<DocumentUpload>>();
                        lists.Add(documentsActionRequired);
                        lists.Add(documentsAwaitingApproval);
                        lists.Add(documentsCompleted);

                        var headers = new List<string>();
                        if (documentsActionRequired.Count > 0)
                        {
                            headers.Add(actionRequired);
                        }
                        if (documentsAwaitingApproval.Count > 0)
                        {
                            headers.Add(awaitingFileReview);
                        }
                        if (documentsCompleted.Count > 0)
                        {
                            headers.Add(completedUploads);
                        }

                        listViews = ViewUtilities.ConvertDocumentUploadsToListViewItems(lists, headers);
                    }
                    else
                    {
                        var header = downloadDocuments;
                        listViews = ViewUtilities.ConvertDocumentDownloadsToListViewItems(documentsDownloaded, header);
                    }

                    _documentsListAdapter = new DocumentsListAdapter(Activity, isUpload, listViews);
                    ListAdapter = _documentsListAdapter;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "DocumentsCenterFragment:LoadDocuments");
            }
        }

        private async void UploadFiles(List<FileInformation> files, string documentId)
        {
            if (files != null && files.Count > 0)
            {
                ShowActivityIndicator();

                var methods = new DocumentMethods();
                var response = await methods.UploadFiles(files, documentId, Activity);

                HideActivityIndicator();

                if (response == null || !response.Success)
                {
                    var error = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "363dbc43-9530-4b81-a2f1-d55833786875", "Error uploading files.");
                    var ok = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "5c884a6c-aa87-4bed-8c00-f7bbf53fe69d", "OK");
                    await AlertMethods.Alert(Activity, error, ok);
                }

                Refresh();
            }
        }
    }
}