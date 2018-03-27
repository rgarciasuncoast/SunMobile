using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using SunBlock.DataTransferObjects.Security;
using SunMobile.Droid.Accounts;
using SunMobile.Shared;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Documents
{
	public class DocumentUploadFragment : BaseListFragment
	{
		public int MaxNumberOfFiles { get; set; }
		public event Action<List<FileInformation>> Completed = delegate { };
		private ImageButton btnCloseWindow;
		private Button btnAddFiles;
		private Button btnGoogleDrive;
		private TextView txtContinue;
		private List<FileInformation> _fileList;
		private const long MAX_FILE_SIZE = 3000000;
		private const string MAX_FILE_SIZE_MESSAGE = "File size is more than 3 megabytes, upload again.";

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.UploadDocumentsGoogleDriveView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			var json = JsonConvert.SerializeObject(_fileList);
			outState.PutString("FileList", json);

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Document Upload");

			_fileList = new List<FileInformation>();

			btnCloseWindow = Activity.FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => NavigationService.NavigatePop(false);

			btnAddFiles = Activity.FindViewById<Button>(Resource.Id.btnAddFiles);
			btnAddFiles.Click += (sender, e) => SelectPhoto();

			btnGoogleDrive = Activity.FindViewById<Button>(Resource.Id.btnGoogleDrive);
			btnGoogleDrive.Click += (sender, e) => SelectGoogleDriveDocument();

			txtContinue = Activity.FindViewById<TextView>(Resource.Id.txtContinueDocumentsUpload);
			txtContinue.Click += (sender, e) => Done();

			DisplayFiles();
		}

		private async Task<bool> HaveMaxNumberOfFilesBeenAdded(bool displayMessage)
		{
			bool returnValue = false;

			if (MaxNumberOfFiles != 0 && _fileList.FindAll(x => x.Status == "Scanned").Count >= MaxNumberOfFiles)
			{
				returnValue = true;

				if (displayMessage)
				{
					await AlertMethods.Alert(Activity, "SunMobile", $"The maximum number of files you can upload is {MaxNumberOfFiles}.", "OK");
				}
			}

			return returnValue;
		}

		private async void SelectPhoto()
		{
			try
			{
				if (!await HaveMaxNumberOfFilesBeenAdded(true))
				{
					var options = new PickMediaOptions();
					options.CompressionQuality = 30;

					var mediaFile = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(options);

					if (mediaFile != null)
					{
						var fileInfo = new FileInformation();
						fileInfo.PathAndFileName = mediaFile.Path;
						fileInfo.FileName = Path.GetFileName(mediaFile.Path);
						var stream = mediaFile.GetStream();

						if (stream.Length > MAX_FILE_SIZE)
						{
							await AlertMethods.Alert(Activity, "SunMobile", MAX_FILE_SIZE_MESSAGE, "OK");
						}
						else
						{
							fileInfo.FileBytes = Images.ConvertStreamToByteArray(stream);
							fileInfo.Status = "Queued";
							fileInfo.MimeType = "image/jpeg";
							_fileList.Add(fileInfo);
							UploadFile();
						}

						stream = null;
					}				
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DocumentUploadFragment:SelectPhoto");
			}
		}

		private async void SelectGoogleDriveDocument()
		{
			try
			{
				if (!await HaveMaxNumberOfFilesBeenAdded(true))
				{
					var intent = new Intent(Activity, typeof(SelectGoogleDriveDocumentActivity));
					StartActivityForResult(intent, 100);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "UploadDisputeDocumentsActivity:SelectGoogleDriveDocument");
			}
		}

		private async void UploadFile()
		{
			try
			{
				var methods = new DocumentMethods();

				int count = _fileList.Count(x => x.Status == "Queued");

				if (count > 0)
				{
					ShowActivityIndicator("Scanning file...");

					foreach (var file in _fileList)
					{
						if (file.Status == "Queued")
						{
							var request = new SecurityScanRequest
							{
								OriginalFileName = file.FileName,
								Contents = file.FileBytes,
								StoreFileForLaterUse = false
							};

							var response = await methods.SecurityScanDocument(request, Activity);

							if (response != null && response.Success)
							{
								file.Status = response.IsFileInfected ? "Infected" : "Scanned";
							}
							else
							{
								file.Status = "Scan Error";
							}
						}
					}

					HideActivityIndicator();
				}

				DisplayFiles();

				if (MaxNumberOfFiles == 1 && await HaveMaxNumberOfFilesBeenAdded(false))
				{
					Done();
				}
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

		private void Done()
		{
			Completed(_fileList);
			NavigationService.NavigatePop(false);
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
			var listAdapter = new UpdateDisputeDocumentsListAdapter(Activity, Resource.Layout.UploadFilesListViewItem, listViewItems, resourceIds, fields);
			ListAdapter = listAdapter;

			listAdapter.RemoveSelected += RemoveFile;
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == (int)Result.Ok && data != null)
			{
				if (requestCode == 100)
				{
					var localFileName = data.GetStringExtra("filename");
					var fileInfo = new FileInformation();
					fileInfo.PathAndFileName = localFileName;
					fileInfo.FileName = Path.GetFileName(localFileName);
					var bytes = IsolatedStorage.LoadBytesFromFile(localFileName);
					fileInfo.FileBytes = bytes;
					fileInfo.MimeType = DocumentMethods.GetMimeTypeFromFileName(fileInfo.PathAndFileName);
					bytes = null;

					fileInfo.Status = "Queued";
					_fileList.Add(fileInfo);
					UploadFile();
				}
			}
		}
	}
}