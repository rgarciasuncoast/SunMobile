using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class UploadFileRequest
	{
		[DataMember]
		public string FileName { get; set; }
		[DataMember]
		public byte[] FileBytes { get; set; }
		[DataMember]
		public string MimeType { get; set; }
	}
}