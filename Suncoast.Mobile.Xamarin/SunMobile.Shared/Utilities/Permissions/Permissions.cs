#if __ANDROID__
using Plugin.Permissions;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using Android.App;
using SunMobile.Shared.Methods;

namespace SunMobile.Shared.Permissions
{
	public static class Permissions
	{
		public static async Task<bool> GetCameraPermission(Activity activity)
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

			if (status != PermissionStatus.Granted)
			{
				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
				{
					await AlertMethods.Alert(activity, "SunMobile", "SunMobile would like to access your camera.", "OK");
				}

				var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });

				if (results != null)
				{
					status = results[Permission.Camera];
				}
			}

			return (status == PermissionStatus.Granted);
		}

		public static async Task<bool> GetMapsPermission(Activity activity)
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

			if (status != PermissionStatus.Granted)
			{
				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
				{
					await AlertMethods.Alert(activity, "SunMobile", "SunMobile would like to access your phone.", "OK");
				}

				var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });

				if (results != null)
				{
					status = results[Permission.Location];
				}
			}

			return (status == PermissionStatus.Granted);
		}

		// Phone State
		public static async Task<bool> GetReadPhonePermission(Activity activity)
		{
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);

			if (status != PermissionStatus.Granted)
			{
				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Phone))
				{
					await AlertMethods.Alert(activity, "SunMobile", "SunMobile would like to access your phone.", "OK");
				}

				var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Phone });

				if (results != null)
				{
					status = results[Permission.Phone];
				}
			}

			return (status == PermissionStatus.Granted);
		}
	}
}

#endif