using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Security
{
	[DataContract]
	public class StoredFileKey
	{
		[DataMember]
		public string DocumentId { get; set; }
		[DataMember]
		public string MemberId { get; set; }
	}
}