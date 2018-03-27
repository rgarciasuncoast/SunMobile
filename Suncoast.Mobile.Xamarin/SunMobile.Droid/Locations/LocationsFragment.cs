using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SunBlock.DataTransferObjects.GeoLocator;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.General;
using Xamarin.Geolocation;

namespace SunMobile.Droid.Locations
{
	public class LocationsFragment : BaseMapFragment, IOnMapReadyCallback
	{
		private SearchView searchView;
		private RadioButton btnAll;
		private RadioButton btnATMs;
		private RadioButton btnBranches;
		private MapFragment mapFragment;
		private GoogleMap _googleMap;
		private Query _locationQuery;
		private Position _currentPosition;
		private List<LocationInfo> _locations;
		private string _searchType = "all";
		private string _locationType = "all";
		private const double MILES = 10;
		private const int ZOOM_LEVEL = 12;
		private CameraPosition camPos;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.LocationsView, null);
			RetainInstance = true;

			mapFragment = new MapFragment();

			var fragment = Activity.FragmentManager.BeginTransaction();
			fragment.Add(Resource.Id.mapLayout, mapFragment);
			fragment.Commit();

			GetMap();

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			try
			{
				var json = JsonConvert.SerializeObject(_googleMap.CameraPosition.Target.Latitude);
				outState.PutString("TargetLat", json);
				json = JsonConvert.SerializeObject(_googleMap.CameraPosition.Target.Longitude);
				outState.PutString("TargetLng", json);
				json = JsonConvert.SerializeObject(_googleMap.CameraPosition.Zoom);
				outState.PutString("Zoom", json);
				json = JsonConvert.SerializeObject(_googleMap.CameraPosition.Tilt);
				outState.PutString("Tilt", json);
				json = JsonConvert.SerializeObject(_googleMap.CameraPosition.Bearing);
				outState.PutString("Bearing", json);

				base.OnSaveInstanceState(outState);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:OnSaveInstanceState");
			}
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "E24B5D09-5DAF-49C7-A009-14E148849E9F", "Locations"));
                searchView.SetQueryHint(CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "AC6ECEAE-E7CD-4E5D-895B-E816A48AAAE9", "Enter Zip Code or City, State"));
                CultureTextProvider.SetMobileResourceText(btnAll, "137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "B95F505D-A62A-4365-BF74-F6E4D398221A", "All");
                CultureTextProvider.SetMobileResourceText(btnATMs, "137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "8A8997F9-6E5D-446E-88E7-FFA269230D00", "ATMs");
                CultureTextProvider.SetMobileResourceText(btnBranches, "137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "906E6EC6-DC5B-4B08-B97D-2BA9893A0A6C", "Branches");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:SetCultureConfiguration");
			}
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Locations");

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("TargetLat");
				var targetLat = JsonConvert.DeserializeObject<double>(json);
				json = savedInstanceState.GetString("TargetLng");
				var targetLng = JsonConvert.DeserializeObject<double>(json);
				json = savedInstanceState.GetString("Zoom");
				var zoom = JsonConvert.DeserializeObject<float>(json);
				json = savedInstanceState.GetString("Tilt");
				var tilt = JsonConvert.DeserializeObject<float>(json);
				json = savedInstanceState.GetString("Bearing");
				var bearing = JsonConvert.DeserializeObject<float>(json);

				var targetLatLng = new LatLng (targetLat, targetLng);
				camPos = new CameraPosition(targetLatLng, zoom, tilt, bearing);
			}

			searchView = Activity.FindViewById<SearchView>(Resource.Id.searchBar);
			searchView.QueryTextSubmit += (sender, e) => SearchMap(e.Query);
			btnAll = Activity.FindViewById<RadioButton>(Resource.Id.btnAll);
			btnAll.CheckedChange += SearchTypeChanged;
			btnATMs = Activity.FindViewById<RadioButton>(Resource.Id.btnATMs);
			btnATMs.CheckedChange += SearchTypeChanged;
			btnBranches = Activity.FindViewById<RadioButton>(Resource.Id.btnBranches);
			btnBranches.CheckedChange += SearchTypeChanged;

			if (_currentPosition == null)
			{
				_currentPosition = new Position();
			}			
		}

		private async void GetMap()
		{
			if (await CheckLocationPermission())
			{
				mapFragment.GetMapAsync(this);
			}
		}

		public async Task<bool> CheckLocationPermission()
		{
			bool returnValue = false;

			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
					{
						var alertText = CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "53252D43-AAD0-4D3B-BB0B-DCD74BF73E97", "SunMobile requires access to your location.");
						var alertOk = CultureTextProvider.GetMobileResourceText("137FB650-033C-48BB-ADA6-B63CC5CC6D4F", "372519AB-CC70-4FA4-A498-897A788AD9E0", "OK");
						await AlertMethods.Alert(Activity, "SunMobile", alertText, alertOk);
					}

					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
					status = results[Permission.Location];
				}

				returnValue = (status == PermissionStatus.Granted);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:CheckLocationPermission");
			}

			return returnValue;
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			_googleMap = googleMap;

			if (_locations == null)
			{
				GetPosition();
			}
			else
			{
				RefreshMapState();
			}
		}

		private void RefreshMapState()
		{
			try 
			{
				InitializeMap(_currentPosition.Latitude, _currentPosition.Longitude);
				var query = new Query();
				query.Latitude = _currentPosition.Latitude;
				query.Longitude = _currentPosition.Longitude;
				GetLocations(query);
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:RefreshMapState");
			}
		}

		private void SearchMap(string search)
		{
			try 
			{
				searchView.ClearFocus();
				GeneralUtilities.CloseKeyboard(Activity);

				var geocoder = new Geocoder(Activity);
				var addresses = geocoder.GetFromLocationName(search, 1);

				if (addresses.Count > 0)
				{
					_locationQuery = new Query();
					_locationQuery.Latitude = addresses[0].Latitude;
					_locationQuery.Longitude = addresses[0].Longitude;
					_locations = null;
					_currentPosition.Latitude = _locationQuery.Latitude;
					_currentPosition.Longitude = _locationQuery.Longitude;

					InitializeMap(_locationQuery.Latitude, _locationQuery.Longitude);

					GetLocations(_locationQuery);
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:SearchMap");
			}
		}

		private void SearchTypeChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (e.IsChecked)
			{
				if (sender == btnATMs)
				{
					_searchType = "atms";
					_locationType = "atm";
				}
				else if (sender == btnBranches)
				{
					_searchType = "branches";
					_locationType = "branch";
				}
				else
				{
					_searchType = "all";
					_locationType = "all";
				}

				GetLocations(_locationQuery);
			}
		}

		protected async void GetPosition()
		{			
			try 
			{
				if (await CheckLocationPermission())
				{
					var locator = new Geolocator(Activity) { DesiredAccuracy = 50 };
					_currentPosition = await locator.GetPositionAsync(10000);

					if (_currentPosition != null)
					{
						InitializeMap(_currentPosition.Latitude, _currentPosition.Longitude);

						_locationQuery = new Query { Latitude = _currentPosition.Latitude, Longitude = _currentPosition.Longitude };

						GetLocations(_locationQuery);
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:GetPosition");
			}
		}

		private void InitializeMap(double latitude, double longitude)
		{
			try 
			{
				var mapCenter = new LatLng(latitude, longitude);
				var builder = CameraPosition.InvokeBuilder();
				builder.Target(mapCenter);
				builder.Zoom(ZOOM_LEVEL);
				builder.Bearing(0);
				builder.Tilt(0);

				_googleMap.MyLocationEnabled = true;
				_googleMap.UiSettings.CompassEnabled = true;
				_googleMap.UiSettings.MyLocationButtonEnabled = true;
				_googleMap.UiSettings.ZoomControlsEnabled = true;

				var cameraPosition = builder.Build();
				var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

				if(camPos != null)
				{
					cameraUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
				}

				_googleMap.InfoWindowClick += OnInfoWindowClicked;

				if (_googleMap != null)
				{
					_googleMap.MoveCamera(cameraUpdate);
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:InitializeMap");
			}
		}

		protected async void GetLocations(Query request)
		{
			try 
			{
				_googleMap.Clear();

				if (_locations == null && request != null)
				{
					var methods = new LocationMethods();
					request.Type = "all";
					request.Distance = MILES;

					ShowActivityIndicator();

					_locations = await methods.GetLocations(request, Activity);

					HideActivityIndicator();
				}

				if (_locations != null)
				{
					// Add ATMs
					foreach (var location in _locations)
					{
						if ((location.Type.ToLower() == _locationType || _searchType == "all") && location.Type.ToLower() != "branch")
						{
							AddAnnotation(location);
						}
					}

					// Add branches second so the markers will appear on top of the Z order.
					foreach (var location in _locations)
					{
						if ((location.Type.ToLower() == _locationType || _searchType == "all") && location.Type.ToLower() == "branch")
						{						
							AddAnnotation(location);					
						}
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:GetLocations");
			}
		}

		private void AddAnnotation(LocationInfo location)
		{
			var annotation = new MarkerOptions();
			annotation.SetPosition(new LatLng(location.Latitude, location.Longitude));
			annotation.SetTitle(location.LocationName + " " + location.Type);
			annotation.SetSnippet(location.Address + "," + location.City + "," + location.State + "," + location.Zip);

			if (location.Type.ToLower().EndsWith("branch", StringComparison.Ordinal))
			{						
				annotation.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
			}
			else
			{
				annotation.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
			}

			_googleMap.AddMarker(annotation);
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

		private void OnInfoWindowClicked(object sender, GoogleMap.InfoWindowClickEventArgs e)
		{
			var marker = e.Marker;

			var locationsDetailViewFragment = new LocationsDetailViewFragment
			{
				Arguments = new Bundle()
			};

			var locationName = marker.Title.Substring(0, marker.Title.LastIndexOf(" ", StringComparison.Ordinal));
			var locationInfo = GetLocation(locationName);
			locationsDetailViewFragment.Location = locationInfo;

			NavigationService.NavigatePush(locationsDetailViewFragment, true, false);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}