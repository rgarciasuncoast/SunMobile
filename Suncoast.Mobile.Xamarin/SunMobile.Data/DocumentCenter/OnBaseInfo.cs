using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Collections;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class OnBaseInfo
	{
		[DataMember]
		public string DocumentType { get; set; }
		[DataMember]
		public string ContentType { get; set; }
		[DataMember]
		public NameValueCollection Keywords { get; set; }
	}
}
