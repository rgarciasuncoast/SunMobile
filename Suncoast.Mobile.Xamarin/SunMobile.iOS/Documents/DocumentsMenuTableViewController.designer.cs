// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Documents
{
    [Register ("DocumentsMenuTableViewController")]
    partial class DocumentsMenuTableViewController
    {
        [Outlet]
        UIKit.UILabel lblDocumentMenuAccountEStatements { get; set; }


        [Outlet]
        UIKit.UILabel lblDocumentMenuCreditCardStatements { get; set; }


        [Outlet]
        UIKit.UILabel lblDocumentMenuDocumentCenter { get; set; }


        [Outlet]
        UIKit.UILabel lblDocumentMenuENotices { get; set; }


        [Outlet]
        UIKit.UILabel lblDocumentMenuTaxDocuments { get; set; }


        [Outlet]
        UIKit.UITableView tableMenuView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblDocumentMenuAccountEStatements != null) {
                lblDocumentMenuAccountEStatements.Dispose ();
                lblDocumentMenuAccountEStatements = null;
            }

            if (lblDocumentMenuCreditCardStatements != null) {
                lblDocumentMenuCreditCardStatements.Dispose ();
                lblDocumentMenuCreditCardStatements = null;
            }

            if (lblDocumentMenuDocumentCenter != null) {
                lblDocumentMenuDocumentCenter.Dispose ();
                lblDocumentMenuDocumentCenter = null;
            }

            if (lblDocumentMenuENotices != null) {
                lblDocumentMenuENotices.Dispose ();
                lblDocumentMenuENotices = null;
            }

            if (lblDocumentMenuTaxDocuments != null) {
                lblDocumentMenuTaxDocuments.Dispose ();
                lblDocumentMenuTaxDocuments = null;
            }

            if (tableMenuView != null) {
                tableMenuView.Dispose ();
                tableMenuView = null;
            }
        }
    }
}