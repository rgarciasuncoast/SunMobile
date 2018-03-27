using System;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.OS;
using SunMobile.Droid.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid
{
	[Activity(Label = "SelectGoogleDriveDocumentActivity", Theme = "@style/CustomHoloLightTheme")]
	public class SelectGoogleDriveDocumentActivity : BaseActivity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		private const string TAG = "drive";
		private const int REQUEST_CODE_SELECT = 102;
		private const int REQUEST_CODE_RESOLUTION = 103;
		private const long MAX_FILE_SIZE = 3000000;
		private GoogleApiClient _googleApiClient;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			BuildGoogleApiClient();
		}

		private void BuildGoogleApiClient()
		{
			try
			{
				if (_googleApiClient == null)
				{
					#pragma warning disable CS0618 // Type or member is obsolete
					_googleApiClient = new GoogleApiClient.Builder(this, this, this)
						.AddApi(DriveClass.Api)
						.AddScope(DriveClass.ScopeFile)
						  .AddScope(DriveClass.ScopeAppfolder)
						.Build();
					#pragma warning restore CS0618 // Type or member is obsolete
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectGoogleDriveDocumentActivity:BuildGoogleApiClient");
			}
		}

		protected override void OnStart()
		{
			try
			{
				base.OnStart();

				if (_googleApiClient != null)
				{
					_googleApiClient.Connect();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectGoogleDriveDocumentActivity:OnStart");
			}
		}

		protected override void OnStop()
		{
			try
			{
				base.OnStop();

				if (_googleApiClient != null)
				{
					_googleApiClient.Disconnect();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectGoogleDriveDocumentActivity:OnStart");
			}
		}

		public void OnConnected(Bundle connectionHint)
		{
			try
			{
				var intentSender = new OpenFileActivityBuilder()
				.SetMimeType(new string[] { "image/bmp", "image/jpeg", "image/png", "image/tiff", "application/pdf" })
				.Build(_googleApiClient);

				StartIntentSenderForResult(intentSender, REQUEST_CODE_SELECT, null, 0, 0, 0);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectGoogleDriveDocumentActivity:OnConnected");
			}
		}

		public void OnConnectionSuspended(int cause)
		{
			switch (cause)
			{
				case 1:
					Logging.Log("SelectGoogleDriveDocumentActivity:OnConnectionSuspended - Cause: " + "Service disconnected");
					break;
				case 2:
					Logging.Log("SelectGoogleDriveDocumentActivity:OnConnectionSuspended - Cause: " + "Connection lost");
					break;
				default:
					Logging.Log("SelectGoogleDriveDocumentActivity:OnConnectionSuspended - Cause: " + "Unknown");
					break;
			}
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			try
			{
				if (!result.HasResolution)
				{	
					#pragma warning disable CS0618 // Type or member is obsolete
					GooglePlayServicesUtil.GetErrorDialog(result.ErrorCode, this, 0).Show();
					#pragma warning restore CS0618 // Type or member is obsolete

					return;
				}

				result.StartResolutionForResult(this, REQUEST_CODE_RESOLUTION);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectGoogleDriveDocumentActivity:OnConnectionFailed");
			}
		}

		protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			try
			{
				switch (requestCode)
				{
					case REQUEST_CODE_SELECT:
						if (resultCode == Result.Ok)
						{
							//var fileSmallEnough = false;
							var driveId = (DriveId)data.GetParcelableExtra(OpenFileActivityBuilder.ExtraResponseDriveId);
							var driveFile = driveId.AsDriveFile();

							/*
							var metaResult = await driveFile.GetMetadataAsync(_googleApiClient);

							if (metaResult.Status.IsSuccess)
							{
								if (metaResult.Metadata.FileSize > MAX_FILE_SIZE)
								{
									await AlertMethods.Alert(this, "SunMobile", "The file selected is over the 3 megabyte limit.", "OK");
								}
								else
								{
									fileSmallEnough = true;
								}
							}
							else
							{
								await AlertMethods.Alert(this, "SunMobile", "Error retrieving Google Drive file size.", "OK");
							}
							*/

							ShowActivityIndicator("Retrieving Google Drive file...");

							var result = await driveFile.OpenAsync(_googleApiClient, DriveFile.ModeReadOnly, null);

							HideActivityIndicator();

							if (result != null && result.Status != null)
							{
								if (result.Status.IsSuccess)
								{
									var stream = result.DriveContents.InputStream;
									var bytes = Images.CompressImageBytes(Images.ConvertStreamToByteArray(stream));

									if (bytes.Length > MAX_FILE_SIZE)
									{
										await AlertMethods.Alert(this, "SunMobile", "The file selected is over the 3 megabyte limit.", "OK");
										SetResult(Result.Canceled);
									}
									else
									{
										var fileMetaData = await driveFile.GetMetadataAsync(_googleApiClient);
										var filePath = fileMetaData.Metadata.OriginalFilename;
										var fileName = System.IO.Path.GetFileName(filePath);
										var localFileName = IsolatedStorage.SaveBytesToFile(fileName, bytes);

										var intent = new Intent();
										intent.PutExtra("filename", localFileName);
										SetResult(Result.Ok, intent);
									}
								}
								else
								{
									await AlertMethods.Alert(this, "SunMobile", "Error retrieving Google Drive file.", "OK");
									SetResult(Result.Canceled);
								}
							}
							else
							{
								SetResult(Result.Canceled);
							}

							Finish();
						}
						break;
					case REQUEST_CODE_RESOLUTION:
						if (resultCode == Result.Ok)
						{
							_googleApiClient.Connect();
						}
						break;
					default:
						base.OnActivityResult(requestCode, resultCode, data);
						break;
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SelectGoogleDriveDocumentActivity:OnActivityResult");
			}
		}
	}
}