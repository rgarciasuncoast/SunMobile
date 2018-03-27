using System;
using Android.App;
using Android.Content;
using Gcm.Client;
using SunMobile.Shared.Utilities.Settings;

[assembly: UsesPermission (Android.Manifest.Permission.ReceiveBootCompleted)]
[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace SunMobile.Droid.Messaging
{
	[BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new[] { Intent.ActionBootCompleted })] // Allow GCM on boot and when app is closed
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]

	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<GoogleCloudMessagingService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project Number
		public static string[] SENDER_IDS = { AppSettings.GoogleApiProjectId };
	}

	[Service] //Must use the service tag
	public class GoogleCloudMessagingService : GcmServiceBase
	{
		public static string RegistrationID { get; private set; }

		public GoogleCloudMessagingService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

		protected override void OnRegistered(Context context, string registrationId)
		{
			SessionSettings.Instance.DeviceToken = registrationId;
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			//Receive notice that the app no longer wants notifications
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			if (intent.Extras.ContainsKey("message")) 
			{
				var message = intent.Extras.Get("message").ToString();

				CreateLocalNotification(context, "SunMobile", message);
			}
		}

		protected override bool OnRecoverableError(Context context, string errorId)
		{
			return true;
		}

		protected override void OnError(Context context, string errorId)
		{
			//Some more serious error happened
		}

		private void CreateLocalNotification(Context context, string title, string message)
		{
			var notificationManager = GetSystemService(NotificationService) as NotificationManager;			
			PendingIntent contentIntent = PendingIntent.GetActivity(context, 0, new Intent(this, typeof(MainActivity)), 0);

			var versionString = Android.OS.Build.VERSION.Sdk;
			int version;
			int.TryParse(versionString, out version);

			var builder = new Notification.Builder(context);
			builder.SetAutoCancel(true);
			builder.SetContentTitle(title);
			builder.SetContentText(message);

			if (version <= 20)
			{
				builder.SetSmallIcon(Resource.Drawable.icon_notification);
			}
			else
			{
				builder.SetSmallIcon(Resource.Drawable.icon_notification_silhouette);
			}

			builder.SetContentIntent(contentIntent);
			var notification = builder.Build();

			var random = new Random();
			int randomNumber = random.Next(0, 100000);

			notificationManager.Notify(randomNumber, notification);
		}
	}
}