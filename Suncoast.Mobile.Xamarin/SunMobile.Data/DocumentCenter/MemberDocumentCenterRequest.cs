using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class MemberDocumentCenterRequest
	{
		[DataMember]
		public int MemberId { get; set; }
	}
}