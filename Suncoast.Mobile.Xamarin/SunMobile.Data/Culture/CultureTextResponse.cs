using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Culture
{
	[DataContract]
	public class CultureTextResponse
	{
		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public FontSizes FontSize { get; set; }
	}
}