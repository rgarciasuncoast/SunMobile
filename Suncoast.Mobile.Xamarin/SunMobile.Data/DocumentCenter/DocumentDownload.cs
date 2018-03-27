using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class DocumentDownload : DocumentCenterItem
	{
		[DataMember]
		public DocumentCenterFile File { get; set; }
		[DataMember]
		public string ViewStatus { get; set; } //ViewStatusTypes enum
	}
}