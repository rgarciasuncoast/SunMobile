using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.iOS.Common;
using SunMobile.iOS.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Methods;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class EStatementOptionsTableViewController : BaseTableViewController
	{
        private bool _isCombined;
        private bool _hideAllRows;

		public EStatementOptionsTableViewController(IntPtr handle) : base(handle)
		{
            _hideAllRows = true;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Hides the remaining rows.
			tableMain.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			switchAccountEStatementEnrollment.ValueChanged += (sender, e) => SetEnrollment(EDocumentTypes.AccountEStatements, sender);
			switchENoticeEnrollment.ValueChanged += (sender, e) => SetEnrollment(EDocumentTypes.ENotices, sender);
            switchENoticeAndEStatementEnrollment.ValueChanged += (sender, e) => 
            {
                SetEnrollment(EDocumentTypes.AccountEStatements, sender);
                SetEnrollment(EDocumentTypes.ENotices, sender);
            };
			btnViewStatementDisclosure.TouchUpInside += (sender, e) => ViewDisclosure();

			GetEnrollment();
		}

		private async void GetEnrollment()
		{
			var methods = new DocumentMethods();
			

			ShowActivityIndicator();

            var eStatementRequest = new EDocumentIsEnrolledRequest { DocumentType = EDocumentTypes.AccountEStatements.ToString() };
            var eStatementResponse = await methods.IsEDocumentEnrolled(eStatementRequest, View);
            var eNoticeRequest = new EDocumentIsEnrolledRequest { DocumentType = EDocumentTypes.ENotices.ToString() };
            var eNoticeResponse = await methods.IsEDocumentEnrolled(eNoticeRequest, View);

            HideActivityIndicator();            

            if (eStatementResponse != null && eStatementResponse.Success)
            {
                switchAccountEStatementEnrollment.On = eStatementResponse.Result;
            }

            if (eNoticeResponse != null && eNoticeResponse.Success)
            {
                switchENoticeEnrollment.On = eNoticeResponse.Result;
            }

            _hideAllRows = false;

            CombineIfMatch();
		}

        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            if (_hideAllRows)
            {
                return 0;
            }

            if (indexPath.Row < 2 && _isCombined)
            {
                return 0;
            }

            if (indexPath.Row == 2 && !_isCombined)
            {
                return 0;
            }

            return base.GetHeightForRow(tableView, indexPath);
        }

        private void CombineIfMatch()
        {
            if (!_isCombined && switchAccountEStatementEnrollment.On == switchENoticeEnrollment.On)
            {
                _isCombined = true;
                switchENoticeAndEStatementEnrollment.On = switchENoticeEnrollment.On;
            }

            tableMain.ReloadData();
        }

		private async void SetEnrollment(EDocumentTypes documentType, object sender)
		{
			var request = new EDocumentEnrollmentRequest
			{
				DocumentType = documentType.ToString(),
				EnrollmentFlag = ((UISwitch)sender).On
			};

			var methods = new DocumentMethods();
			var response = await methods.SetEDocumentEnrollment(request, View);

			if (response == null || !response.Success)
			{
				await AlertMethods.Alert(View, "SunMobile", "Unable to update enrollment.", "OK");
			}

            CombineIfMatch();
		}

		private void ViewDisclosure()
		{
            var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
            documentViewerViewController.DocumentType = Shared.Data.DocumentViewerTypes.Url;
            documentViewerViewController.Urls = new List<string> { "https://sunblockstorage.blob.core.windows.net/documents/eStatement-Disclosure.pdf" };
            NavigationController.PushViewController(documentViewerViewController, true);
		}
	}
}