using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Security
{

	[DataContract]
	public class SecurityScanRequest
	{
		[DataMember]
		public string MemberId { get; set; }
		[DataMember]
		public string DocumentId { get; set; }
		[DataMember]
		public string OriginalFileName { get; set; }
		[DataMember]
		public byte[] Contents { get; set; }
		[DataMember]
		public bool StoreFileForLaterUse { get; set; }
	}
}