using System;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Attributes;

namespace SunBlock.DataTransferObjects.Messages.SecuredMessaging
{
	[DataContract]
	public class Message
	{
		[Queryable]
		[DataMember]
		public Guid Id { get; set; }

		[Queryable]
		[DataMember]
		public DateTime EntryDateUtc { get; set; }

		[DataMember]
		[SensitiveData]
		public string Body { get; set; }

		[DataMember]
		public bool DeletedByMember { get; set; }

		[DataMember]
		public bool SentByMember { get; set; }

		[DataMember]
		public DateTime LastUpdatedUtc { get; set; }

		[DataMember]
		public Guid AgentId { get; set; }

		[DataMember]
		public bool ReadByMember { get; set; }

		[DataMember]
		public bool Recalled { get; set; }

		[DataMember]
		public bool ShowExternally { get; set; }

		[DataMember]
		public bool RemoveFromCaseAssignment { get; set; }

		#region OnSite Fields

		[DataMember]
		public bool FromCrm { get; set; }

		[DataMember]
		public bool DocumentUploadAssociated { get; set; }

		[DataMember]
		public bool DocumentDownloadAssociated { get; set; }

		#endregion
	}
}