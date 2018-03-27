using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.Messages.SecuredMessaging
{
	[DataContract]
	public class MessageThread
	{
		[Queryable]
		[DataMember]
		public Guid Id { get; set; }

		[Queryable]
		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public List<Message> Messages { get; set; }

		[Queryable]
		[DataMember]
		public string Status { get; set; }

		/*
        Open,
        Assigned,
        Allotted,
        PendingQueue,
        PendingReAssign,
        ClosedByMember,
        Closed
        */

		[Queryable]
		[DataMember]
		public string Category { get; set; }

		[Queryable]
		[SensitiveData]
		[DataMember]
		public int MemberId { get; set; }

		[DataMember]
		public DateTime OpenedDateUtc { get; set; }

		[DataMember]
		public DateTime ClosedDateUtc { get; set; }

		[Queryable]
		[DataMember]
		public Guid MessageTypeId { get; set; }

		[DataMember]
		public Guid AgentId { get; set; }

		[DataMember]
		public DateTime LastUpdatedUtc { get; set; }

		[DataMember]
		public bool ElectronicStatementAssociated { get; set; }

		[DataMember]
		public bool IsNotification { get; set; }

		#region OnSite Fields

		[DataMember]
		public string CrmTypeDescription { get; set; }

		[DataMember]
		public long ConversationNo { get; set; }

		#endregion
	}
}