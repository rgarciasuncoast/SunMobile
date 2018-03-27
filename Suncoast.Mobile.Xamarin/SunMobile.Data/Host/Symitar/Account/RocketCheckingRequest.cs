using System;
using System.Runtime.Serialization;

namespace SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account
{
	[DataContract]
	public class RocketCheckingRequest
	{
		[DataMember]
		public int MemberId { get; set; }
		[DataMember]
		public string FundingSuffix { get; set; }
		[DataMember]
		public decimal FundingAmount { get; set; }
		[DataMember]
		public bool CreateDebitCard { get; set; }
		[DataMember]
		public bool EnrollInEstatements { get; set; }
		[DataMember]
		public int ServiceCode { get; set; }
	}
}