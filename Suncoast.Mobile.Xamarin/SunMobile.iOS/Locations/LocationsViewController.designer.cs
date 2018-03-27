// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Locations
{
    [Register ("LocationsViewController")]
    partial class LocationsViewController
    {
        [Outlet]
        MapKit.MKMapView mapView { get; set; }


        [Outlet]
        UIKit.UISearchBar searchBar { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segmentSearchType { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (mapView != null) {
                mapView.Dispose ();
                mapView = null;
            }

            if (searchBar != null) {
                searchBar.Dispose ();
                searchBar = null;
            }

            if (segmentSearchType != null) {
                segmentSearchType.Dispose ();
                segmentSearchType = null;
            }
        }
    }
}