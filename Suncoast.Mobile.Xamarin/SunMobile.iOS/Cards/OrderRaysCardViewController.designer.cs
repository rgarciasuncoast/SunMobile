// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Cards
{
    [Register ("OrderRaysCardViewController")]
    partial class OrderRaysCardViewController
    {
        [Outlet]
        UIKit.UILabel lblCard { get; set; }


        [Outlet]
        UIKit.UILabel lblNamesLabel { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectCardHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectCardImage { get; set; }


        [Outlet]
        UIKit.UILabel lblSelectImageHeader { get; set; }


        [Outlet]
        UIKit.UITextField txtNames { get; set; }


        [Outlet]
        UIKit.UIView viewNames { get; set; }


        [Outlet]
        UIKit.UIView viewPlaceholder { get; set; }


        [Outlet]
        UIKit.UIView viewSelectCard { get; set; }


        [Outlet]
        UIKit.UIView viewSeparatorBottom { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblCard != null) {
                lblCard.Dispose ();
                lblCard = null;
            }

            if (lblNamesLabel != null) {
                lblNamesLabel.Dispose ();
                lblNamesLabel = null;
            }

            if (lblSelectCardHeader != null) {
                lblSelectCardHeader.Dispose ();
                lblSelectCardHeader = null;
            }

            if (lblSelectCardImage != null) {
                lblSelectCardImage.Dispose ();
                lblSelectCardImage = null;
            }

            if (lblSelectImageHeader != null) {
                lblSelectImageHeader.Dispose ();
                lblSelectImageHeader = null;
            }

            if (txtNames != null) {
                txtNames.Dispose ();
                txtNames = null;
            }

            if (viewNames != null) {
                viewNames.Dispose ();
                viewNames = null;
            }

            if (viewPlaceholder != null) {
                viewPlaceholder.Dispose ();
                viewPlaceholder = null;
            }

            if (viewSelectCard != null) {
                viewSelectCard.Dispose ();
                viewSelectCard = null;
            }

            if (viewSeparatorBottom != null) {
                viewSeparatorBottom.Dispose ();
                viewSeparatorBottom = null;
            }
        }
    }
}