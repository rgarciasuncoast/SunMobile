using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public class EDocumentRequest
	{
		[DataMember]
		public string DocumentType { get; set; } // SunBlock.DataTransferObjects.OnBase.EDocumentTypes
		[DataMember]
		public int DocumentId { get; set; }
		[DataMember]
		public DateTime BeginTimeQuery { get; set; }
		[DataMember]
		public DateTime EndTimeQuery { get; set; }

	}
}