using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Cards
{
	public class CardsMenuFragment : BaseFragment
	{
		private TableRow tableRowTravelNotifications;
        private TableRow tableRowsOrderRaysCard;
        private TextView lblTravelNotifications;
        private TextView lblTampaBayRaysCard;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.CardsMenuView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			((MainActivity)Activity).SetActionBarTitle("Cards");

            lblTravelNotifications = Activity.FindViewById<TextView>(Resource.Id.lblTravelNotifications);
            lblTampaBayRaysCard = Activity.FindViewById<TextView>(Resource.Id.lblTampaBayRaysCard);
			tableRowTravelNotifications = Activity.FindViewById<TableRow>(Resource.Id.rowTravelNotifications);
			tableRowTravelNotifications.Click += (sender, e) => ListItemClicked(0);
            tableRowsOrderRaysCard = Activity.FindViewById<TableRow>(Resource.Id.rowOrderRaysCard);
			tableRowsOrderRaysCard.Click += (sender, e) => ListItemClicked(1);

            SetCultureInformation();
		}

        private void SetCultureInformation()
        {
            ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "1e9837a3-f890-4b1e-94f0-2699e849674b", "Cards"));

            lblTravelNotifications.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "2e4602e6-9afa-43ac-8f44-e255236cc1e4", "Travel Notifications");
            lblTampaBayRaysCard.Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "361ccc96-5ceb-11e7-907b-a6006ad3dba0", "Tampa Bay Rays Card");
        }

		public void ListItemClicked(int position)
		{
			Android.Support.V4.App.Fragment fragment = null;

			switch (position)
			{
				case 0:
					fragment = new TravelNotificationsFragment();
					break;
				case 1:
                    fragment = new OrderRaysCardFragment();
					break;
			}

			if (fragment != null)
			{
				NavigationService.NavigatePush(fragment, true, false);
			}
		}
	}
}