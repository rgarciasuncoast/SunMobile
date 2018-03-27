// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Accounts
{
    [Register ("UploadDisputeDocumentsTableViewController")]
    partial class UploadDisputeDocumentsTableViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnUploadiCloud { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnUploadPhotos { get; set; }


        [Outlet]
        UIKit.UITableView tblViewUploadedDocuments { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnUploadPhotos != null) {
                btnUploadPhotos.Dispose ();
                btnUploadPhotos = null;
            }

            if (tblViewUploadedDocuments != null) {
                tblViewUploadedDocuments.Dispose ();
                tblViewUploadedDocuments = null;
            }
        }
    }
}