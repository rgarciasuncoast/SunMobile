using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.OnBase
{
	[DataContract]
	public class StoreAndScanDocumentRequest
	{
		[DataMember]
		public string DocumentBase64String { get; set; }
		[DataMember]
		public string FileName { get; set; }
		[DataMember]
		public string ContentType { get; set; }
	}
}