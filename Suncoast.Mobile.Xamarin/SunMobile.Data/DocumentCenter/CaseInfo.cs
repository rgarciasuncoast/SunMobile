using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class CaseInfo
	{
		[DataMember]
		public string CaseId { get; set; }
		[DataMember]
		public string AgentId { get; set; }
	}
}
