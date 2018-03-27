﻿using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
	[DataContract]
	public class TransferSourceAccountListRequest
	{
		[DataMember]
		public int ExcludeMemberId { get; set; }
		[DataMember]
		public string ExcludeSuffix { get; set; }
	}
}