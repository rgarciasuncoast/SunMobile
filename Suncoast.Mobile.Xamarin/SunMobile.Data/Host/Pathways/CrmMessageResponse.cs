using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Messages.SecuredMessaging;

namespace SunBlock.DataTransferObjects.Host.Pathways
{
	[DataContract]
	public class CrmMessageResponse : StatusResponse
	{
		[DataMember]
		public int NewEnotificationsCount { get; set; }

		[DataMember]
		public int NewSecureMessagesCount { get; set; }

		[DataMember]
		public List<MessageThread> MessageThreads { get; set; }
	}
}
