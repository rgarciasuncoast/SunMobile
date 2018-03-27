using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class UploadDocumentRequest
	{
		[DataMember]
		public int MemberId { get; set; }
		[DataMember]
		public string UploadDocumentId { get; set; }
		[DataMember]
		public List<UploadFileRequest> Files { get; set; }
	}
}