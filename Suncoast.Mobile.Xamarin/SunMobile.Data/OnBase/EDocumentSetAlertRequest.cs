using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public class EDocumentSetAlertRequest
	{
		[DataMember]
		public string EmailAddress { get; set; }
		[DataMember]
		public string AlertType { get; set; }
		[DataMember]
		public bool AlertEnabled { get; set; }
	}
}