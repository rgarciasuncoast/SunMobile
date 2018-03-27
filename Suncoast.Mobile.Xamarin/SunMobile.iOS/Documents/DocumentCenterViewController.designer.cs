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
    [Register ("DocumentCenterViewController")]
    partial class DocumentCenterViewController
    {
        [Outlet]
        UIKit.UISegmentedControl segmentDocumentType { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMain { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (segmentDocumentType != null) {
                segmentDocumentType.Dispose ();
                segmentDocumentType = null;
            }

            if (tableViewMain != null) {
                tableViewMain.Dispose ();
                tableViewMain = null;
            }
        }
    }
}