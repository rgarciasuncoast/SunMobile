using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.SecuredMessaging
{
	[DataContract]
	public class ComposeThreadRequest
	{
		[DataMember]
		public string MessageCategory { get; set; }
		[DataMember]
		public string MessageBody { get; set; }
	}
}