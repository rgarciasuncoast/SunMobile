using System;
using Foundation;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class DocumentsMenuTableViewController : BaseTableViewController
	{
		public DocumentsMenuTableViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Hides the remaining rows.
			tableMenuView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);

			CommonMethods.AddBottomToolbar(this);
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("441F87A2-7C3B-4296-A017-999BDB2BE512", "4455C2A9-0A4E-456C-A852-A023032B3C52", "Documents");
			CultureTextProvider.SetMobileResourceText(lblDocumentMenuDocumentCenter, "441F87A2-7C3B-4296-A017-999BDB2BE512", "EFC795FA-785C-4C01-9A1D-EC4A6627754F", "Document Center");
			CultureTextProvider.SetMobileResourceText(lblDocumentMenuAccountEStatements, "441F87A2-7C3B-4296-A017-999BDB2BE512", "4258CDF2-5D12-47A8-B912-8B5A8E1B310A", "Account eStatements");
			CultureTextProvider.SetMobileResourceText(lblDocumentMenuTaxDocuments, "441F87A2-7C3B-4296-A017-999BDB2BE512", "46651E04-01C9-4B98-9333-21EFC980813B", "Tax Documents");
			CultureTextProvider.SetMobileResourceText(lblDocumentMenuENotices, "441F87A2-7C3B-4296-A017-999BDB2BE512", "EABA0759-FF1A-4D9F-96F5-1AC83303C541", "eNotices");		

		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			try
			{
				switch (indexPath.Row)
				{
					case 0:
						var documentCenterViewController = AppDelegate.StoryBoard.InstantiateViewController("DocumentCenterViewController") as DocumentCenterViewController;
						NavigationController.PushViewController(documentCenterViewController, true);
						break;
					case 1:
						var eStatementsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("EStatementsTableViewController") as EStatementsTableViewController;
						eStatementsTableViewController.Header = "Account eStatements";
						eStatementsTableViewController.DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.AccountEStatements;
						NavigationController.PushViewController(eStatementsTableViewController, true);
						break;					
                    case 2:
                        var creditCardStatementsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("EStatementsTableViewController") as EStatementsTableViewController;
                        creditCardStatementsTableViewController.Header = "Credit Card eStatements";
                        creditCardStatementsTableViewController.DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.CreditCardAnnualEStatements;
                        NavigationController.PushViewController(creditCardStatementsTableViewController, true);
                        break;
					case 3:
						var eNoticesTableViewController = AppDelegate.StoryBoard.InstantiateViewController("EStatementsTableViewController") as EStatementsTableViewController;
						eNoticesTableViewController.Header = "eNotices";
						eNoticesTableViewController.DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.ENotices;
						NavigationController.PushViewController(eNoticesTableViewController, true);
						break;                    
                    case 4:
                        var taxDocumentsTableViewController = AppDelegate.StoryBoard.InstantiateViewController("EStatementsTableViewController") as EStatementsTableViewController;
                        taxDocumentsTableViewController.Header = "Tax Documents";
                        taxDocumentsTableViewController.DocumentType = SunBlock.DataTransferObjects.OnBase.EDocumentTypes.TaxDocuments;
                        NavigationController.PushViewController(taxDocumentsTableViewController, true);
                        break;				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DocumentsMenuTableViewController:RowSelected");
			}
		}
	}
}