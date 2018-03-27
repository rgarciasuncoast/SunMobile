using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;
using Plugin.Media.Abstractions;
using SunBlock.DataTransferObjects.Security;
using SunMobile.iOS.Common;
using SunMobile.Shared;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Documents
{
	public partial class DocumentUploadViewController : BaseViewController
	{
		public int MaxNumberOfFiles { get; set; }
		public event Action<List<FileInformation>> Completed = delegate { };
		private List<FileInformation> _fileList;
		private const long MAX_FILE_SIZE = 3000000;
		private const string MAX_FILE_SIZE_MESSAGE = "File size is more than 3 megabytes, upload again.";

		public DocumentUploadViewController(IntPtr handle) : base(handle)
		{
			_fileList = new List<FileInformation>();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var rightButton = new UIBarButtonItem("Continue", UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => Done();

			btnUploadPhoto.Clicked += (sender, e) => SelectPhoto();
			btnUploadiCloud.Clicked += (sender, e) => SelectiCloudDocument();
		}

		private async Task<bool> HaveMaxNumberOfFilesBeenAdded(bool displayMessage)
		{
			bool returnValue = false;

			if (MaxNumberOfFiles != 0 && _fileList.FindAll(x => x.Status == "Scanned").Count >= MaxNumberOfFiles)
			{
				returnValue = true;

				if (displayMessage)
				{
					await AlertMethods.Alert(View, "SunMobile", $"The maximum number of files you can upload is {MaxNumberOfFiles}.", "OK");
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
							await AlertMethods.Alert(View, "SunMobile", MAX_FILE_SIZE_MESSAGE, "OK");
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
				Logging.Log(ex, "DocumentUploadViewController:SelectPhotos");
			}
		}

		private async void SelectiCloudDocument()
		{
			try
			{
				if (!await HaveMaxNumberOfFilesBeenAdded(true))
				{
					var allowedUTIs = new string[]
					{
						UTType.PNG,
						UTType.JPEG,
						UTType.BMP,
						UTType.PDF,
						UTType.TIFF
					};

					var documentPicker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);

					documentPicker.DidPickDocument += async (sender, e) =>
					{
						var fileInfo = new FileInformation();

						try
						{
							e.Url.StartAccessingSecurityScopedResource();
							var data = NSData.FromUrl(e.Url);
							fileInfo.FileName = e.Url.LastPathComponent;
							fileInfo.PathAndFileName = e.Url.LastPathComponent;
							fileInfo.FileBytes = Images.CompressImageBytes(data.ToArray());
							fileInfo.MimeType = DocumentMethods.GetMimeTypeFromFileName(fileInfo.PathAndFileName);

							if (fileInfo.FileBytes.Length > MAX_FILE_SIZE)
							{
								await AlertMethods.Alert(View, "SunMobile", MAX_FILE_SIZE_MESSAGE, "OK");
							}
							else
							{
								fileInfo.Status = "Queued";
								_fileList.Add(fileInfo);
								UploadFile();
							}
						}
						catch (Exception ex)
						{
							Logging.Log(ex, "DocumentUploadViewController:SelectiCloudDocuments");
						}
						finally
						{
							e.Url.StopAccessingSecurityScopedResource();
						}
					};

					documentPicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
					PresentViewController(documentPicker, true, null);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "DocumentUploadViewController:SelectiCloudDocuments");
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
					ShowActivityIndicator();

					foreach (var file in _fileList)
					{
						if (file.Status == "Queued")
						{
							var request = new SecurityScanRequest
							{
								OriginalFileName = file.FileName,
								Contents = file.FileBytes,
								StoreFileForLaterUse = true
							};

							var response = await methods.SecurityScanDocument(request, View);

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
				Logging.Log(ex, "DocumentUploadViewController:UploadFiles");
			}
			finally
			{
				HideActivityIndicator();
			}
		}

		private void RemoveFile(int index)
		{
			_fileList.RemoveAt(index);
			DisplayFiles();
		}

		private void Done()
		{
			Completed(_fileList);
			NavigationController.PopViewController(true);
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

			var tableViewSource = new DocumentUploadTableViewSource(listViewItems);
			tableViewSource.RemoveSelected += RemoveFile;

			tableViewMain.Source = tableViewSource;
			tableViewMain.ReloadData();

			NavigationItem.RightBarButtonItem.Enabled = _fileList.Count > 0;
		}
	}
}