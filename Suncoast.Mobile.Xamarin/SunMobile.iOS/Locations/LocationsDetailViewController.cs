using System;
using CoreLocation;
using Foundation;
using MapKit;
using UIKit;
using SunBlock.DataTransferObjects.GeoLocator;

namespace SunMobile.iOS.Locations
{
	public partial class LocationsDetailViewController : UITableViewController
	{
		public CLLocationCoordinate2D CurrentLocation { get; set; }
		public CLLocationCoordinate2D DestinationLocation { get; set; }
		public LocationInfo Location { get; set; }

		private string _phoneNumber;

		public LocationsDetailViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Details";

			btnGetDirections.TouchUpInside += (sender, e) => 
			GetDirections();

			btnCall.TouchUpInside += (sender, e) => 
			Call();

			lblLocationName.Text = Location.LocationName;
			lblAddress1.Text = Location.Address;
			lblAddress2.Text = Location.City + ", " + Location.StateAbbr + " " + Location.Zip;

			// See if it is a phone.
			if (Location.Details.ContainsKey("Phone"))
			{
				_phoneNumber = Location.Details["Phone"];

				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				{
					btnCall.SetTitle("Call " + _phoneNumber, UIControlState.Normal);
				}
				else
				{
					btnCall.SetTitle(_phoneNumber, UIControlState.Normal);
				}
			} 
			else
			{
				btnCall.SetTitle(string.Empty, UIControlState.Normal);
				btnCall.Enabled = false;
			}

			// Hides the remaining rows.
			tableView.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);	
		}

		private void GetDirections()
		{
			var destPlaceMark = new MKPlacemark(DestinationLocation, new MKPlacemarkAddress());
			var destItem = new MKMapItem(destPlaceMark) 
			{
				Name = Location.LocationName
			};

			if (!string.IsNullOrEmpty(_phoneNumber))
			{
				destItem.PhoneNumber = _phoneNumber;
			}

			var launchOptions = new MKLaunchOptions 
			{
				DirectionsMode = MKDirectionsMode.Driving,
				MapCenter = CurrentLocation
			};

			destItem.OpenInMaps(launchOptions);
		}

		private void Call()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				var url = new NSUrl(string.Format(@"telprompt://{0}", _phoneNumber));
				UIApplication.SharedApplication.OpenUrl(url);
			}
		}
	}
}