using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Culture
{
	[DataContract]
	public class ResourceId
	{
		[DataMember]
		public string Id { get; set; }
	}
}