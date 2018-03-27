using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.DocumentCenter
{
	[DataContract]
	public abstract class DocumentCenterItem
	{
		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public DateTime DocumentTimeUtc { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public bool Active { get; set; }
		[DataMember]
		public OnBaseInfo OnBaseInfo { get; set; }
		[DataMember]
		public CaseInfo CaseInfo { get; set; }
	}
}