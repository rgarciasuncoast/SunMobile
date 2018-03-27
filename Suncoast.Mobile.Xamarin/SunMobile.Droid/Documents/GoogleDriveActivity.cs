using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.Gms.Drive.Query;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using SunMobile.Droid.Common;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "GoogleDriveActivity", Theme = "@style/CustomHoloLightTheme")]
	public class GoogleDriveActivity : BaseListActivity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		GoogleApiClient googleApiClient;
		Button buttonSearch;
		EditText editTextFilename;
		ListView listViewResults;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.GoogleDriveView);

			buttonSearch = FindViewById<Button>(Resource.Id.buttonSearch);
			editTextFilename = FindViewById<EditText>(Resource.Id.editTextFilename);

			buttonSearch.Click += async delegate
			{

				buttonSearch.Enabled = false;

				// Build a query to search for files of
				// This will only return files that you created/opened through this app
				var query = new QueryClass.Builder()
					.AddFilter(Filters.Contains(SearchableField.Title, editTextFilename.Text))
					.Build();

				// Execute search asynchronously
				var results = await DriveClass.DriveApi.Query(googleApiClient, query).AsAsync<IDriveApiMetadataBufferResult>();

				// Check for a successful result
				if (results.Status.IsSuccess)
				{

					var files = new List<Metadata>();

					// Loop through the results
					for (var i = 0; i < results.MetadataBuffer.Count; i++)
						files.Add(results.MetadataBuffer.Get(i).JavaCast<Metadata>());

					listViewResults.Adapter = new ArrayAdapter<string>(
						this,
						Android.Resource.Layout.SimpleListItem1,
						Android.Resource.Id.Text1,
						(from f in files select f.Title).ToArray());

					if (files.Count <= 0)
						Toast.MakeText(this, "No results found!", ToastLength.Short).Show();
				}

				buttonSearch.Enabled = true;
			};

			buttonSearch.Enabled = false;

			googleApiClient = new GoogleApiClient.Builder(this)
				.AddApi(DriveClass.API)
				.AddScope(DriveClass.ScopeFile)
				.AddScope(DriveClass.ScopeAppfolder)
				.UseDefaultAccount()
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.Build();
		}

		protected override void OnStart()
		{
			base.OnStart();
			googleApiClient.Connect();
		}

		public async void OnConnected(Bundle connectionHint)
		{
			buttonSearch.Enabled = true;
		}

		public void OnConnectionSuspended(int cause)
		{
			Console.WriteLine("Connection Suspended: {0}", cause);
		}

		public void OnConnectionFailed(ConnectionResult result)
		{

			if (result.HasResolution)
			{
				try
				{
					result.StartResolutionForResult(this, 101);
				}
				catch (IntentSender.SendIntentException ex)
				{
					// Unable to resolve, message user appropriately
				}
			}
			else {
				GooglePlayServicesUtil.GetErrorDialog(result.ErrorCode, this, 0).Show();
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			switch (requestCode)
			{
			case 101: // Something may have been resolved, try connecting again
				if (resultCode == Result.Ok)
					googleApiClient.Connect();
				break;
			}
		}
	}
}

