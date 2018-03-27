using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.iOS.Common;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class DocumentCenterViewController : BaseViewController
	{
		public bool ShowDownloads { get; set; }
		private UIRefreshControl _refreshControl;
		private MemberDocumentCenter _viewModel;

		public DocumentCenterViewController(IntPtr handle) : base(handle)
		{
		}	

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_refreshControl = new UIRefreshControl();
			tableViewMain.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += (sender, e) => Refresh();
			segmentDocumentType.ValueChanged += (sender, e) => Refresh();
			segmentDocumentType.SelectedSegment = ShowDownloads ? 1 : 0;

			LoadDocuments();
		}

		public override void SetCultureConfiguration()
		{
		    Title = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "EFC795FA-785C-4C01-9A1D-EC4A6627754F", "Document Center");
			segmentDocumentType.SetTitle(CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "5EB188EB-0A6F-49CF-9ABD-09602DA1617D", "Sent"), 0);
			segmentDocumentType.SetTitle(CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "E45FC456-B7D3-4EEA-B1B9-9EA4D8D9B943", "Received"), 1);			
		}

		private void Refresh()
		{
			_viewModel = null;
			LoadDocuments();
		}

		private async void LoadDocuments()
		{
			if (_viewModel == null)
			{
				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

				var methods = new DocumentMethods();
				var request = new MemberDocumentCenterRequest();
				var response = await methods.GetMemberDocumentCenter(request, View);

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.EndRefreshing();
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

			var tableViewSource = new DocumentCenterTableViewSource(_viewModel, segmentDocumentType.SelectedSegment == 0);

			tableViewSource.ItemSelected += item =>
			{
				if (item is DocumentUpload)
				{
					var document = (DocumentUpload)item;

					if (document.StatusType == DocumentUploadStatusTypes.Accepted.ToString() ||
						document.StatusType == DocumentUploadStatusTypes.AwaitingApproval.ToString())
					{
						var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
						documentViewerViewController.Files = document.Files;
						NavigationController.PushViewController(documentViewerViewController, true);
					}
					else
					{
						var documentUploadViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentUploadViewController") as DocumentUploadViewController;
						documentUploadViewController.MaxNumberOfFiles = 2;

						documentUploadViewController.Completed += (files) =>
						{
							UploadFiles(files, ((DocumentUpload)item).Id);
						};

						NavigationController.PushViewController(documentUploadViewController, true);
					}
				}
				else if (item is DocumentDownload)
				{
					var document = (DocumentDownload)item;

					var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
					documentViewerViewController.Files = new List<DocumentCenterFile> { document.File };
					NavigationController.PushViewController(documentViewerViewController, true);
				}
			};

			tableViewMain.Source = tableViewSource;
			tableViewMain.ReloadData();
		}

		private async void UploadFiles(List<FileInformation> files, string documentId)
		{
			ShowActivityIndicator();

			var methods = new DocumentMethods();
			var response = await methods.UploadFiles(files, documentId, View);

			HideActivityIndicator();

			if (response == null || !response.Success)
			{
				var error = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "363dbc43-9530-4b81-a2f1-d55833786875", "Error uploading files.");
				var ok = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "5c884a6c-aa87-4bed-8c00-f7bbf53fe69d", "OK");
                await AlertMethods.Alert(View, "SunMobile", error, ok);
			}

			Refresh();
		}
	}
}