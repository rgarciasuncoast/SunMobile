using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class MemberDocumentCenter
	{
		[DataMember]
		public List<DocumentUpload> UploadDocuments { get; set; }

		[DataMember]
		public List<DocumentDownload> DownloadDocuments { get; set; }
	}
}