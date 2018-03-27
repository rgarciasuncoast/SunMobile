using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public class EDocumentIsEnrolledRequest
	{
		[DataMember]
		public string DocumentType { get; set; } // SunBlock.DataTransferObjects.OnBase.EDocumentTypes
	}
}