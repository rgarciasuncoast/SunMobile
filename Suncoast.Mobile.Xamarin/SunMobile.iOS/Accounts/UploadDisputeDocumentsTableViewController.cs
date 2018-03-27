using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SunBlock.DataTransferObjects.Mobile.Model.OnBase;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Images;
using SunMobile.Shared.Views;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public partial class UploadDisputeDocumentsTableViewController : BaseViewController
	{
		public event Action<List<FileInformation>> Completed = delegate { };
		private List<FileInformation> _fileList;
		private long MAX_FILE_SIZE = 3000000;
        private string MAX_FILE_SIZE_MESSAGE = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "9952E938-D708-46D6-AA56-5E9AE4C74F65", "File size exceeds 3 megabytes.");
        private string QUEUED = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "7876686D-1420-49F8-9405-28C8418F8A6A", "Queued");

		public UploadDisputeDocumentsTableViewController (IntPtr handle) : base(handle)
		{
			_fileList = new List<FileInformation>();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var rightButton = new UIBarButtonItem(CultureTextProvider.CONTINUE(), UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => UploadFiles();

			btnUploadPhotos.Clicked += (sender, e) =>
			{
				AddFiles();
			};		
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "C1E99194-9061-4F57-8D02-BE1D6CACF341", "Upload Documents");	
            CultureTextProvider.SetMobileResourceText(btnUploadPhotos, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0CF06088-6480-4082-AC54-87D71A38145A", "Upload Photo");
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
                    await AlertMethods.Alert(View, "SunMobile", MAX_FILE_SIZE_MESSAGE, CultureTextProvider.OK());
				}
				else
				{
					fileInfo.Base64String = Images.ConvertStreamToUIImageToBase64StringWithCompression(stream);
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
					ShowActivityIndicator();

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
				}

				DisplayFiles();

				Completed(_fileList);

				NavigationController.PopViewController(true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "UploadDisputeDocumentsTableViewController:UploadFiles");
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

			var tableViewSource = new UploadDisputeDocumentsTableViewSource(listViewItems);
			tableViewSource.RemoveSelected += RemoveFile;

			tblViewUploadedDocuments.Source = tableViewSource;
			tblViewUploadedDocuments.ReloadData();

			NavigationItem.RightBarButtonItem.Enabled = _fileList.Count > 0;
		}
	}
}