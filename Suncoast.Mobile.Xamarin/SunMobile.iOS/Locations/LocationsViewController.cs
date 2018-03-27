using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using MapKit;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SunBlock.DataTransferObjects.GeoLocator;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using UIKit;
using Xamarin;
using Xamarin.Geolocation;

namespace SunMobile.iOS.Locations
{
	public partial class LocationsViewController : BaseViewController
	{
		private MyMapDelegate _mapDelegate;
		private CLLocationManager _locationManager;
		private UISearchDisplayController _searchController;
		private const double REGION = 8046.72;  // 5 Miles
		private const double MILES = 10;
		private Query _locationQuery;
		private string _searchType = "all";
		private string _locationType = "all";
		private Position _currentPosition;
		private List<LocationInfo> _locations;

		public LocationsViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			try
			{
				base.ViewDidLoad();

				_locationQuery = new Query();
				_currentPosition = new Position();

				_locationManager = new CLLocationManager();

				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{
					try
					{
						_locationManager.RequestWhenInUseAuthorization();
					}
					catch (Exception ex)
					{
						Logging.Log(ex, "LocationsViewController:RequestWhenInUseAuthorization");
					}
				}

				mapView.MapType = MKMapType.Standard;
				mapView.ShowsUserLocation = true;

				segmentSearchType.ValueChanged += (sender, e) => SearchTypeChanged();

				_mapDelegate = new MyMapDelegate(this);
				mapView.Delegate = _mapDelegate;

				_searchController = new UISearchDisplayController(searchBar, this);
				_searchController.Delegate = new SearchDelegate(mapView);
				_searchController.SearchResultsSource = new SearchSource(this, _searchController);

				GetPosition();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsViewController:ViewDidLoad");
			}
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "E24B5D09-5DAF-49C7-A009-14E148849E9F", "Locations");
			//searchView.SetQueryHint(CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "AC6ECEAE-E7CD-4E5D-895B-E816A48AAAE9", "Enter Zip Code or City, State"));
			segmentSearchType.SetTitle(CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "B95F505D-A62A-4365-BF74-F6E4D398221A", "All"), 0);
			segmentSearchType.SetTitle(CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "8A8997F9-6E5D-446E-88E7-FFA269230D00", "ATMs"), 1);
			segmentSearchType.SetTitle(CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "906E6EC6-DC5B-4B08-B97D-2BA9893A0A6C", "Branches"), 2);
		}

		protected async void GetPosition()
		{
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

				if (status != PermissionStatus.Granted)
				{
					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
					status = results[Permission.Location];
				}

				if (status == PermissionStatus.Granted)
				{
					var locator = new Geolocator { DesiredAccuracy = 50 };

					ShowActivityIndicator();

					_currentPosition = await locator.GetPositionAsync(10000);

					HideActivityIndicator();

					if (_currentPosition != null)
					{
						_locationQuery = new Query
						{
							Latitude = _currentPosition.Latitude,
							Longitude = _currentPosition.Longitude
						};

						GetLocations(_locationQuery);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsViewController:GetPosition");
			}
			finally
			{
				HideActivityIndicator();
			}
		}

		protected void GetLocations(CLLocationCoordinate2D coordinate, bool resetLocations)
		{
			try
			{
				if (resetLocations)
				{
					_locations = null;
				}

				_currentPosition.Latitude = coordinate.Latitude;
				_currentPosition.Longitude = coordinate.Longitude;
				_locationQuery.Latitude = coordinate.Latitude;
				_locationQuery.Longitude = coordinate.Longitude;

				GetLocations(_locationQuery);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsViewController:GetLocations");
			}
		}

		protected async void GetLocations(Query request)
		{
			var mapCenter = new CLLocationCoordinate2D(request.Latitude, request.Longitude);
			var mapRegion = MKCoordinateRegion.FromDistance(mapCenter, REGION, REGION);
			mapView.CenterCoordinate = mapCenter;
			mapView.Region = mapRegion;
			mapView.RemoveAnnotations(mapView.Annotations);

			if (_locations == null)
			{
				request.Type = "all";
				request.Distance = MILES;

				var methods = new LocationMethods();

				ShowActivityIndicator();

				_locations = await methods.GetLocations(request, View);

				HideActivityIndicator();
			}

			if (_locations != null)
			{
				// Add Branches
				foreach (var location in _locations)
				{
					if ((location.Type.ToLower() == _locationType || _searchType == "all") && location.Type.ToLower() == "branch")
					{
						AddAnnotation(location);
					}
				}

				// Add ATMs (don't show the ATM if it is also a branch because the pins will overlap)
				foreach (var location in _locations)
				{
					if ((location.Type.ToLower() == _locationType || _searchType == "all") && location.Type.ToLower() != "branch")
					{
						bool skip = false;

						foreach (var ann in mapView.Annotations)
						{
							if (ann.Coordinate.Latitude == location.Latitude && ann.Coordinate.Longitude == location.Longitude)
							{
								skip = true;
							}
						}

						if (!skip)
						{
							AddAnnotation(location);
						}
					}
				}
			}
		}

		private void AddAnnotation(LocationInfo location)
		{
			var annotation = new MKPointAnnotation
			{
				Title = location.LocationName + " " + location.Type,
				Subtitle = location.Address + "," + location.City + "," + location.State + "," + location.Zip
			};

			annotation.SetCoordinate(new CLLocationCoordinate2D(location.Latitude, location.Longitude));

			mapView.AddAnnotation(annotation);
		}

		private LocationInfo GetLocation(string locationName)
		{
			var locationInfo = new LocationInfo();

			foreach (var location in _locations)
			{
				if (location.LocationName == locationName)
				{
					locationInfo = location;
					break;
				}
			}

			return locationInfo;
		}

		private void SearchTypeChanged()
		{
			var selectedSegmentId = segmentSearchType.SelectedSegment;

			switch (selectedSegmentId)
			{
				case 1:
					_searchType = "atms";
					_locationType = "atm";
					break;
				case 2:
					_searchType = "branches";
					_locationType = "branch";
					break;
				default:
					_searchType = "all";
					_locationType = "all";
					break;
			}

			_locationQuery.Type = _searchType;

			GetLocations(_locationQuery);
		}

		class MyMapDelegate : MKMapViewDelegate
		{
			const string pId = "PinAnnotation";
			readonly LocationsViewController _parentView;

			public MyMapDelegate(LocationsViewController parentView)
			{
				_parentView = parentView;
			}

			public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
			{
				if (annotation is MKUserLocation)
				{
					return null;
				}

				if (annotation is MKPointAnnotation)
				{
					var pinView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(pId);

					if (pinView == null)
					{
						pinView = new MKPinAnnotationView(annotation, pId);
					}

					var pointAnnotation = (MKPointAnnotation)annotation;
					pinView.PinColor = pointAnnotation.Title.ToUpper().EndsWith("BRANCH", StringComparison.Ordinal) ? MKPinAnnotationColor.Green : MKPinAnnotationColor.Red;
					pinView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
					pinView.CanShowCallout = true;

					return pinView;
				}

				return null;
			}

			public override void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
			{
				var pinAnnotationView = view as MKPinAnnotationView;

				if (pinAnnotationView != null)
				{
					var pointAnnotation = pinAnnotationView.Annotation as MKPointAnnotation;

					if (pointAnnotation != null)
					{
						var locationName = pointAnnotation.Title.Substring(0, pointAnnotation.Title.LastIndexOf(" ", StringComparison.Ordinal));
						var locationInfo = _parentView.GetLocation(locationName);

						var myStoryboard = AppDelegate.StoryBoard;
						var locationsDetailViewController = myStoryboard.InstantiateViewController("LocationsDetailViewController") as LocationsDetailViewController;
						locationsDetailViewController.CurrentLocation = new CLLocationCoordinate2D(_parentView._currentPosition.Latitude, _parentView._currentPosition.Longitude);
						locationsDetailViewController.DestinationLocation = new CLLocationCoordinate2D(pointAnnotation.Coordinate.Latitude, pointAnnotation.Coordinate.Longitude);
						locationsDetailViewController.Location = locationInfo;

						_parentView.NavigationController.PushViewController(locationsDetailViewController, true);
					}
				}
			}
		}

		class SearchDelegate : UISearchDisplayDelegate
		{
			readonly MKMapView map;

			public SearchDelegate(MKMapView map)
			{
				this.map = map;
			}

			public override bool ShouldReloadForSearchString(UISearchDisplayController controller, string forSearchString)
			{
				var searchRequest = new MKLocalSearchRequest();
				searchRequest.NaturalLanguageQuery = forSearchString;
				searchRequest.Region = new MKCoordinateRegion(map.UserLocation.Coordinate, new MKCoordinateSpan(0.25, 0.25));

				var localSearch = new MKLocalSearch(searchRequest);
				localSearch.Start(delegate (MKLocalSearchResponse response, NSError error)
				{
					if (response != null && error == null)
					{
						((SearchSource)controller.SearchResultsSource).MapItems = response.MapItems.ToList();
						controller.SearchResultsTableView.ReloadData();
					}
					else
					{
						Console.WriteLine("local search error: {0}", error);
					}
				});

				return true;
			}
		}

		class SearchSource : UITableViewSource
		{
			private static readonly string mapItemCellId = "mapItemCellId";
			private readonly UISearchDisplayController _searchController;
			private readonly LocationsViewController _parentView;

			public List<MKMapItem> MapItems { get; set; }

			public SearchSource(LocationsViewController parentView, UISearchDisplayController searchController)
			{
				_parentView = parentView;
				_searchController = searchController;

				MapItems = new List<MKMapItem>();
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return MapItems.Count;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(mapItemCellId) ?? new UITableViewCell();

				cell.TextLabel.Text = MapItems[indexPath.Row].Name;

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				_searchController.SetActive(false, true);

				var coord = MapItems[indexPath.Row].Placemark.Location.Coordinate;
				_parentView.GetLocations(coord, true);
			}
		}
	}
}