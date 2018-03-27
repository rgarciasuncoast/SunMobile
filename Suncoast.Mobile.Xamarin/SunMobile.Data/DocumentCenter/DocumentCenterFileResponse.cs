using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class DocumentCenterFileResponse
	{
		[DataMember]
		public DocumentCenterFile FileInfo { get; set; }
		[DataMember]
		public byte[] FileBytes { get; set; }
	}
}