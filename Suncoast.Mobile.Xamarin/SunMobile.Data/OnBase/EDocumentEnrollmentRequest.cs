using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public class EDocumentEnrollmentRequest
	{
		[DataMember]
		public string DocumentType { get; set; } // SunBlock.DataTransferObjects.OnBase.EDocumentTypes
		[DataMember]
		public bool EnrollmentFlag { get; set; }
	}
}