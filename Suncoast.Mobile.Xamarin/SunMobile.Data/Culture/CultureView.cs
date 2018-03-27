using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Culture
{
	[DataContract]
	public class CultureView : ResourceId
	{
		[DataMember]
		public string ViewName { get; set; }

		[DataMember]
		public List<CultureText> TextConfigurations { get; set; }
	}
}