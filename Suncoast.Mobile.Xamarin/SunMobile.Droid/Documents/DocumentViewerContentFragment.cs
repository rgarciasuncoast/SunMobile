using System;
using System.IO;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Shared;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;
using Syncfusion.SfPdfViewer.Android;

namespace SunMobile.Droid.Documents
{
    public class DocumentViewerContentFragment : BaseFragment
    {
        public DocumentCenterFile File { get; set; }
        public byte[] FileBytes { get; set; }
        private WebView webView;
        private SfPdfViewer pdfViewer;
        private Bundle _webViewBundle;
        private string _tempFileName;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RetainInstance = true;

            ((MainActivity)Activity).SetActionBarTitle("Document Viewer");

            var view = (LinearLayout)inflater.Inflate(Resource.Layout.DocumentViewerContentView, container, false);

            webView = view.FindViewById<WebView>(Resource.Id.webView1);
            webView.SetWebViewClient(new CustomWebViewClient());
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.AllowFileAccess = true;
            webView.Settings.AllowUniversalAccessFromFileURLs = true;
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.DisplayZoomControls = false;
            webView.Settings.SetPluginState(WebSettings.PluginState.On);
            webView.Settings.LoadWithOverviewMode = true;
            webView.Settings.UseWideViewPort = true;
            webView.Visibility = ViewStates.Gone;

            pdfViewer = view.FindViewById<SfPdfViewer>(Resource.Id.pdfViewer);
            pdfViewer.AnnotationMode = AnnotationMode.None;
            pdfViewer.IsTextSearchEnabled = false;
            pdfViewer.IsTextSelectionEnabled = true;
            pdfViewer.Visibility = ViewStates.Gone;

            LoadFile();

            return view;
        }

        private async void LoadFile()
        {
            if (File != null && string.IsNullOrEmpty(File.FileName))
            {
                File.FileName = File.FileId + "." + DocumentMethods.GetFileExtensionFromMimeType(File.MimeType);
            }

            if (File != null && File.FileBytes != null)
            {
                FileBytes = File.FileBytes;
                File.MimeType = "application/pdf";
            }

            if (FileBytes == null && !string.IsNullOrEmpty(File.URL))
            {
                var webClient = new System.Net.WebClient();

                FileBytes = webClient.DownloadData(File.URL);
                File.MimeType = "application/pdf";
            }
            else if (!string.IsNullOrEmpty(File.OnBaseImageDocumentType))
            {
                if (FileBytes == null)
                {
                    var methods = new DocumentMethods();
                    var request = new EDocumentRequest { DocumentId = int.Parse(File.FileId), DocumentType = File.OnBaseImageDocumentType };

                    ShowActivityIndicator();

                    var response = await methods.GetEDocuments(request, Activity);

                    HideActivityIndicator();

                    if (response != null && response.Success && response.Result != null && response.Result.Count > 0 && response.Result[0].Images != null && response.Result[0].Images.Count > 0 && response.Result[0].Images[0].ImageStream != null)
                    {
                        FileBytes = response.Result[0].Images[0].ImageStream;
                        File.MimeType = "application/pdf";
                    }
                }
            }
            else
            {
                if (FileBytes == null)
                {
                    var methods = new DocumentMethods();
                    var request = new DocumentCenterFileRequest { FileId = File.FileId };

                    ShowActivityIndicator();

                    var response = await methods.GetDocumentCenterFile(request, Activity);

                    HideActivityIndicator();

                    if (response != null && response.Success && response.Result != null)
                    {
                        FileBytes = response.Result.FileBytes;
                        File.MimeType = response.Result.FileInfo.MimeType;
                    }
                }
            }

            // If we can't load the file, display the no documents image.
            if (FileBytes == null)
            {
                var stream = Context.Resources.OpenRawResource(Resource.Drawable.nodocuments);
                FileBytes = Images.ConvertStreamToByteArray(stream);
                File.MimeType = "image/jpeg";
            }

            if (FileBytes != null)
            {
                var fileExtension = DocumentMethods.GetFileExtensionFromMimeType(File.MimeType);

                if (fileExtension != "pdf")
                {
                    var htmlString = Convert.ToBase64String(FileBytes);
                    webView.Visibility = ViewStates.Visible;
                    webView.LoadData(htmlString, File.MimeType, "base64");
                }
                else
                {
                    var stream = new MemoryStream(FileBytes);
                    pdfViewer.Visibility = ViewStates.Visible;
                    pdfViewer.LoadDocument(stream);

                    // Load WebView for Printing / Sharing
                    var fileName = Guid.NewGuid() + ".pdf";
                    _tempFileName = IsolatedStorage.SaveBytesToFile(fileName, FileBytes);
                    var urlFileName = $"file://{_tempFileName}";
                    webView.LoadUrl($"file:///android_asset/pdfjs/web/viewer.html?file={urlFileName}");
                }
            }
        }

        public WebView GetWebView()
        {
            return webView;
        }

        public override void OnPause()
        {
            base.OnPause();

            _webViewBundle = new Bundle();
            webView.SaveState(_webViewBundle);
        }

        public override void OnStop()
        {
            IsolatedStorage.DeleteFile(_tempFileName);

            base.OnStop();
        }
    }

#pragma warning disable CS0672 // Member overrides obsolete member
    public class CustomWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            return false;
        }
    }
#pragma warning restore CS0672 // Member overrides obsolete member
}