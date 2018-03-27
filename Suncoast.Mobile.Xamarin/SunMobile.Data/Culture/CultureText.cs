using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Culture
{
	[DataContract]
	public class CultureText : ResourceId
	{
		[DataMember]
		public string EnglishText { get; set; }

		[DataMember]
		public string SpanishText { get; set; }

		[DataMember]
		public string SpanishFontSize { get; set; } //FontSizes enum
	}
}