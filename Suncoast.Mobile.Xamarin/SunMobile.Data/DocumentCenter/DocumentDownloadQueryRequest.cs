using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class DocumentDownloadQueryRequest
	{

		[DataMember]
		public int MemberId { get; set; }
		[DataMember]
		public long CaseId { get; set; }
	}
}