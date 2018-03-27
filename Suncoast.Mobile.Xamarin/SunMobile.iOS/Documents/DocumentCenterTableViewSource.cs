using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public class DocumentCenterTableViewSource : UITableViewSource
	{
		public event Action<object> ItemSelected = delegate { };
		private MemberDocumentCenter _model = new MemberDocumentCenter();
		private bool _isUploadDocuments = true;
		private List<DocumentUpload> _documentsActionRequired;
		private List<DocumentUpload> _documentsAwaitingApproval;
		private List<DocumentUpload> _documentsCompleted;
		private List<DocumentDownload> _documentsDownloaded;

		public DocumentCenterTableViewSource(MemberDocumentCenter model, bool isUploadDocuments)
		{
			try
			{
				_model = model;
				_isUploadDocuments = isUploadDocuments;

				_documentsActionRequired = new List<DocumentUpload>();
				_documentsAwaitingApproval = new List<DocumentUpload>();
				_documentsCompleted = new List<DocumentUpload>();
				_documentsDownloaded = new List<DocumentDownload>();

				if (_model.UploadDocuments != null)
				{
					_documentsActionRequired.AddRange(_model.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.Requested.ToString()));
					_documentsActionRequired.AddRange(_model.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.Rejected.ToString()));
					_documentsAwaitingApproval.AddRange(_model.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.AwaitingApproval.ToString()));
					_documentsCompleted.AddRange(_model.UploadDocuments.FindAll(x => x.StatusType == DocumentUploadStatusTypes.Accepted.ToString()));
				}

				if (_model.DownloadDocuments != null)
				{
					_documentsDownloaded.AddRange(_model.DownloadDocuments);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DocumentCenterTableViewSource");
			}
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			nint returnValue = 0;

			if (_isUploadDocuments)
			{
				switch (section)
				{
					case 0:
						returnValue = _documentsActionRequired.Count();
						break;
					case 1:
						returnValue = _documentsAwaitingApproval.Count();
						break;
					case 2:
						returnValue = _documentsCompleted.Count();
						break;
				}
			}
			else
			{
				returnValue = _documentsDownloaded.Count();
			}

			return returnValue;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			var returnValue = 1;

			if (_isUploadDocuments)
			{
				returnValue = 3;
			}

			return returnValue;
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			var view = new UIView(new CGRect(0, 0, 320, 28));
			view.BackgroundColor = AppStyles.TableHeaderBackgroundColor;

			var label = new UILabel();
			label.BackgroundColor = UIColor.Clear;
			label.Opaque = false;
			label.TextColor = AppStyles.TitleBarItemTintColor;
			label.Font = UIFont.FromName("Helvetica", 16f);
			label.Frame = new CGRect(15, 2, 290, 24);

			var actionRequired = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "08f39e56-5ea4-49db-8cab-d0a8bc1d9783", "Action Required ");
			var awaitingFileReview = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "58aacc79-d0f0-4636-b308-26a1f1bb4fc5", "Awaiting File Review");
			var completedUploads = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "2a42206d-80cb-4b5f-a4fd-d0e9285d6112", "Completed Uploads");
			var documentsReceived = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "8a3daffe-ec3d-40d1-9dc7-82f32a021bf1", "Documents Received");

			if (_isUploadDocuments)
			{
				switch (section)
				{
					case 0:
                        label.Text = _documentsActionRequired.Count > 0 ? actionRequired : string.Empty;
						break;
					case 1:
                        label.Text = _documentsAwaitingApproval.Count > 0 ? awaitingFileReview : string.Empty;
						break;
					case 2:
                        label.Text = _documentsCompleted.Count > 0 ? completedUploads : string.Empty;
						break;
				}
			}
			else
			{
                label.Text = documentsReceived;
			}

			view.AddSubview(label);

			return view;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			nfloat returnValue = 28;

			if (_isUploadDocuments)
			{
				switch (section)
				{
					case 0:
						if (_documentsActionRequired.Count <= 0)
						{
							returnValue = 0;
						}
						break;
					case 1:
						if (_documentsAwaitingApproval.Count <= 0)
						{
							returnValue = 0;
						}
						break;
					case 2:
						if (_documentsCompleted.Count <= 0)
						{
							returnValue = 0;
						}
						break;
				}
			}

			return returnValue;
		}

		public override nfloat GetHeightForFooter(UITableView tableView, nint section)
		{
			return 0.1f;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (_isUploadDocuments)
			{
				var document = (DocumentUpload)GetDocument(indexPath);

				if (document.StatusType != DocumentUploadStatusTypes.Rejected.ToString())
				{
					return 50;
				}
			}
			else
			{
				return 62;
			}

			return tableView.RowHeight;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ItemSelected(GetDocument(indexPath));
			tableView.DeselectRow(indexPath, true);
		}

		private object GetDocument(NSIndexPath indexPath)
		{
			object returnValue = null;

			if (_isUploadDocuments)
			{
				switch (indexPath.Section)
				{
					case 0:
						returnValue = _documentsActionRequired[indexPath.Row];
						break;
					case 1:
						returnValue = _documentsAwaitingApproval[indexPath.Row];
						break;
					case 2:
						returnValue = _documentsCompleted[indexPath.Row];
						break;
				}
			}
			else
			{
				returnValue = _documentsDownloaded[indexPath.Row];
			}

			return returnValue;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("cellMain");

			var lblDate = (UILabel)cell.ViewWithTag(100);
			var lblTitle = (UILabel)cell.ViewWithTag(200);
			var lblStatus = (UILabel)cell.ViewWithTag(300);
			var lblStatusDescription = (UILabel)cell.ViewWithTag(400);
			var btnUpload = (UIButton)cell.ViewWithTag(500);

			lblStatus.Text = string.Empty;
			lblStatusDescription.Text = string.Empty;
			btnUpload.Hidden = true;

			if (_isUploadDocuments)
			{
				var document = (DocumentUpload)GetDocument(indexPath);

				lblDate.Text = document.DocumentTimeUtc.ToShortDateString();
				lblTitle.Text = document.Title;
				lblStatusDescription.Text = document.StatusDescription;

				if (document.StatusType == DocumentUploadStatusTypes.Requested.ToString() ||
					document.StatusType == DocumentUploadStatusTypes.Rejected.ToString())
				{
					btnUpload.Hidden = false;
					lblStatus.Text = document.StatusType;
					lblStatus.TextColor = document.StatusType == DocumentUploadStatusTypes.Rejected.ToString() ? UIColor.Red : UIColor.Black;

					if (!string.IsNullOrEmpty(lblStatusDescription.Text))
					{
						CommonMethods.SizeLabelToMaxNumberOfLines(lblStatusDescription, 2);
					}
				}
			}
			else
			{
				var document = (DocumentDownload)GetDocument(indexPath);
				lblDate.Text = document.DocumentTimeUtc.ToShortDateString();
				lblTitle.Text = document.Title.Trim();
			}

			CommonMethods.SizeLabelToMaxNumberOfLines(lblTitle, 2);

			return cell;
		}
	}
}