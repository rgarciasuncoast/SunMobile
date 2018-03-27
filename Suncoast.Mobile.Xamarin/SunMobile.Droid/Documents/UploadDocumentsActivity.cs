using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Widget;
using Common.Utilities.Serialization;
using Java.Lang;
using SunBlock.DataTransferObjects.Mobile.Model.OnBase;
using SunMobile.Droid.Accounts;
using SunMobile.Droid.Common;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Documents
{
	[Activity(Label = "UploadDisputeDocumentsActivity", Theme = "@style/CustomHoloLightTheme")]
	public class UploadDocumentsActivity : BaseListActivity
	{
		private ImageButton btnCloseWindow;
		private Button btnAddFiles;
		private Button btnGoogleDrive;
		private TextView txtContinue;
		private List<FileInformation> _fileList;
		private const long MAX_FILE_SIZE = 1000000;
		private const string MAX_FILE_SIZE_MESSAGE = "File size exceeds 1 megabyte.";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("FileList");
				_fileList = Json.Deserialize<List<FileInformation>>(json);
			}

			SetupView(Resource.Layout.UploadDocumentsGoogleDriveView);

			_fileList = new List<FileInformation>();

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			btnAddFiles = FindViewById<Button>(Resource.Id.btnAddFiles);
			btnAddFiles.Click += (sender, e) =>
			{
				AddFiles();
			};

			btnGoogleDrive = FindViewById<Button>(Resource.Id.btnGoogleDrive);
			btnGoogleDrive.Click += (sender, e) =>
			{
				GoogleDrive();
			};

			txtContinue = FindViewById<TextView>(Resource.Id.txtContinueDocumentsUpload);
			txtContinue.Click += (sender, e) =>
			{
				UploadFiles();
			};

			DisplayFiles();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			var json = Json.Serialize(_fileList);
			outState.PutString("FileList", json);

			base.OnSaveInstanceState(outState);
		}

		private async void AddFiles()
		{
			var mediaFile = await Plugin.Media.CrossMedia.Current.PickPhotoAsync();

			if (mediaFile != null)
			{
				var fileInfo = new FileInformation();
				fileInfo.PathAndFileName = mediaFile.Path;
				fileInfo.FileName = Path.GetFileName(mediaFile.Path);
				var stream = mediaFile.GetStream();

				if (stream.Length > MAX_FILE_SIZE)
				{
					await AlertMethods.Alert(this, "SunMobile", MAX_FILE_SIZE_MESSAGE, "OK");
				}
				else
				{
					fileInfo.Base64String = Images.ConvertStreamToBitmapToBase64StringWithCompression(stream);
					fileInfo.Status = "Queued";
					_fileList.Add(fileInfo);
				}

				stream = null;

				DisplayFiles();
			}
		}

		private async void GoogleDrive()
		{
			try
			{
	//			var googleApiClient = new GoogleApiClient.Builder(this)
	//			.AddApi(DriveClass.API)
	//			.AddScope(DriveClass.ScopeFile)
	//			.AddScope(DriveClass.ScopeAppfolder)
	//			.UseDefaultAccount()
	//			.Build();
	//			IntentSender intentSender = DriveClass.DriveApi.NewOpenFileActivityBuilder().SetMimeType(new string[] { DriveFolder.MimeType }).Build();
 //.setMimeType(new String[] { DriveFolder.MIME_TYPE })  // <--- FOLDER
	//												   //.setMimeType(new String[] { "text/plain", "text/html" }) // <- TEXT FILES
 //.build(getGoogleApiClient());
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "UploadDisputeDocumentsActivity:GoogleDrive");
			}
		}

		private async void UploadFiles()
		{
			try
			{
				var methods = new AccountMethods();

				int count = _fileList.Count(x => x.Status == "Queued");

				if (count > 0)
				{
					ShowActivityIndicator("Uploading files.");

					foreach (var file in _fileList)
					{
						if (file.Status == "Queued")
						{
							var request = new StoreAndScanDocumentRequest
							{
								FileName = file.FileName,
								ContentType = "",
								DocumentBase64String = file.Base64String
							};

							var response = await methods.StoreAndScanDocument(request, this);

							if (response?.Success != null)
							{
								file.FileId = response.Result;
								file.Status = "Uploaded";
							}
							else
							{
								file.Status = "Upload Error";
							}
						}

						// Clear the string after upload.
						file.Base64String = string.Empty;
					}

					HideActivityIndicator();
					DisplayFiles();
				}

				var intent = new Intent();
				var json = Json.Serialize(_fileList);
				intent.PutExtra("FileList", json);
				SetResult(Result.Ok, intent);
				Finish();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "UploadDisputeDocumentsActivity:UploadFiles");
			}
		}

		private void RemoveFile(ListViewItem item)
		{
			_fileList.RemoveAll(x => x.FileName == item.Item1Text);
			DisplayFiles();
		}

		private void DisplayFiles()
		{
			var listViewItems = new List<ListViewItem>();

			foreach (var file in _fileList)
			{
				var listViewItem = new ListViewItem
				{
					Item1Text = file.FileName,
					Item2Text = file.Status
				};

				listViewItems.Add(listViewItem);
			}

			int[] resourceIds = { Resource.Id.lblFileName, Resource.Id.lblStatus };
			string[] fields = { "Item1Text", "Item2Text" };
			var listAdapter = new UpdateDisputeDocumentsListAdapter(this, Resource.Layout.UploadFilesListViewItem, listViewItems, resourceIds, fields);
			ListAdapter = listAdapter;

			listAdapter.RemoveSelected += RemoveFile;
		}
	}
}