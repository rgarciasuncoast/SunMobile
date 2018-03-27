using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public class DocumentCenterFile
	{
		[DataMember]
		public string URL { get; set; }
		[DataMember]
		public string FileId { get; set; }
		[DataMember]
		public DateTime FileTimeUtc { get; set; }
		[DataMember]
		public string FileName { get; set; }
		[DataMember]
		public string MimeType { get; set; }
		[DataMember]
		public string OnBaseImageDocumentType { get; set; } //EDocumentTypes Enum for OnBase Document queries
        [DataMember]
        public byte[] FileBytes { get; set; }
	}
}