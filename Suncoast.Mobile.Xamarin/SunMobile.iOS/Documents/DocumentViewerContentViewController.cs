using System;
using System.Net;
using Foundation;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.iOS.Common;
using SunMobile.Shared;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class DocumentViewerContentViewController : BaseViewController
	{
		public DocumentCenterFile File { get; set; }
		public string Url { get; set; }
		public byte[] FileBytes { get; set; }
		public int PageIndex { get; set; }

		public DocumentViewerContentViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			LoadFile();
		}

		private async void LoadFile()
		{
			if (File == null)
			{
				File = new DocumentCenterFile();
			}

            if (File.FileBytes != null)
            {
                FileBytes = File.FileBytes;

                if (string.IsNullOrEmpty(File.MimeType))
                {
                    File.MimeType = "application/pdf";
                }
            }
			else if (!string.IsNullOrEmpty(File.OnBaseImageDocumentType))
			{
				if (FileBytes == null)
				{
					var methods = new DocumentMethods();
					var request = new EDocumentRequest { DocumentId = int.Parse(File.FileId), DocumentType = File.OnBaseImageDocumentType };

					ShowActivityIndicator();

					var response = await methods.GetEDocuments(request, View);

					HideActivityIndicator();

					if (response != null && response.Success && response.Result != null && response.Result.Count > 0 && response.Result[0].Images != null && response.Result[0].Images.Count > 0 && response.Result[0].Images[0].ImageStream != null)
					{
						FileBytes = response.Result[0].Images[0].ImageStream;
						File.MimeType = "application/pdf";
					}
				}
			}
			else if (!string.IsNullOrEmpty(Url))
			{
				if (FileBytes == null)
				{
					var webClient = new WebClient();

					ShowActivityIndicator();

					FileBytes = webClient.DownloadData(Url);
					File.MimeType = "application/pdf";

					HideActivityIndicator();
				}
			}
			else
			{
				if (File != null && string.IsNullOrEmpty(File.FileName))
				{
					File.FileName = File.FileId + "." + DocumentMethods.GetFileExtensionFromMimeType(File.MimeType);
				}

				if (FileBytes == null)
				{
					var methods = new DocumentMethods();
					var request = new DocumentCenterFileRequest { FileId = File.FileId };

					ShowActivityIndicator();

					var response = await methods.GetDocumentCenterFile(request, View);

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
				FileBytes = System.IO.File.ReadAllBytes("nodocuments.png");
				File.MimeType = "image/jpeg";
			}

			if (FileBytes != null)
			{
				var data = NSData.FromArray(FileBytes);
				webView.LoadData(data, File.MimeType, "", new NSUrl(""));
				webView.ScalesPageToFit = true;
			}
		}

		public UIWebView GetWebView()
		{
			return webView;
		}	
	}
}