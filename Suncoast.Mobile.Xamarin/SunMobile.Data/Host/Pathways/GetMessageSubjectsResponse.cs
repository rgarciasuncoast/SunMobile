using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
	[DataContract]
	public class GetMessageSubjectsResponse : StatusResponse
	{
		[DataMember]
		public List<string> Subjects { get; set; }
		[DataMember]
		public List<string> DocumentTypes { get; set; }
	}
}