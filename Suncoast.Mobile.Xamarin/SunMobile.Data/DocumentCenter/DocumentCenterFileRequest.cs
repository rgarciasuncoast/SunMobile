using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class DocumentCenterFileRequest
	{
		[DataMember]
		public int MemberId { get; set; }
		[DataMember]
		public string FileId { get; set; }
		[DataMember]
		public bool MarkAsRead { get; set; }
	}
}