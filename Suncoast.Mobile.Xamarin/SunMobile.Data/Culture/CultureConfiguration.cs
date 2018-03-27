using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Culture
{
	[DataContract]
	public class CultureConfiguration
	{
		[DataMember]
		public DateTime LastUpdateTimeUtc { get; set; }

		[DataMember]
		public List<CultureView> ViewConfigurations { get; set; }
	}
}