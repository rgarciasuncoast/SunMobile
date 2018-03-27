using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class DocumentUpload : DocumentCenterItem
	{
		[DataMember]
		public string UploadRequestType { get; set; } //UploadRequestTypes enum
		[DataMember]
		public List<DocumentCenterFile> Files { get; set; }
		[DataMember]
		public string StatusType { get; set; } //DocumentUploadStatusTypes enum
		[DataMember]
		public string StatusDescription { get; set; }
	}
}
