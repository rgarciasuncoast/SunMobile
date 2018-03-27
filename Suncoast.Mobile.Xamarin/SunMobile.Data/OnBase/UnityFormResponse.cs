using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public class UnityFormResponse : StatusResponse
	{
		[DataMember]
		public string DocumentId { get; set; }
		[DataMember]
		public string DisputeSeqNum { get; set; }
	}
}