using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Mobile.Model.SecuredMessaging
{
	[DataContract]
	public class ReplyToThreadRequest
	{
		[DataMember]
		public Guid MessageTypeId { get; set; }
		[DataMember]
		public string MessageBody { get; set; }
	}
}