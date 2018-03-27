using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.iOS.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts.SubAccounts
{
    public partial class SubAccountsFinishViewController : SubAccountsBaseContentViewController, ISubAccountsView
    {
        private string nextSteps;
        private bool creationSuccess;

        public SubAccountsFinishViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hides the remaining rows.
            tableForms.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            var response = ((SubAccountsViewController)ParentViewController.ParentViewController).CreateAccountResponse;
            creationSuccess = false;

            if (response != null && response.Success && !response.OutOfBandChallengeRequired && response.Result != null && response.Result.Success)
            {
                nextSteps = $"We have opened your new Smart Checking™ account.  Congratulations.\n\nThere are a few features that we offer you with your new product.  If you wish to add a joint owner, direct deposit, overdraft protection, order checks or a beneficiary to your new checking account, please contact the credit union at 800-999-5887 and we will send you the necessary paperwork to complete.  You can also request this paperwork via Message Center.  Thank you for being our Member!";
                labelNextSteps.ScrollRangeToVisible(new NSRange(0, 0));
                creationSuccess = true;
                GetDocuments();
                Logging.Track("Rocket Checking created.");
            }
            else if ((response != null && !response.OutOfBandChallengeRequired) || response == null)
            {
                nextSteps = "There was a problem opening your Smart Checking™ account.";
                Logging.Track("Rocket Checking creation failed.");
            }
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                if (creationSuccess)
                {
                    CultureTextProvider.SetMobileResourceText(labelNextSteps, cultureViewId, "001A085D-CC88-48C8-8546-A407ED44A4CF", nextSteps);
                }
                else
                {
                    CultureTextProvider.SetMobileResourceText(labelNextSteps, cultureViewId, "456A56AD-DBEC-4594-BAA0-B9645D306C88", nextSteps);
                }

                CultureTextProvider.SetMobileResourceText(txtAccountFormsHeading, cultureViewId, "EAB263F7-AFC1-4855-AE33-9C444956D86C", "Account Forms");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsFinishViewController:SetCultureConfiguration");
            }
        }

        private async void GetDocuments()
        {
            var methods = new DocumentMethods();
            var request = new EDocumentRequest
            {
                DocumentType = EDocumentTypes.SignatureCardForm.ToString()
            };

            ShowActivityIndicator();

            var response = await methods.GetEDocuments(request, View);

            HideActivityIndicator();

            if (response != null && response.Success)
            {
                var tableViewSource = new FormsTableViewSource(response.Result);
                tableViewSource.ItemSelected += ItemSelected;
                tableForms.Source = tableViewSource;
                tableForms.ReloadData();
            }
        }

        private void ItemSelected(ListViewItem item)
        {
            var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;

            // OnBase Document
            if (item.Data is ImageDocument)
            {
                documentViewerViewController.DocumentType = DocumentViewerTypes.DocumentCenterFile;

                var document = item.Data as ImageDocument;

                if (null != document)
                {
                    documentViewerViewController.Files = new List<DocumentCenterFile> { new DocumentCenterFile { FileId = document.DocumentId.ToString(), OnBaseImageDocumentType = EDocumentTypes.SignatureCardForm.ToString() } };
                }
            }

            // Direct Deposit Document
            if (item.Data is DocumentCenterFile)
            {
                documentViewerViewController.DocumentType = DocumentViewerTypes.Url;

                var document = item.Data as DocumentCenterFile;
                documentViewerViewController.Urls = new List<string> { document.URL };
            }

            if (documentViewerViewController.Files != null || documentViewerViewController.Urls != null)
            {
                NavigationController.PushViewController(documentViewerViewController, true);
            }
        }

        public string Validate()
        {
            return string.Empty;
        }
    }

    public class FormsTableViewSource : UITableViewSource
    {
        private List<ListViewItem> _model;
        public event Action<ListViewItem> ItemSelected = delegate { };

        public FormsTableViewSource(List<ImageDocument> documents)
        {
            _model = ViewUtilities.GetSubAccountsNextStepsDocuments(documents);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _model.Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _model[indexPath.Row];

            var cell = tableView.DequeueReusableCell("cellMain");

            var lblFormTitle = (UILabel)cell.ViewWithTag(100);
            lblFormTitle.Text = item.Item2Text;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ItemSelected(_model[indexPath.Row]);
            tableView.DeselectRow(indexPath, true);
        }
    }
}