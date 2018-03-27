using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.Mobile.Model.OnBase;
using SunMobile.Droid.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Views;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "UploadDisputeDocumentsActivity", Theme = "@style/CustomHoloLightTheme")]
	public class UploadDisputeDocumentsActivity : BaseListActivity
	{
        private TextView txtTitle;
		private ImageButton btnCloseWindow;
		private Button btnAddFiles;
		private TextView txtContinue;
		private List<FileInformation> _fileList;
		private const long MAX_FILE_SIZE = 3000000;		
		private string MAX_FILE_SIZE_MESSAGE = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "9952E938-D708-46D6-AA56-5E9AE4C74F65", "File size exceeds 3 megabytes.");
		private string QUEUED = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "7876686D-1420-49F8-9405-28C8418F8A6A", "Queued");

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (savedInstanceState != null)
			{
				var json = savedInstanceState.GetString("FileList");
				_fileList = JsonConvert.DeserializeObject<List<FileInformation>>(json);
			}

			SetupView(Resource.Layout.UploadDocumentsView);

			_fileList = new List<FileInformation>();

            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			btnAddFiles = FindViewById<Button>(Resource.Id.btnAddFiles);
			btnAddFiles.Click += (sender, e) =>
			{
				AddFiles();
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
			var json = JsonConvert.SerializeObject(_fileList);
			outState.PutString("FileList", json);

			base.OnSaveInstanceState(outState);
		}

		public override void SetCultureConfiguration()
		{
			CultureTextProvider.SetMobileResourceText(txtTitle, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0CF06088-6480-4082-AC54-87D71A38145A", "Upload Photos");
            CultureTextProvider.SetMobileResourceText(btnAddFiles, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0CF06088-6480-4082-AC54-87D71A38145A", "Upload Photo");
            txtContinue.Text = CultureTextProvider.CONTINUE();
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
                    await AlertMethods.Alert(this, "SunMobile", MAX_FILE_SIZE_MESSAGE, CultureTextProvider.OK());
				}
				else
				{
					fileInfo.Base64String = Images.ConvertStreamToBitmapToBase64StringWithCompression(stream);
                    fileInfo.Status = QUEUED;
					_fileList.Add(fileInfo);
				}

				stream = null;

				DisplayFiles();
			}
		}

		private async void UploadFiles()
		{
			try
			{
				var methods = new AccountMethods();

                int count = _fileList.Count(x => x.Status == QUEUED);

				if (count > 0)
				{
					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "27FDDA93-DC12-4376-A885-4DB5145A85B7", "Uploading files."));

					foreach (var file in _fileList)
					{
						if (file.Status == QUEUED)
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
								file.Status = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "6A12B6A2-6CDE-4B72-A47C-97641C2186A6", "Uploaded");
							}
							else
							{
								file.Status = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "7E9EAE21-F2D2-4804-95DB-2545C35EA1FC", "Upload Error");
							}
						}

						// Clear the string after upload.
						file.Base64String = string.Empty;
					}

					HideActivityIndicator();
					DisplayFiles();
				}

				var intent = new Intent();
				var json = JsonConvert.SerializeObject(_fileList);
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