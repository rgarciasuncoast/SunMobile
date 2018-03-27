using System;
using System.Collections.Generic;
using Foundation;
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
    public partial class SubAccountsAgreementViewController : SubAccountsBaseContentViewController, ISubAccountsView
    {
        private bool _agreementAccepted;

        public SubAccountsAgreementViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hides the remaining rows.
            mainTableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

            switchAgree.ValueChanged += (sender, e) =>
            {
                _agreementAccepted = switchAgree.On;
            };

            lblEnrollInEstatements.Hidden = true;
            switchEnrollInEStatements.Hidden = true;

            LoadDisclosures();
        }

        public override void SetCultureConfiguration()
        {
            try
            {
                CultureTextProvider.SetMobileResourceText(lblEnrollInEstatements, cultureViewId, "69D5A8BE-ADF1-4016-B169-23E818D447E6", "Enroll in eStatements");
                CultureTextProvider.SetMobileResourceText(lblAgree, cultureViewId, "F392F568-8E18-4912-AA99-A050CBD75BA2", "I agree to the terms and conditions of the above agreements");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "SubAccountsAgreementsViewController:SetCultureConfiguration");
            }
        }

        private async void LoadDisclosures()
        {
            // See if the member is enrolled in eStatements.  If so, hide the enrollment switch
            var methods = new DocumentMethods();
            var request = new EDocumentIsEnrolledRequest { DocumentType = EDocumentTypes.AccountEStatements.ToString() };

            ShowActivityIndicator();

            var response = await methods.IsEDocumentEnrolled(request, View);

            HideActivityIndicator();

            bool isEnrolledInEStatements = false;

            if (response != null && response.Result)
            {
                isEnrolledInEStatements = true;
            }
            else
            {
                lblEnrollInEstatements.Hidden = false;
                switchEnrollInEStatements.Hidden = false;
            }

            // Display the disclosures
            var tableViewSource = new AgreementsViewSource(isEnrolledInEStatements);

            tableViewSource.ItemSelected += item =>
            {
                try
                {
                    var documentViewerViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentViewerViewController") as DocumentViewerViewController;
                    documentViewerViewController.DocumentType = DocumentViewerTypes.Url;
                    documentViewerViewController.Urls = new List<string> { item.Item2Text };
                    NavigationController.PushViewController(documentViewerViewController, true);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, "SubAccountsAgreementViewController:LoadDisclosures");
                }
            };

            mainTableView.Source = tableViewSource;
            mainTableView.ReloadData();
        }

        public string Validate()
        {
            var returnValue = string.Empty;

            if (!_agreementAccepted)
            {
                returnValue = CultureTextProvider.GetMobileResourceText(cultureViewId, "DD526FEB-D214-453D-9994-4BECC9C1156F", "You must agree to the terms and conditions before continuing.");
            }

            ((SubAccountsViewController)ParentViewController.ParentViewController).EnrollInEstatements = switchEnrollInEStatements.On;

            return returnValue;
        }
    }

    public class AgreementsViewSource : UITableViewSource
    {
        public event Action<ListViewItem> ItemSelected = delegate { };
        private readonly List<ListViewItem> _disclosureItems;
        private bool _isEnrolledInEStatements;

        public AgreementsViewSource(bool isEnrolledInEStatements)
        {
            _disclosureItems = ViewUtilities.GetSubAccountsDisclosures(isEnrolledInEStatements);
            _isEnrolledInEStatements = isEnrolledInEStatements;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            nint rows = 0;

            switch (section)
            {
                case 0: // Information
                    rows = _isEnrolledInEStatements ? 7 : 8;
                    break;
            }

            return rows;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("cellMain");

            var lblDescription = (UILabel)cell.ViewWithTag(100);

            var item = new ListViewItem();

            item = _disclosureItems[indexPath.Row];

            lblDescription.Text = item.Item1Text;

            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = new ListViewItem();

            item = _disclosureItems[indexPath.Row];

            ItemSelected(item);

            tableView.DeselectRow(indexPath, true);
        }
    }
}