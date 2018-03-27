using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.OnBase;
using SunBlock.DataTransferObjects.Security;
using SunMobile.Shared.Data;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared
{
	public class DocumentMethods : SunBlockServiceBase
	{
		public Task<MobileStatusResponse<MemberDocumentCenter>> GetMemberDocumentCenter(MemberDocumentCenterRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetMemberDocumentCenter";
			var response = PostToSunBlock<MobileStatusResponse<MemberDocumentCenter>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<MobileStatusResponse<DocumentCenterFileResponse>> GetDocumentCenterFile(DocumentCenterFileRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetDocumentCenterFile";
			var response = PostToSunBlock<MobileStatusResponse<DocumentCenterFileResponse>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<MobileStatusResponse> UploadDocumentCenterFile(UploadDocumentRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/UploadDocumentCenterFile";
			var response = PostToSunBlock<MobileStatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<SecurityScanResponse> SecurityScanDocument(SecurityScanRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SecurityScanDocument";
			var response = PostToSunBlock<SecurityScanResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<List<DocumentDownload>>> QueryDownloadDocuments(DocumentDownloadQueryRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/QueryDownloadDocuments";
			var response = PostToSunBlock<StatusResponse<List<DocumentDownload>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		#region eStatements

		public Task<StatusResponse<bool>> IsEDocumentEnrolled(EDocumentIsEnrolledRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/IsEDocumentEnrolled";
			var response = PostToSunBlock<StatusResponse<bool>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> SetEDocumentEnrollment(EDocumentEnrollmentRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SetEDocumentEnrollment";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<List<ImageDocument>>> GetEDocuments(EDocumentRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/GetEDocuments";
			var response = PostToSunBlock<StatusResponse<List<ImageDocument>>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<AlertSettings>> GetEDocumentAlertSettings(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetEDocumentAlertSettings";
			var response = PostToSunBlock<StatusResponse<AlertSettings>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> SetEDocumentAlertSettings(EDocumentSetAlertRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SetEDocumentAlertSettings";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		#endregion

		public async Task<MobileStatusResponse> UploadFiles(List<FileInformation> files, string documentId, object view)
		{
			var returnValue = new MobileStatusResponse
			{
				Success = true
			};

			try
			{
				if (files != null && files.FindAll(x => x.Status == "Scanned").Count > 0)
				{
					var methods = new DocumentMethods();
					var request = new UploadDocumentRequest
					{
						UploadDocumentId = documentId,
						Files = new List<UploadFileRequest>()
					};

					foreach (var file in files)
					{
						if (file.Status == "Scanned")
						{
							var uploadFile = new UploadFileRequest();
							uploadFile.FileName = file.FileName;
							uploadFile.FileBytes = file.FileBytes;
							uploadFile.MimeType = file.MimeType;
							request.Files.Add(uploadFile);
						}
					}

					returnValue = await methods.UploadDocumentCenterFile(request, view);
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "DocumentMethods:UploadFiles");
			}

			return returnValue;
		}

		public static string GetMimeTypeFromFileName(string fileName)
		{
			string mimeType = "image/jpeg";

			var fileExtension = fileName.Substring(fileName.IndexOf('.') + 1);

			switch (fileExtension)
			{
				case "bmp":
					mimeType = "image/bmp";
					break;
				case "jpg":
					mimeType = "image/jpeg";
					break;
				case "pdf":
					mimeType = "application/pdf";
					break;
				case "png":
					mimeType = "image/png";
					break;
				case "tif":
				case "tiff":
					mimeType = "image/tiff";
					break;
			}

			return mimeType;
		}

		public static string GetFileExtensionFromMimeType(string mimeType)
		{
			var returnValue = "jpg";

			switch (mimeType)
			{
				case "image/bmp":
					returnValue = "bmp";
					break;
				case "image/jpeg":
					returnValue = "jpg";
					break;
				case "application/pdf":
					returnValue = "pdf";
					break;
				case "image/png":
					returnValue = "png";
					break;
				case "image/tiff":
					returnValue = "tif";
					break;
			}

			return returnValue;
		}
	}
}