using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunMobile.Droid.Authentication;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Profile
{
	public class PasswordReminderFragment : BaseFragment
	{
		private TableRow rowUpdatePassword;
		private TableRow rowRemindMeLater;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.PasswordReminderView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			((MainActivity)Activity).SetActionBarTitle("Password Reminder");

			rowUpdatePassword = Activity.FindViewById<TableRow>(Resource.Id.rowUpdatePassword);
			rowUpdatePassword.Click += (sender, e) =>
			{
				UpdatePassword();
			};

			rowRemindMeLater = Activity.FindViewById<TableRow>(Resource.Id.rowRemindMeLater);
			rowRemindMeLater.Click += (sender, e) =>
			{
				RemindMeLater();
			};
		}

		private void UpdatePassword()
		{
			try
			{
				var updatePasswordFragment = new UpdatePasswordFragment();

				updatePasswordFragment.Updated += (obj) =>
				{
					NavigationService.NavigatePop(false);
				};

				NavigationService.NavigatePush(updatePasswordFragment, true, false);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "PasswordReminderFragment:UpdatePassword");
			}
		}

		private async void RemindMeLater()
		{
			try
			{
				var request = new DelayPasswordNotificationRequest();

				ShowActivityIndicator();
				var methods = new AuthenticationMethods();
				await methods.DelayPasswordNotification(request, Activity);
				HideActivityIndicator();

				SessionSettings.Instance.ShowPasswordReminder = false;
				((MainActivity)Activity).ShowInfoActionButton(false);
				NavigationService.NavigatePop(false);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "PasswordReminderFragment:RemindMeLater");
			}
		}
	}
}