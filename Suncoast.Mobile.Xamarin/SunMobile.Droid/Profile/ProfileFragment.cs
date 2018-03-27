using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Profile
{
    public class ProfileFragment : BaseFragment
	{
        private TableRow tableRowManageContactInfo;
		private TableRow tableRowUpdatePassword;
		private TableRow tableRowEDocumentsOptions;
		private TableRow tableRowMessageAlertSettings;

        private TextView lblProfileContactInfo;
        private TextView lblProfileElectronicDocumentEnrollment;
        private TextView lblProfileManageAlertSettings;
        private TextView lblProfileUpdatePassword;

        private static readonly string cultureViewId = "9EA5D29A-8CD2-40E4-861B-290FB3C4A864";

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.ProfileView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			//Challenge();

			SetupView();
		}

		public override void SetCultureConfiguration()
		{
			try
			{
				var viewText = CultureTextProvider.GetMobileResourceText(cultureViewId, "92FE93E5-E3E1-43E5-9390-483DB48654E6");

				if (!string.IsNullOrEmpty(viewText))
				{
					((MainActivity)Activity).SetActionBarTitle(viewText);
				}

				CultureTextProvider.SetMobileResourceText(lblProfileContactInfo, cultureViewId, "06B488C2-82FB-4029-BE4E-FF4D2AD397A7");
				CultureTextProvider.SetMobileResourceText(lblProfileElectronicDocumentEnrollment, cultureViewId, "ADE900A8-F1A4-4D00-8BD3-134D91BB5BF4");
				CultureTextProvider.SetMobileResourceText(lblProfileManageAlertSettings, cultureViewId, "15B4185A-2541-462B-8250-06F58164A0DC");
				CultureTextProvider.SetMobileResourceText(lblProfileUpdatePassword, cultureViewId, "1F5DC964-15B6-4A56-93EF-73EF21BDEE3C");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ContactInfoFragment:SetCultureConfiguration");
			}
		}

		public override void SetupView()
		{
			base.SetupView();

            tableRowManageContactInfo = Activity.FindViewById<TableRow>(Resource.Id.rowManageContactInfo);
			tableRowManageContactInfo.Click += (sender, e) => ListItemClicked(0);
			tableRowUpdatePassword = Activity.FindViewById<TableRow>(Resource.Id.rowUpdatePassword);
			tableRowUpdatePassword.Click += (sender, e) => ListItemClicked(1);
			tableRowEDocumentsOptions = Activity.FindViewById<TableRow>(Resource.Id.rowEDocumentsOptions);
			tableRowEDocumentsOptions.Click += (sender, e) => ListItemClicked(2);
			tableRowMessageAlertSettings = Activity.FindViewById<TableRow>(Resource.Id.rowMessageAlertSettings);
			tableRowMessageAlertSettings.Click += (sender, e) => ListItemClicked(3);

            lblProfileContactInfo = Activity.FindViewById<TextView>(Resource.Id.lblProfileManageContactInfo);
            lblProfileElectronicDocumentEnrollment = Activity.FindViewById<TextView>(Resource.Id.lblProfileElectronicDocumentEnrollment);
            lblProfileManageAlertSettings = Activity.FindViewById<TextView>(Resource.Id.lblProfileManageAlertSettings);
            lblProfileUpdatePassword = Activity.FindViewById<TextView>(Resource.Id.lblProfileUpdatePassword);
		}

		/*
		private void Challenge()
		{
			// Validate the account
			// If the account is not validated, disable the screen.
			if (!SessionSettings.Instance.IsAccountValidated)
			{
				GeneralUtilities.DisableView((ViewGroup)View, true);
			}
		}
		*/

		public void ListItemClicked(int position)
		{
			Android.Support.V4.App.Fragment fragment = null;

			try
			{
				switch (position)
				{
                    case 0:
                        fragment = new ContactInfoFragment();
                        ((ContactInfoFragment)fragment).EditMode = true;
                        break;
					case 1:
						fragment = new UpdatePasswordFragment();
						break;
					case 2:
						fragment = new DocumentOptionsFragment();
					    break;
					case 3:
						fragment = new ManageAlertsFragment();
					    break;
				}

				if (fragment != null)
				{
					NavigationService.NavigatePush(fragment, true, false);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "ProfileTableViewController:RowSelected");
			}
		}
	}
}