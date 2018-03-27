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
    [Register ("DocumentUploadViewController")]
    partial class DocumentUploadViewController
    {
        [Outlet]
        UIKit.UIBarButtonItem btnUploadiCloud { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnUploadPhoto { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnUploadiCloud != null) {
                btnUploadiCloud.Dispose ();
                btnUploadiCloud = null;
            }

            if (btnUploadPhoto != null) {
                btnUploadPhoto.Dispose ();
                btnUploadPhoto = null;
            }

            if (tableViewMain != null) {
                tableViewMain.Dispose ();
                tableViewMain = null;
            }
        }
    }
}