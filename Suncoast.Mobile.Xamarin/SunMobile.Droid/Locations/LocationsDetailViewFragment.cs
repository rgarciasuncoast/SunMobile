using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.GeoLocator;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.General;
using Uri = Android.Net.Uri;

namespace SunMobile.Droid
{
	public class LocationsDetailViewFragment : BaseFragment
	{
		private TextView _locationName;
		private TextView _locationAddress;
		private Button _btnGetDirections;
		private Button _btnLocationPhone;
		private TextView _locationPhone;
		private View _lastSeparator;

		public LocationInfo Location { get; set; }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.LocationsDetailView, null);

			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();
		}

		public void GetDirections(string address)
		{
			var intent = new Intent(Intent.ActionView, Uri.Parse("http://maps.google.com/maps?&daddr=" + address));
			StartActivity(intent);
		}

		public override void SetupView()
		{
			try
			{
				base.SetupView();

				((MainActivity)Activity).SetActionBarTitle("Location Details");

				_locationName = Activity.FindViewById<TextView>(Resource.Id.txtLocationName);
				_locationAddress = Activity.FindViewById<TextView>(Resource.Id.txtAddress);
				_btnGetDirections = Activity.FindViewById<Button>(Resource.Id.btnGetDirections);
				_btnGetDirections.Click += (s, e) => GetDirections(Location.Address + "," + Location.City + "," + Location.StateAbbr);
				_locationPhone = Activity.FindViewById<TextView>(Resource.Id.txtPhoneNumber);
				_btnLocationPhone = Activity.FindViewById<Button>(Resource.Id.btnPhoneNumber);
				_btnLocationPhone.Click += (s, e) => CallNumber(Location.Details["Phone"]);
				_lastSeparator = Activity.FindViewById<View>(Resource.Id.lastSeparator);

				_locationName.Text = Location.LocationName;
				_locationAddress.Text = Location.Address + "\n" + Location.City + ", " + Location.StateAbbr + " " + Location.Zip;

				var phoneNumber = string.Empty;

				if (Location?.Details != null && Location.Details.ContainsKey("Phone"))
				{
					phoneNumber = Location.Details["Phone"];

					if (GeneralUtilities.CanMakeCalls(Activity))
					{
						SetCallButton(true, "Call " + phoneNumber);
					}
					else
					{
						SetCallButton(false, phoneNumber);
					}
				}
				else
				{
					SetCallButton(false, phoneNumber);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsFragment:SetupView");
			}
		}

		private void SetCallButton(bool canMakeCalls, string text)
		{
			_btnLocationPhone.Visibility = canMakeCalls && !string.IsNullOrEmpty(text) ? ViewStates.Visible : ViewStates.Gone;
			_lastSeparator.Visibility = string.IsNullOrEmpty(text) ? ViewStates.Gone : ViewStates.Visible;
			_locationPhone.Visibility = canMakeCalls ? ViewStates.Gone : ViewStates.Visible;

			_btnLocationPhone.Text = text;
			_locationPhone.Text = text;
		}

		public void CallNumber(string phoneNumber)
		{
			try
			{
				var callIntent = new Intent(Intent.ActionDial);
				callIntent.SetData(Uri.Parse("tel:" + phoneNumber));
				StartActivity(callIntent);

			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LocationsDetailViewFragment:CallNumber");
			}
		}
	}
}