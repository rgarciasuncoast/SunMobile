using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Security
{
	[DataContract]
	public class SecurityScanResponse : StatusResponse
	{
		[DataMember]
		public bool IsFileInfected { get; set; }
		[DataMember]
		public StoredFileKey StoredDocumentKey { get; set; }
		[DataMember]
		public string ScannerOutput { get; set; }
		[DataMember]
		public string OriginalFileName { get; set; }
	}
}