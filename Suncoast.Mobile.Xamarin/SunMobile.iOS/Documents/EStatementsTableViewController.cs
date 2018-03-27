using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.iOS.Common;
using SunMobile.Shared;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class EStatementsTableViewController : BaseTableViewController
	{
		public string Header { get; set; }
		public EDocumentTypes DocumentType { get; set; }
		private List<ImageDocument> _viewModel;
		private UIRefreshControl _refreshControl;
		private bool _enrolled;

		public EStatementsTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (!string.IsNullOrEmpty(Header))
			{
				Title = Header;
			}

			_refreshControl = new UIRefreshControl();
			TableView.AddSubview(_refreshControl);
			_refreshControl.ValueChanged += (sender, e) => Refresh();

			LoadDocuments();
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
				var methods = new DocumentMethods();
				var request = new EDocumentRequest
				{
					DocumentType = DocumentType.ToString(),
					EndTimeQuery = DateTime.UtcNow
				};

				switch (DocumentType)
				{					
					default:
						request.BeginTimeQuery = DateTime.UtcNow.AddYears(-2);
						break;
				}

				if (!_refreshControl.Refreshing)
				{
					ShowActivityIndicator();
				}

				switch (DocumentType)
				{
					case EDocumentTypes.AccountEStatements:
						_enrolled = await GetEnrollment(EDocumentTypes.AccountEStatements);
						break;
					case EDocumentTypes.ENotices:
						_enrolled = await GetEnrollment(EDocumentTypes.ENotices);
						break;
					default:
						_enrolled = true;
						break;
				}

				if (!_enrolled)
				{
					var documentTypeMessage = (DocumentType == EDocumentTypes.AccountEStatements) ? "eStatements" : "eNotices";
					var alertMessage = "You are not currently enrolled in " + documentTypeMessage + ". Would you like to enroll?";
					var alertResponseOptions = new string[] { "Accept", "Decline" };
					var alertResponse = await AlertMethods.Alert(View, documentTypeMessage + " Enrollment", alertMessage, alertResponseOptions);

					if (alertResponse == "Accept")
					{
						switch (DocumentType)
						{
							case EDocumentTypes.AccountEStatements:
								SetEnrollment(EDocumentTypes.AccountEStatements);
								_enrolled = true;
								break;
							case EDocumentTypes.ENotices:
								SetEnrollment(EDocumentTypes.ENotices);
								_enrolled = true;
								break;
						}
					}

					else if (alertResponse == "Decline")
					{
						NavigationController.PopViewController(true);
					}
				}

				if (_enrolled)
				{
					var response = await methods.GetEDocuments(request, View);

					if (response != null && response.Success)
					{
						_viewModel = response.Result;
					}
				}

				if (!_refreshControl.Refreshing)
				{
					HideActivityIndicator();
				}
				else
				{
					_refreshControl.EndRefreshing();
				}

				if (_viewModel == null)
				{
					_viewModel = new List<ImageDocument>();
				}

				switch (DocumentType)
				{
                    case EDocumentTypes.TaxDocuments:
						{
							var tableViewSource = new TaxDocumentsTableViewSource(_viewModel);
							tableViewSource.ItemSelected += ItemSelected;							
							TableView.Source = tableViewSource;
							TableView.ReloadData();
							break;
						}
					default:
						{
							var tableViewSource = new EStatementsTableViewSource(_viewModel, _enrolled);
							tableViewSource.ItemSelected += ItemSelected;
							TableView.Source = tableViewSource;
							TableView.ReloadData();
							break;
						}
				}

				switch (DocumentType)
				{
					case EDocumentTypes.AccountEStatements:
						Logging.Track("Starting EStatements.");
						break;
					case EDocumentTypes.ENotices:
						Logging.Track("Starting ENotices.");
						break;
					case EDocumentTypes.TaxDocuments:
						Logging.Track("Starting Tax Documents.");
						break;
				}
			}
		}

		private async Task<bool> GetEnrollment(EDocumentTypes documentType)
		{
            var returnValue = true;

			var request = new EDocumentIsEnrolledRequest { DocumentType = documentType.ToString() };

			var methods = new DocumentMethods();
			var response = await methods.IsEDocumentEnrolled(request, View);

            if (response != null)
            {
                returnValue = response.Result;
            }
			
            return returnValue;
		}

		private void ItemSelected(ListViewItem item)
		{
			var document = item.Data as ImageDocument;

			if (null != document)
			{
				var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
				documentViewerViewController.DocumentType = Shared.Data.DocumentViewerTypes.DocumentCenterFile;
                var documentCenterFile = new DocumentCenterFile { FileId = document.DocumentId.ToString(), OnBaseImageDocumentType = DocumentType.ToString(), MimeType = "application/pdf" };

                if (document.Images != null && document.Images.Count > 0 && document.Images[0].ImageStream != null)
                {
                    documentCenterFile.FileBytes = document.Images[0].ImageStream;
                }

                documentViewerViewController.Files = new List<DocumentCenterFile> { documentCenterFile };
				NavigationController.PushViewController(documentViewerViewController, true);
			}
		}

		private async void SetEnrollment(EDocumentTypes documentType)
		{
			var request = new EDocumentEnrollmentRequest
			{
				DocumentType = documentType.ToString(),
				EnrollmentFlag = true
			};

			var methods = new DocumentMethods();
			var response = await methods.SetEDocumentEnrollment(request, View);

			if (response == null || !response.Success)
			{
				await AlertMethods.Alert(View, "SunMobile", "Unable to update enrollment.", "OK");
			}
		}
	}
}