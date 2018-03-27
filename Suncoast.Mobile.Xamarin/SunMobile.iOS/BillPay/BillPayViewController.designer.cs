// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.BillPay
{
    [Register ("BillPayViewController")]
    partial class BillPayViewController
    {
        [Outlet]
        UIKit.UIButton btnNext { get; set; }


        [Outlet]
        UIKit.UIButton btnPrevious { get; set; }


        [Outlet]
        UIKit.UILabel lblDate { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segmentListType { get; set; }


        [Outlet]
        UIKit.UITableView tableViewPayments { get; set; }


        [Outlet]
        UIKit.UIToolbar toolBarBottom { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (segmentListType != null) {
                segmentListType.Dispose ();
                segmentListType = null;
            }

            if (tableViewPayments != null) {
                tableViewPayments.Dispose ();
                tableViewPayments = null;
            }

            if (toolBarBottom != null) {
                toolBarBottom.Dispose ();
                toolBarBottom = null;
            }
        }
    }
}